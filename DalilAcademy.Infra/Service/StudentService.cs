using DalilAcademy.Core.DTOs;
using DalilAcademy.Core.Entites;
using DalilAcademy.Core.Exceptions;
using DalilAcademy.Core.Mappings;
using DalilAcademy.Core.Repository;
using DalilAcademy.Core.Service;
using FluentValidation;

namespace DalilAcademy.Infra.Service
{
    public class StudentService(
        IStudentRepository repository,
        IValidator<CreateStudentDto> createValidator,
        IValidator<UpdateStudentDto> updateValidator) : IStudentService
    {
        private const int MaxPageSize = 50;

        public async Task<PageResult<StudentDto>> GetPagedAsync(int page, int pageSize)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 1, MaxPageSize);

            var (items, total) = await repository.GetPagedAsync(page, pageSize);
            return new PageResult<StudentDto>(
                items.Select(s => s.ToDto()).ToList(), page, pageSize, total);
        }

        public async Task<StudentDto> GetByIdAsync(int id)
        {
            var student = await repository.GetByIdAsync(id)
                ?? throw new NotFoundException($"Student with id {id} was not found.");
            return student.ToDto();
        }

        public async Task<List<CourseResponseDto>> GetMyCoursesAsync(int studentId)
        {
            return await repository.GetCoursesByStudentIdAsync(studentId, paidOnly: true) ?? [];
        }

        public async Task<StudentDto> CreateStudentAsync(CreateStudentDto dto)
        {
            await createValidator.ValidateAndThrowAsync(dto);
            var created = await repository.CreateStudentAsync(dto.ToEntity());
            return created.ToDto();
        }

        public async Task UpdateStudentAsync(int id, UpdateStudentDto dto)
        {
            await updateValidator.ValidateAndThrowAsync(dto);

            var updated = await repository.UpdateStudentAsync(
                id, new() { FullName = dto.FullName, Email = dto.Email });

            if (!updated)
                throw new NotFoundException($"Student with id {id} was not found.");
        }

        public async Task DeleteStudentAsync(int id)
        {
            if (!await repository.DeleteStudentAsync(id))
                throw new NotFoundException($"Student with id {id} was not found.");
        }
    }
}