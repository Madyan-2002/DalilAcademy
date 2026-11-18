using DalilAcademy.Core.Entites;
using DalilAcademy.Core.Exceptions;
using DalilAcademy.Core.Repository;
using DalilAcademy.Infra.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace DalilAcademy.Infra.Repository
{
    public class EnrollmentRepository(AppDbContext db) : IEnrollmentRepository
    {
        public async Task<List<Enrollment>> GetAllAsync()
            => await db.Enrollments
                .AsNoTracking()
                .Include(e => e.Student)
                .Include(e => e.Course)
                .OrderBy(e => e.Id)
                .ToListAsync();

        public async Task<Enrollment?> GetByIdAsync(int id)
            => await db.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .FirstOrDefaultAsync(e => e.Id == id);

        public async Task<Enrollment> CreateAsync(Enrollment enrollment)
        {
            db.Enrollments.Add(enrollment);
            await SaveAsync();
            return enrollment;
        }

        public async Task UpdateAsync(Enrollment enrollment)
            => await SaveAsync();

        public async Task DeleteAsync(Enrollment enrollment)
        {
            db.Enrollments.Remove(enrollment);
            await SaveAsync();
        }

        public async Task<List<Student>> GetPaidStudentsByCourseAsync(int courseId)
            => await db.Enrollments
                .AsNoTracking()
                .Where(e => e.CourseId == courseId && e.Status == EnrollmentStatus.Paid)
                .Select(e => e.Student)
                .OrderBy(s => s.Id)
                .ToListAsync();

        private async Task SaveAsync()
        {
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException
            { SqlState: PostgresErrorCodes.UniqueViolation })
            {
                throw new ConflictException("The student is already enrolled in this course.");
            }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException
            { SqlState: PostgresErrorCodes.ForeignKeyViolation })
            {
                throw new ConflictException("This enrollment has payment records and cannot be deleted.");
            }
        }
    }
}