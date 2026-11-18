namespace DalilAcademy.Core.DTOs
{
    public class Auth
    {
        public record LoginRequestDto(string Email, string Password);
        public record LoginResponseDto(string Token, DateTime ExpiresAt);
        public record RegisterRequestDto(string FullName, string Email, string Password);
    }
}
