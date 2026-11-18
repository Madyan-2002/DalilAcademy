using DalilAcademy.Core.DTOs;
using DalilAcademy.Core.Entites;
using DalilAcademy.Core.Exceptions;
using DalilAcademy.Core.Repository;
using DalilAcademy.Infra.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace DalilAcademy.Infra.Repository
{
    public class StudentRepository(AppDbContext db) : IStudentRepository
    {
        public async Task<List<Student>> GetAllStudentsAsync()
            => await db.Students.AsNoTracking().ToListAsync();

        public async Task<Student?> GetByIdAsync(int id)
            => await db.Students.FindAsync(id);

        public async Task<Student> CreateStudentAsync(Student student)
        {
            db.Students.Add(student);
            await SaveAsync();
            return student;
        }

        public async Task<(List<Student> Items, int TotalCount)> GetPagedAsync(int page, int pageSize)
        {
            var query = db.Students.AsNoTracking();

            var total = await query.CountAsync();

            var items = await query
                .OrderBy(s => s.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }

        public async Task<List<CourseResponseDto>?> GetCoursesByStudentIdAsync(int studentId, bool paidOnly = false)
        {
            return await db.Students
                .AsNoTracking()
                .Where(s => s.Id == studentId)
                .Select(s => s.Enrollments
                    .Where(e => !paidOnly || e.Status == EnrollmentStatus.Paid)
                    .Select(e => new CourseResponseDto(
                        e.Course.Id,
                        e.Course.Title,
                        e.Course.Price,
                        e.Course.Instructor.FullName,
                        e.Course.ImageUrl))
                    .ToList())
                .FirstOrDefaultAsync();
        }
        public async Task<bool> UpdateStudentAsync(int id, Student student)
        {
            var existing = await db.Students.FindAsync(id);
            if (existing is null) return false;

            existing.FullName = student.FullName;
            existing.Email = student.Email;

            await SaveAsync();
            return true;
        }

        public async Task<bool> DeleteStudentAsync(int id)
        {
            var student = await db.Students.FindAsync(id);
            if (student is null) return false;

            db.Students.Remove(student);
            await db.SaveChangesAsync();
            return true;
        }

        private async Task SaveAsync()
        {
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException
            { SqlState: PostgresErrorCodes.UniqueViolation })
            {
                throw new ConflictException("A student with this email already exists.");
            }
        }
    }
}