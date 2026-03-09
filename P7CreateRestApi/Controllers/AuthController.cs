using FindexiumAPI.Models;
using FindexiumAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FindexiumAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var result = await _authService.Register(dto);
            if (!result.IsSuccess)
            {
                if (result.Code == "UserExists")
                    return Conflict(result.ErrorMessage);
                else
                    return BadRequest(result.ErrorMessage);
            }
            return Ok(result.Data);
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var result = await _authService.Authenticate(dto);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);
            return Ok(result.Data);
        }

        [HttpPost]
        [Authorize(Policy = "Users")]
        [Route("password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != dto.Id)
                    return Unauthorized("You can only change your own password.");

            var result = await _authService.ChangePassword(dto);
            if (!result.IsSuccess)
            {
                if (result.Code == "NotFound")
                    return NotFound(result.ErrorMessage);
                else
                    return BadRequest(result.ErrorMessage);
            }

            return Ok(result.Data);
        }
    }
}
