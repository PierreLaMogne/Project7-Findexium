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

        public async Task<UserDto> CreateUserAsync(CreateUserDto dto)
        {
            var user = new User
            {
                UserName = dto.UserName,
                FullName = dto.FullName
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                throw new Exception($"Unable to create the new User: {string.Join(", ", result.Errors.Select(e => e.Description))}");

            var roleExists = await _roleManager.RoleExistsAsync(dto.Role);
            if (!roleExists)
            {
                var roleResult = await _roleManager.CreateAsync(new IdentityRole(dto.Role));
                if (!roleResult.Succeeded)
                    throw new Exception($"Unable to create the new Role: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
            }
            
            var addToRoleResult = await _userManager.AddToRoleAsync(user, dto.Role);
            if (!addToRoleResult.Succeeded)
                throw new Exception($"Unable to add the mentionned Role to the new User: {string.Join(", ", addToRoleResult.Errors.Select(e => e.Description))}");

            var newUser = await GetUserByIdAsync(user.Id);
            return newUser!;
        }
        
        public async Task<UserDto> UpdateUserAsync(string id, UpdateUserDto dto)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                throw new Exception("The Id mentioned does not exist.");
            if (user.Id != dto.Id)
                throw new Exception("The Id focused and the Id mentioned are different.");

            user.UserName = dto.UserName;
            user.FullName = dto.FullName;
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                throw new Exception($"Unable to update the User: {string.Join(", ", result.Errors.Select(e => e.Description))}");

            // Update role if changed
            var currentRoles = await _userManager.GetRolesAsync(user);
            if (!currentRoles.Contains(dto.Role))
            {
                var roleExists = await _roleManager.RoleExistsAsync(dto.Role);
                if (!roleExists)
                {
                    var roleResult = await _roleManager.CreateAsync(new IdentityRole(dto.Role));
                    if (!roleResult.Succeeded)
                        throw new Exception($"Unable to create the Role mentionned: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
                }

                var removeFromRolesResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!removeFromRolesResult.Succeeded)
                    throw new Exception($"Unable to remove the existing Role: {string.Join(", ", removeFromRolesResult.Errors.Select(e => e.Description))}");

                var addToRoleResult = await _userManager.AddToRoleAsync(user, dto.Role);
                if (!addToRoleResult.Succeeded)
                    throw new Exception($"Unable to add the new Role: {string.Join(", ", addToRoleResult.Errors.Select(e => e.Description))}");
            }

            var updatedUser = await GetUserByIdAsync(user.Id);
            return updatedUser!;
        }
        
        public async Task<bool> DeleteUserAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return false;

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                throw new Exception($"Unable to delete the User: {string.Join(", ", result.Errors.Select(e => e.Description))}");

            return true;
        }
    }
}