using DalilAcademy.Core.Entites;
using DalilAcademy.Core.Exceptions;
using DalilAcademy.Core.Repository;
using DalilAcademy.Infra.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace DalilAcademy.Infra.Repository
{
    public class UserRepository(AppDbContext db) : IUserRepository
    {
        public Task<User?> GetByEmailAsync(string email)
            => db.Users.AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email);

        public async Task RegisterStudentAsync(User user, string fullName)
        {
            try
            {
                db.Users.Add(user);
                db.Students.Add(new Student { FullName = fullName, Email = user.Email, User = user });
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException
            { SqlState: PostgresErrorCodes.UniqueViolation })
            {
                throw new ConflictException("An account with this email already exists.");
            }
        }
    }
}