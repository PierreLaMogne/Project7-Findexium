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
        public Task<Result<string>> Authenticate(LoginDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<Result<string>> ChangePassword(ChangePasswordDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<Result<string>> Register(CreateUserDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
