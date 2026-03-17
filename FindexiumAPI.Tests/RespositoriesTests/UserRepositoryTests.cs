using FindexiumAPI.Common;
using FindexiumAPI.Data;
using FindexiumAPI.Domain;
using FindexiumAPI.Models;
using FindexiumAPI.Repositories;
using FindexiumAPI.Tests.RespositoriesTests;
using FindexiumAPI.Tests.TestData;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using System.Reflection;
using System.Runtime.InteropServices;
using Xunit;

namespace FindexiumAPI.Tests.RespositoriesTests
{
    public class UserRepositoryTests : IDisposable
    {
        private readonly LocalDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserRepository _userRepository;

        public UserRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<LocalDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new LocalDbContext(options);

            var userStore = new UserStore<User, IdentityRole, LocalDbContext>(_context);
            _userManager = new UserManager<User>(
                userStore, null, new PasswordHasher<User>(),
                Enumerable.Empty<IUserValidator<User>>(),
                Enumerable.Empty<IPasswordValidator<User>>(),
                null, null, null, null);

            var roleStore = new RoleStore<IdentityRole, LocalDbContext>(_context);
            _roleManager = new RoleManager<IdentityRole>(
                roleStore, null, null, null, null);

            _context.Database.EnsureCreated();
            
            _roleManager.CreateAsync(new IdentityRole("Admin")).Wait();
            _roleManager.CreateAsync(new IdentityRole("User")).Wait();

            _userRepository = new UserRepository(_userManager, _roleManager);
        }



        // Testing GetAllAsync
        [Theory]
        [MemberData(nameof(UserTestData.GetUsersScenarios), MemberType = typeof(UserTestData))]
        public async Task GetAllAsync_ShouldReturnExpectedDtos(List<User> testData)
        {
            // Arrange
            _context.Database.EnsureCreated();
            await _context.Users.AddRangeAsync(testData);
            await _context.SaveChangesAsync();

            // Act
            var result = await _userRepository.GetAllUsersAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(testData.Count);
            result.Should().BeEquivalentTo(
                testData,
                options => options
                    .ExcludingMissingMembers()
            );
        }


        // Testing GetByIdAsync
        [Theory]
        [MemberData(nameof(UserTestData.GetUsersScenarios), MemberType = typeof(UserTestData))]
        public async Task GetByIdAsync_ShouldReturnExpectedDto(List<User> testData)
        {
            // Arrange
            _context.Database.EnsureCreated();
            await _context.Users.AddRangeAsync(testData);
            await _context.SaveChangesAsync();

            if (testData.Count == 0)
            {
                return; // Skip the test if there's no data to test
            }

            // Act & Assert
            foreach (var user in testData)
            {
                var result = await _userRepository.GetUserByIdAsync(user.Id);
                result.Should().NotBeNull();
                result.Should().BeEquivalentTo(
                    user,
                    options => options
                        .ExcludingMissingMembers()
                );
            }

        }

        [Theory]
        [MemberData(nameof(UserTestData.GetUsersScenarios), MemberType = typeof(UserTestData))]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound(List<User> testData)
        {
            // Arrange
            _context.Database.EnsureCreated();
            await _context.Users.AddRangeAsync(testData);
            await _context.SaveChangesAsync();

            // Act
            var result = await _userRepository.GetUserByIdAsync(Guid.Empty.ToString()); // Use an ID that doesn't exist

            // Assert
            result.Should().BeNull();
        }


        // Testing CreateAsync
        [Theory]
        [MemberData(nameof(UserTestData.GetCreateUsersScenarios), MemberType = typeof(UserTestData))]
        public async Task CreateAsync_ShouldReturnExpectedResult(CreateUserDto testDto)
        {

            // Arrange
            _context.Database.EnsureCreated();
            var existingUser = new CreateUserDto
            {
                UserName = "abc",
                FullName = "Abc",
                Role = "Admin",
                Password = "Bonjour123!",
                ConfirmPassword = "Bonjour123!"
            };
            await _userRepository.CreateUserAsync(existingUser);
            await _context.SaveChangesAsync();

            // Act
            var result = await _userRepository.CreateUserAsync(testDto);
            await _context.SaveChangesAsync();

            // Assert
            var dbCount = await _context.Users.CountAsync();
            result.Should().NotBeNull();
            if (testDto.UserName == "abc")
            {
                result.Code.Should().Be("409");
                result.ErrorMessage.Should().Be("The UserName mentioned already exists.");
                dbCount.Should().Be(1);
            }

            if (testDto.UserName == "pqr")
            {
                result.Code.Should().Be("400");
                result.ErrorMessage.Should().Be("Passwords do not match.");
                _context.Users.FirstOrDefault(u => u.UserName == "pqr").Should().BeNull();
                dbCount.Should().Be(1);
            }

            if (testDto.UserName == "def")
            {
                var createdUser = _context.Users.FirstOrDefault(u => u.UserName == testDto.UserName);
                createdUser.Should().NotBeNull();
                createdUser.UserName.Should().Be(testDto.UserName);
                createdUser.Role.Should().Be("Admin");
                createdUser.Should().BeEquivalentTo(
                    testDto,
                    options => options
                        .ExcludingMissingMembers()
                );
                dbCount.Should().Be(2);
            }

            if (testDto.UserName == "ghi" || testDto.UserName == "jkl" || testDto.UserName == "mno")
            {
                var createdUser = _context.Users.FirstOrDefault(u => u.UserName == testDto.UserName);
                createdUser.Should().NotBeNull();
                createdUser.UserName.Should().Be(testDto.UserName);
                createdUser.Role.Should().Be("User");
                createdUser.Should().BeEquivalentTo(
                    testDto,
                    options => options
                        .ExcludingMissingMembers()
                        .Excluding(u => u.Role)
                );
                dbCount.Should().Be(2);
            }
        }


        // Testing UpdateAsync
        [Theory]
        [MemberData(nameof(UserTestData.GetUpdateUserScenariosWithData), MemberType = typeof(UserTestData))]
        public async Task UpdateAsync_ShouldModifyExistingUserWhenExpected(List<User> testData, UserDto testDto)
        {
            // Arrange
            _context.Database.EnsureCreated();
            await _context.Users.AddRangeAsync(testData);
            await _context.SaveChangesAsync();

            var existingUser = _context.Users.FirstOrDefault(u => u.Id == testDto.Id);

            // Act
            var result = await _userRepository.UpdateUserAsync(existingUser!.Id, testDto);

            // Assert
            var dbCount = await _context.Users.CountAsync();
            dbCount.Should().Be(4);
            result.Should().NotBeNull();
            if (testDto.UserName != "alreadyExists")
            {
                result.Data.Should().NotBeNull();
                result.Data.Id.Should().Be(existingUser.Id);
                result.Data.Should().BeEquivalentTo(
                    testDto,
                    options => options
                        .ExcludingMissingMembers()
                        .Excluding(e => e.Role)
                );
            }

            if (testDto.UserName == "alreadyExists")
            {
                result.Code.Should().Be("409");
                result.ErrorMessage.Should().Be("The UserName mentioned already exists.");
            }

            if (testDto.UserName == "abc_updated")
                result.Data.Role.Should().Be("User");

            if (testDto.UserName == "def_updated")
                result.Data.Role.Should().Be("User");

            if (testDto.UserName == "ghi_updated")
                result.Data.Role.Should().Be("Admin");
        }

        [Theory]
        [MemberData(nameof(UserTestData.GetUsersToUpdateScenarios), MemberType = typeof(UserTestData))]
        public async Task UpdateAsync_ShouldReturnFalse_WhenNotFound(List<User> testData)
        {
            // Arrange
            _context.Database.EnsureCreated();
            await _context.Users.AddRangeAsync(testData);
            await _context.SaveChangesAsync();

            var updateDto = new UserDto {
                Id = Guid.Empty.ToString(),
                UserName = "abc",
                FullName = "Abc",
                Role = "Admin"
            };

            // Act
            var result = await _userRepository.UpdateUserAsync(updateDto.Id, updateDto); // Use an ID that doesn't exist

            // Assert
            result.ErrorMessage.Should().Be("The Id mentioned can't be found.");
            result.Code.Should().Be("404");
        }


        // Testing DeleteAsync
        [Theory]
        [MemberData(nameof(UserTestData.GetUsersScenarios), MemberType = typeof(UserTestData))]
        public async Task DeleteAsync_ShouldRemoveUser(List<User> testData)
        {
            // Arrange
            _context.Database.EnsureCreated();
            await _context.Users.AddRangeAsync(testData);
            await _context.SaveChangesAsync();

            if (testData.Count == 0)
            {
                return; // Skip the test if there's no data to test
            }

            var existingUser = testData.First();

            // Act
            var result = await _userRepository.DeleteUserAsync(existingUser.Id);

            // Assert
            result.Data.Should().Be(true);
            var deletedUser = await _context.Users.FindAsync(existingUser.Id);
            deletedUser.Should().BeNull();
        }

        [Theory]
        [MemberData(nameof(UserTestData.GetUsersScenarios), MemberType = typeof(UserTestData))]
        public async Task DeleteAsync_ShouldReturnFalse_WhenNotFound(List<User> testData)
        {
            // Arrange
            _context.Database.EnsureCreated();
            await _context.Users.AddRangeAsync(testData);
            await _context.SaveChangesAsync();

            // Act
            var result = await _userRepository.DeleteUserAsync(Guid.Empty.ToString()); // Use an ID that doesn't exist

            // Assert
            result.Data.Should().Be(false);
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}