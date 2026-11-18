using DalilAcademy.Core.DTOs;

namespace DalilAcademy.Core.Service
{
	public interface ICourseService
	{
		Task<PageResult<CourseResponseDto>> GetPageAsync(
			int page, int pageSize, decimal? minPrice = null, decimal? maxPrice = null);
		Task<CourseResponseDto?> GetByIdAsync(int id);
		Task<CourseResponseDto> CreateCourseAsync(CreateCourseDto dto);
		Task UpdateCourseAsync(int id, UpdateCourseDto dto);
		Task DeleteCourseAsync(int id);
		Task<int> CountAsync();
	}
}