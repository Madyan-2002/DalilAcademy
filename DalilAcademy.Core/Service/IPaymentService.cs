using DalilAcademy.Core.DTOs;

namespace DalilAcademy.Core.Service
{
    public interface IPaymentService
    {
        Task<PayResponseDto> PayAsync(int enrollmentId, CancellationToken ct = default);
        Task<EnrollmentResponseDto> CaptureAsync(int enrollmentId, CancellationToken ct = default);
    }
}