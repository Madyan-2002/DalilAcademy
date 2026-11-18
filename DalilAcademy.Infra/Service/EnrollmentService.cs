using DalilAcademy.Core.DTOs;
using DalilAcademy.Core.Entites;
using DalilAcademy.Core.Exceptions;
using DalilAcademy.Core.Mappings;
using DalilAcademy.Core.Repository;
using DalilAcademy.Core.Service;

namespace DalilAcademy.Infra.Service
{
    public class EnrollmentService(
        IEnrollmentRepository enrollments,
        IStudentRepository students,
        ICourseRepository courses) : IEnrollmentService
    {
        public async Task<List<EnrollmentResponseDto>> GetAllAsync()
        {
            var list = await enrollments.GetAllAsync();
            return list.Select(e => e.ToDto()).ToList();
        }

        public async Task<EnrollmentResponseDto> GetByIdAsync(int id)
        {
            var enrollment = await enrollments.GetByIdAsync(id)
                ?? throw new NotFoundException($"Enrollment {id} not found.");
            return enrollment.ToDto();
        }

        public async Task<EnrollmentResponseDto> CreateAsync(CreateEnrollmentDto dto)
        {
            var student = await students.GetByIdAsync(dto.StudentId)
                ?? throw new NotFoundException($"Student {dto.StudentId} not found.");

            var course = await courses.GetByIdAsync(dto.CourseId)
                ?? throw new NotFoundException($"Course {dto.CourseId} not found.");

            var enrollment = new Enrollment
            {
                StudentId = student.Id,
                CourseId = course.Id,
                Student = student,
                Course = course,
                // كورس مجاني ← Paid مباشرة
                Status = course.Price == 0 ? EnrollmentStatus.Paid : EnrollmentStatus.Pending
            };

            var created = await enrollments.CreateAsync(enrollment);
            return created.ToDto();
        }

        public async Task UpdateAsync(int id, UpdateEnrollmentDto dto)
        {
            var enrollment = await enrollments.GetByIdAsync(id)
                ?? throw new NotFoundException($"Enrollment {id} not found.");

            if (enrollment.Status == EnrollmentStatus.Paid)
                throw new ConflictException("A paid enrollment cannot be moved to another course.");

            var course = await courses.GetByIdAsync(dto.CourseId)
                ?? throw new NotFoundException($"Course {dto.CourseId} not found.");

            enrollment.Course = course;
            enrollment.CourseId = course.Id;

            await enrollments.UpdateAsync(enrollment);
        }

        public async Task DeleteAsync(int id)
        {
            var enrollment = await enrollments.GetByIdAsync(id)
                ?? throw new NotFoundException($"Enrollment {id} not found.");

            if (enrollment.Status == EnrollmentStatus.Paid)
                throw new ConflictException("A paid enrollment cannot be deleted.");

            await enrollments.DeleteAsync(enrollment);
        }

        public async Task<List<StudentDto>> GetPaidStudentsByCourseAsync(int courseId)
        {
            _ = await courses.GetByIdAsync(courseId)
                ?? throw new NotFoundException($"Course {courseId} not found.");

            var list = await enrollments.GetPaidStudentsByCourseAsync(courseId);
            return list.Select(s => s.ToDto()).ToList();
        }
    }
}