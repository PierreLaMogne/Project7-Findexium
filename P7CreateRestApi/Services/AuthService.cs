using FindexiumAPI.Common;
using FindexiumAPI.Domain;
using FindexiumAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FindexiumAPI.Services
{


    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly JwtSettings _JwtSettings;

        public AuthService(UserManager<User> userManager, IOptions<JwtSettings> JwtOptions)
        {
            _userManager = userManager;
            _JwtSettings = JwtOptions.Value;
        }

        public async Task<Result<string>> Authenticate(LoginDto dto)
        {
            var user = await _userManager.FindByNameAsync(dto.UserName);
            if (user == null)
                return Result<string>.Fail("Invalid username or password.", "400");

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (!isPasswordValid)
                return Result<string>.Fail("Invalid username or password.", "400");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = GenerateJwtToken(claims);
            return Result<string>.Ok(token);
        }

        public async Task<Result<string>> Register(RegisterDto dto)
        {
            var existingUser = await _userManager.FindByNameAsync(dto.UserName);
            if (existingUser != null)
                return Result<string>.Fail("Username already exists.", "409");

            if (dto.Password != dto.ConfirmPassword)
                return Result<string>.Fail("Passwords do not match.", "400");

            var user = new User
            {
                UserName = dto.UserName,
                FullName = dto.FullName,
                Role = "User"
            };

            await _userManager.CreateAsync(user);
            await _userManager.AddPasswordAsync(user, dto.Password);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = GenerateJwtToken(claims);
            return Result<string>.Ok(token);
        }

        public async Task<Result<string>> ChangePassword(ChangePasswordDto dto)
        {
            var user = await _userManager.FindByIdAsync(dto.Id);
            if (user == null)
                return Result<string>.Fail("User not found.", "404");

            var isCurrentPasswordValid = await _userManager.CheckPasswordAsync(user, dto.CurrentPassword);
            if (!isCurrentPasswordValid)
                return Result<string>.Fail("Current password is incorrect.", "400");

            if (dto.NewPassword != dto.ConfirmPassword)
                return Result<string>.Fail("New passwords do not match.", "400");

            var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
            if (!result.Succeeded)
                return Result<string>.Fail("Failed to change password.", "400");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = GenerateJwtToken(claims);
            return Result<string>.Ok(token);
        }

        private string GenerateJwtToken(List<Claim> claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_JwtSettings.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_JwtSettings.ExpirationInMinutes),
                Issuer = _JwtSettings.Issuer,
                Audience = _JwtSettings.Audience,
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
