using DalilAcademy.Core.Entites;

namespace DalilAcademy.Core.Repository
{
    public interface IEnrollmentRepository
    {
        Task<List<Enrollment>> GetAllAsync();
        Task<Enrollment?> GetByIdAsync(int id);
        Task<Enrollment> CreateAsync(Enrollment enrollment);
        Task UpdateAsync(Enrollment enrollment);
        Task DeleteAsync(Enrollment enrollment);
        Task<List<Student>> GetPaidStudentsByCourseAsync(int courseId);
    }
}