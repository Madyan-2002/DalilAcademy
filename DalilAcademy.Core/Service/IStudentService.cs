using DalilAcademy.Core.DTOs;

namespace DalilAcademy.Core.Service
{
    public interface IStudentService
    {
        Task<PageResult<StudentDto>> GetPagedAsync(int page, int pageSize);
        Task<StudentDto> GetByIdAsync(int id);
        Task<List<CourseResponseDto>> GetMyCoursesAsync(int studentId);
        Task<StudentDto> CreateStudentAsync(CreateStudentDto dto);
        Task UpdateStudentAsync(int id, UpdateStudentDto dto);
        Task DeleteStudentAsync(int id);
    }
}