using DalilAcademy.Core.DTOs;
using DalilAcademy.Core.Entites;
using DalilAcademy.Core.Repository;
using DalilAcademy.Core.Security;
using DalilAcademy.Core.Service;
using FluentValidation;
using static DalilAcademy.Core.DTOs.Auth;

namespace DalilAcademy.Infra.Service
{
    public class AuthService(
        IUserRepository users,
        IPasswordHasher hasher,
        ITokenService tokens,
        IValidator<RegisterRequestDto> registerValidator) : IAuthService
    {
        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto dto)
        {
            var email = dto.Email.Trim().ToLowerInvariant();

            var user = await users.GetByEmailAsync(email);
            if (user is null || !hasher.Verify(dto.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid email or password");

            var (token, expiresAt) = tokens.CreateToken(user);
            return new LoginResponseDto(token, expiresAt);
        }

        public async Task<LoginResponseDto> RegisterAsync(RegisterRequestDto dto)
        {
            await registerValidator.ValidateAndThrowAsync(dto);

            var user = new User
            {
                Email = dto.Email.Trim().ToLowerInvariant(),
                PasswordHash = hasher.Hash(dto.Password),
                Role = Roles.Student          // ثابت، ما بينقرأ من الـ Body
            };

            await users.RegisterStudentAsync(user, dto.FullName);

            var (token, expiresAt) = tokens.CreateToken(user);
            return new LoginResponseDto(token, expiresAt);
        }
    }
}