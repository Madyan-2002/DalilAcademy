using DalilAcademy.Core.DTOs;
using DalilAcademy.Core.Entites;

namespace DalilAcademy.Core.Repository
{
    public interface ICourseRepository
    {
        Task<PageResult<CourseResponseDto>> GetPageAsync(
            int page, int pageSize, decimal? minPrice = null, decimal? maxPrice = null);
        Task<Course?> GetByIdAsync(int id);
        Task<Course> CreateCourseAsync(Course course);
        Task UpdateCourseAsync(Course course);
        Task<bool> DeleteCourseAsync(int id);
        Task<int> CountAsync();
    }
}