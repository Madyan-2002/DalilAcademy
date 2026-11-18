using DalilAcademy.Core.DTOs;
using DalilAcademy.Core.Entites;
using DalilAcademy.Core.Exceptions;
using DalilAcademy.Core.Repository;
using DalilAcademy.Infra.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;
namespace DalilAcademy.Infra.Repository
{
    public class CourseRepository(AppDbContext db) : ICourseRepository
    {

        public async Task<PageResult<CourseResponseDto>> GetPageAsync(
    int page, int pageSize, decimal? minPrice = null, decimal? maxPrice = null)
        {
            var query = db.Courses.AsNoTracking();

            if (minPrice.HasValue) query = query.Where(c => c.Price >= minPrice.Value);
            if (maxPrice.HasValue) query = query.Where(c => c.Price <= maxPrice.Value);

            var total = await query.CountAsync();

            var items = await query
                .OrderBy(c => c.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CourseResponseDto(
                    c.Id, c.Title, c.Price, c.Instructor.FullName, c.ImageUrl))
                .ToListAsync();

            return new PageResult<CourseResponseDto>(items, page, pageSize, total);
        }

        public async Task<int> CountAsync() => await db.Courses.CountAsync();
        public async Task<Course?> GetByIdAsync(int id)
        {

            return await db.Courses.Include(c => c.Instructor).FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Course> CreateCourseAsync(Course course)
        {
            try
            {
                db.Courses.Add(course);
                await db.SaveChangesAsync();
                await db.Entry(course).Reference(c => c.Instructor).LoadAsync();
                return course;
            }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation})
            {
                throw new ConflictException("A course with the same name already exists.");
            }
        }

        public async Task UpdateCourseAsync(Course course)
        {
            try
            {
                db.Courses.Update(course);
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException
            { SqlState: PostgresErrorCodes.UniqueViolation })
            {
                throw new ConflictException("A course with the same title already exists.");
            }
        }
        public async Task<bool> DeleteCourseAsync(int id)
        {
            var course = await db.Courses.FindAsync(id);

            if (course == null) return false;

            db.Courses.Remove(course);
            await db.SaveChangesAsync();
            return true;
        }
    }
}
