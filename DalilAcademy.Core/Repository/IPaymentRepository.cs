using DalilAcademy.Core.Entites;

namespace DalilAcademy.Core.Repository
{
    public interface IPaymentRepository
    {
        Task<Payment> CreateAsync(Payment payment);
        Task<Payment?> GetLatestCreatedAsync(int enrollmentId);
        Task<List<Payment>> GetByEnrollmentAsync(int enrollmentId);
        Task UpdateAsync(Payment payment);
    }
}