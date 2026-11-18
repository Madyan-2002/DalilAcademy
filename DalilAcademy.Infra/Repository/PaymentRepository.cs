using DalilAcademy.Core.Entites;
using DalilAcademy.Core.Repository;
using DalilAcademy.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace DalilAcademy.Infra.Repository
{
    public class PaymentRepository(AppDbContext db) : IPaymentRepository
    {
        public async Task<Payment> CreateAsync(Payment payment)
        {
            db.Payments.Add(payment);
            await db.SaveChangesAsync();
            return payment;
        }

        public async Task<Payment?> GetLatestCreatedAsync(int enrollmentId)
            => await db.Payments
                .Where(p => p.EnrollmentId == enrollmentId && p.Status == PaymentStatus.Created)
                .OrderByDescending(p => p.CreatedAt)
                .ThenByDescending(p => p.Id)
                .FirstOrDefaultAsync();

        public async Task<List<Payment>> GetByEnrollmentAsync(int enrollmentId)
            => await db.Payments
                .AsNoTracking()
                .Where(p => p.EnrollmentId == enrollmentId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

        // الـ entity متتبّع أصلاً، فيكفي SaveChanges
        public async Task UpdateAsync(Payment payment)
            => await db.SaveChangesAsync();
    }
}