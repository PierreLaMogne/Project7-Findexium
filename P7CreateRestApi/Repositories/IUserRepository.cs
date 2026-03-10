using FindexiumAPI.Common;
using FindexiumAPI.Models;

namespace FindexiumAPI.Repositories
{
    public interface IUserRepository
    {
        Task<List<UserDto>> GetAllUsersAsync();
        Task<UserDto?> GetUserByIdAsync(string id);
        Task<UserDto?> GetUserByUserNameAsync(string userName);
        Task<Result<UserDto>> CreateUserAsync(CreateUserDto dto);
        Task<Result<UserDto>> UpdateUserAsync(string id, UserDto dto);
        Task<Result<bool>> DeleteUserAsync(string id);
    }
}
