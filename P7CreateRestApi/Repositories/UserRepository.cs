using FindexiumAPI.Common;
using FindexiumAPI.Domain;
using FindexiumAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FindexiumAPI.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserRepository(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            return users.Select(u => new UserDto
            {
                Id = u.Id,
                UserName = u.UserName,
                FullName = u.FullName,
                Role = u.Role
            }).ToList();
        }
        
        public async Task<UserDto?> GetUserByIdAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return null;

            return new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                FullName = user.FullName,
                Role = user.Role
            };
        }

        public async Task<UserDto?> GetUserByUserNameAsync(string userName)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == userName);
            if (user == null)
                return null;
            return new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                FullName = user.FullName,
                Role = user.Role
            };
        }

        public async Task<Result<UserDto>> CreateUserAsync(CreateUserDto dto)
        {
            if (dto.Password != dto.ConfirmPassword)
                return Result<UserDto>.Fail("Passwords do not match.");

            if (await _userManager.Users.AnyAsync(u => u.UserName == dto.UserName))
                return Result<UserDto>.Fail("The UserName mentioned already exists.");

            var user = new User
            {
                UserName = dto.UserName,
                FullName = dto.FullName
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                return Result<UserDto>.Fail($"Unable to create the User: {string.Join(", ", result.Errors.Select(e => e.Description))}");

            var roleExists = await _roleManager.RoleExistsAsync(dto.Role);
            if (!roleExists)
            {
                var roleResult = await _roleManager.CreateAsync(new IdentityRole(dto.Role));
                if (!roleResult.Succeeded)
                    return Result<UserDto>.Fail($"Unable to create the new Role: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
            }
            
            var addToRoleResult = await _userManager.AddToRoleAsync(user, dto.Role);
            if (!addToRoleResult.Succeeded)
                return Result<UserDto>.Fail($"Unable to add the mentionned Role to the new User: {string.Join(", ", addToRoleResult.Errors.Select(e => e.Description))}");

            var newUser = await GetUserByIdAsync(user.Id);
            return Result<UserDto>.Ok(newUser!);
        }
        
        public async Task<Result<UserDto>> UpdateUserAsync(string id, UserDto dto)
        {
            if (id != dto.Id)
                return Result<UserDto>.Fail("The Id focused and the Id mentioned are different.");

            if (await _userManager.Users.AnyAsync(u => u.UserName == dto.UserName && u.Id != id))
                return Result<UserDto>.Fail("The UserName mentioned already exists.");

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return Result<UserDto>.Fail("The Id mentioned does not exist.");
            if (user.Id != dto.Id)
                return Result<UserDto>.Fail("The Id focused and the Id mentioned are different.");

            user.UserName = dto.UserName;
            user.FullName = dto.FullName;
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                return Result<UserDto>.Fail($"Unable to update the User: {string.Join(", ", result.Errors.Select(e => e.Description))}");

            // Update role if changed
            var currentRoles = await _userManager.GetRolesAsync(user);
            if (!currentRoles.Contains(dto.Role))
            {
                var roleExists = await _roleManager.RoleExistsAsync(dto.Role);
                if (!roleExists)
                {
                    var roleResult = await _roleManager.CreateAsync(new IdentityRole(dto.Role));
                    if (!roleResult.Succeeded)
                        return Result<UserDto>.Fail($"Unable to create the Role mentionned: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
                }

                var removeFromRolesResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!removeFromRolesResult.Succeeded)
                    return Result<UserDto>.Fail($"Unable to remove the existing Role: {string.Join(", ", removeFromRolesResult.Errors.Select(e => e.Description))}");

                var addToRoleResult = await _userManager.AddToRoleAsync(user, dto.Role);
                if (!addToRoleResult.Succeeded)
                    return Result<UserDto>.Fail($"Unable to add the new Role: {string.Join(", ", addToRoleResult.Errors.Select(e => e.Description))}");
            }

            var updatedUser = await GetUserByIdAsync(user.Id);
            return Result<UserDto>.Ok(updatedUser!);
        }
        
        public async Task<Result<bool>> DeleteUserAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return Result<bool>.Fail("The Id mentioned does not exist.");

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                return Result<bool>.Fail($"Unable to delete the User: {string.Join(", ", result.Errors.Select(e => e.Description))}");

            return Result<bool>.Ok(true);
        }
    }
}