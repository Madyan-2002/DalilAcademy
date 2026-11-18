using DalilAcademy.Core.Entites;

namespace DalilAcademy.Core.Repository
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task RegisterStudentAsync(User user, string fullName);
    }
}
