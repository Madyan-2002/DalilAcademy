using DalilAcademy.Core.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static DalilAcademy.Core.DTOs.Auth;

namespace DalilAcademy.API.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        // POST /api/auth/login
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login(LoginRequestDto dto)
            => Ok(await authService.LoginAsync(dto));

        // POST /api/auth/register
        [HttpPost("register")]
        public async Task<ActionResult<LoginResponseDto>> Register(RegisterRequestDto dto)
            => StatusCode(StatusCodes.Status201Created, await authService.RegisterAsync(dto));
    }
}