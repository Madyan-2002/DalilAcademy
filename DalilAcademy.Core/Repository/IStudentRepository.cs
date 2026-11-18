using DalilAcademy.Core.Entites;
using DalilAcademy.Core.DTOs;

namespace DalilAcademy.Core.Repository
{
    public interface IStudentRepository
    {
        Task<List<Student>> GetAllStudentsAsync();
        Task<List<CourseResponseDto>?> GetCoursesByStudentIdAsync(int studentId, bool paidOnly = false);
        Task<(List<Student> Items, int TotalCount)> GetPagedAsync(int page, int pageSize);
        Task<Student> CreateStudentAsync(Student student);
        Task<bool> UpdateStudentAsync(int id, Student student);
        Task<bool> DeleteStudentAsync(int id);
        Task<Student?> GetByIdAsync(int id);
    }
}