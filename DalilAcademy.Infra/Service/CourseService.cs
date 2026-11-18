using DalilAcademy.Core.DTOs;
using DalilAcademy.Core.Exceptions;
using DalilAcademy.Core.Mappings;
using DalilAcademy.Core.Repository;
using DalilAcademy.Core.Service;
using FluentValidation;

namespace DalilAcademy.Infra.Service
{
	public class CourseService(
		ICourseRepository courseRepository,
		IValidator<CreateCourseDto> validator) : ICourseService
	{
		private const int MaxPageSize = 50;

		public Task<PageResult<CourseResponseDto>> GetPageAsync(
			int page, int pageSize, decimal? minPrice = null, decimal? maxPrice = null)
		{
			page = Math.Max(page, 1);
			pageSize = Math.Clamp(pageSize, 1, MaxPageSize);
			return courseRepository.GetPageAsync(page, pageSize, minPrice, maxPrice);
		}

		public async Task<CourseResponseDto?> GetByIdAsync(int id)
		{
			var course = await courseRepository.GetByIdAsync(id)
				?? throw new NotFoundException($"Course {id} not found");
			return course.ToDto();
		}

		public async Task<CourseResponseDto> CreateCourseAsync(CreateCourseDto dto)
		{
			await validator.ValidateAndThrowAsync(dto);
			var created = await courseRepository.CreateCourseAsync(dto.ToEntity());
			return created.ToDto();
		}

		public async Task UpdateCourseAsync(int id, UpdateCourseDto dto)
		{
			var course = await courseRepository.GetByIdAsync(id)
				?? throw new NotFoundException($"Course {id} not found");

			course.Title = dto.Title;
			course.Price = dto.Price;
            if (dto.ImageUrl is not null) course.ImageUrl = dto.ImageUrl;
            await courseRepository.UpdateCourseAsync(course);
		}

		public async Task DeleteCourseAsync(int id)
		{
			if (!await courseRepository.DeleteCourseAsync(id))
				throw new NotFoundException($"Course {id} not found");
		}

		public Task<int> CountAsync() => courseRepository.CountAsync();
	}
}