using FindexiumAPI.Common;
using FindexiumAPI.Models;

namespace FindexiumAPI.Services
{
    public interface IAuthService
    {
        Task<Result<string>> Register(CreateUserDto dto);
        Task<Result<string>> Authenticate(LoginDto dto);
        Task<Result<string>> ChangePassword(ChangePasswordDto dto);
    }
}
