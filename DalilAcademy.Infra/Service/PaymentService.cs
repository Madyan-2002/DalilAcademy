using DalilAcademy.Core.DTOs;
using DalilAcademy.Core.Entites;
using DalilAcademy.Core.Exceptions;
using DalilAcademy.Core.Mappings;
using DalilAcademy.Core.Payments;
using DalilAcademy.Core.Repository;
using DalilAcademy.Core.Service;
using Microsoft.Extensions.Options;

namespace DalilAcademy.Infra.Service
{
    public class PaymentService(
        IEnrollmentRepository enrollments,
        IPaymentRepository payments,
        IPaymentGateway gateway,
        IOptions<PayPalSettings> options) : IPaymentService
    {
        public async Task<PayResponseDto> PayAsync(int enrollmentId, CancellationToken ct = default)
        {
            var enrollment = await enrollments.GetByIdAsync(enrollmentId)
                ?? throw new NotFoundException($"Enrollment {enrollmentId} not found.");

            if (enrollment.Status == EnrollmentStatus.Paid)
                throw new ConflictException("This enrollment is already paid.");

            // التحقق من أن سعر الكورس موجود وصحيح لتجنب قيمة الصفر
            var coursePrice = enrollment.Course?.Price ?? 0;
            if (coursePrice <= 0)
            {
                throw new BadRequestException("The course price cannot be zero or negative.");
            }

            var rate = options.Value.JodToUsdRate <= 0 ? 1.0m : options.Value.JodToUsdRate;

            // حساب المبلغ بالـ Server: السعر × معدل التحويل، مقرّب لخانتين
            var amount = Math.Round(
                coursePrice * rate,
                2, MidpointRounding.AwayFromZero);

            var order = await gateway.CreateOrderAsync(amount, ct);

            var payment = await payments.CreateAsync(new Payment
            {
                EnrollmentId = enrollment.Id,
                Amount = amount,
                Currency = "USD",
                PayPalOrderId = order.OrderId,
                Status = PaymentStatus.Created,
                CreatedAt = DateTime.UtcNow
            });

            return new PayResponseDto(payment.Id, order.OrderId, amount, "USD", order.ApprovalUrl);
        }

        public async Task<EnrollmentResponseDto> CaptureAsync(int enrollmentId, CancellationToken ct = default)
        {
            var enrollment = await enrollments.GetByIdAsync(enrollmentId)
                ?? throw new NotFoundException($"Enrollment {enrollmentId} not found.");

            if (enrollment.Status == EnrollmentStatus.Paid)
                throw new ConflictException("This enrollment is already paid.");

            var payment = await payments.GetLatestCreatedAsync(enrollmentId)
                ?? throw new BadRequestException("Call /pay first.");

            var completed = await gateway.CaptureOrderAsync(payment.PayPalOrderId, ct);

            if (!completed)
            {
                payment.Status = PaymentStatus.Failed;
                await payments.UpdateAsync(payment);

                throw new PaymentFailedException(
                    "The payment was not completed: the order may not be approved yet, " +
                    "already captured, or expired. Call /pay again.");
            }

            payment.Status = PaymentStatus.Completed;
            payment.PaidAt = DateTime.UtcNow;
            await payments.UpdateAsync(payment);

            enrollment.Status = EnrollmentStatus.Paid;
            await enrollments.UpdateAsync(enrollment);

            return enrollment.ToDto();
        }
    }
}