using FindexiumAPI.Data;
using FindexiumAPI.Domain;
using FindexiumAPI.Models;
using FindexiumAPI.Repositories;
using FindexiumAPI.Tests.TestData;
using FindexiumAPI.Tests.TestUtilities;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FindexiumAPI.Tests.RepositoriesTests
{
    public class UserRepositoryTests : IClassFixture<LocalDbFixture>, IDisposable
    {
        private readonly LocalDbFixture _fixture;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserRepository _userRepository;

        public UserRepositoryTests(LocalDbFixture fixture)
        {
            _fixture = fixture;

            // Setting up UserManager and RoleManager with the in-memory database context
            var userStore = new UserStore<User, IdentityRole, LocalDbContext>(_fixture.Context);
            _userManager = new UserManager<User>(
                userStore, null, new PasswordHasher<User>(),
                Enumerable.Empty<IUserValidator<User>>(),
                Enumerable.Empty<IPasswordValidator<User>>(),
                null, null, null, null);

            var roleStore = new RoleStore<IdentityRole, LocalDbContext>(_fixture.Context);
            _roleManager = new RoleManager<IdentityRole>(roleStore, null, null, null, null);

            _userRepository = new UserRepository(_userManager, _roleManager);
            InitializeRoles().Wait();
            ClearUsers();
        }

        private async Task InitializeRoles()
        {
            if (!await _roleManager.RoleExistsAsync("Admin"))
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
            if (!await _roleManager.RoleExistsAsync("User"))
                await _roleManager.CreateAsync(new IdentityRole("User"));
        }

        private void ClearUsers()
        {
            _fixture.Context.Users.RemoveRange(_fixture.Context.Users);
            _fixture.Context.SaveChanges();
        }

        private async Task SeedUsersAsync(List<User> users)
        {
            ClearUsers();
            await _fixture.Context.Users.AddRangeAsync(users);
            await _fixture.Context.SaveChangesAsync();
        }


        // Testing GetAllAsync
        [Theory]
        [MemberData(nameof(UserTestData.GetUsersScenarios), MemberType = typeof(UserTestData))]
        public async Task GetAllAsync_ShouldReturnExpectedDtos(List<User> testData)
        {
            // Arrange
            await SeedUsersAsync(testData);

            // Act
            var result = await _userRepository.GetAllUsersAsync();

            // Assert
            result.Should().HaveCount(testData.Count);
            if (testData.Count > 0)
            {
                result.Should().BeEquivalentTo(testData, options => options.ExcludingMissingMembers());
            }
        }


        // Testing GetByIdAsync
        [Theory]
        [MemberData(nameof(UserTestData.GetUsersScenarios), MemberType = typeof(UserTestData))]
        public async Task GetByIdAsync_ShouldReturnExpectedDto(List<User> testData)
        {
            // Arrange
            await SeedUsersAsync(testData);

            // Act & Assert
            foreach (var user in testData.Where(u => !string.IsNullOrEmpty(u.Id)))
            {
                var result = await _userRepository.GetUserByIdAsync(user.Id);
                result.Should().NotBeNull();
                result.Should().BeEquivalentTo(user, options => options.ExcludingMissingMembers());
            }
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound()
        {
            // Arrange
            await SeedUsersAsync(new List<User>());

            // Act
            var result = await _userRepository.GetUserByIdAsync(Guid.Empty.ToString());

            // Assert
            result.Should().BeNull();
        }


        // Testing CreateAsync
        [Theory]
        [MemberData(nameof(UserTestData.GetCreateUserDuplicateScenarios), MemberType = typeof(UserTestData))]
        public async Task CreateAsync_ShouldFail_WhenUserNameExists(CreateUserDto testDto)
        {
            // Arrange - Seed an existing user with the same UserName
            var existingUser = new CreateUserDto { UserName = "abc", FullName = "Abc", Role = "Admin", Password = "Bonjour123!", ConfirmPassword = "Bonjour123!" };
            await _userRepository.CreateUserAsync(existingUser);
            await _fixture.Context.SaveChangesAsync();

            // Act
            var result = await _userRepository.CreateUserAsync(testDto);

            // Assert
            result.Code.Should().Be("409");
            result.ErrorMessage.Should().Be("The UserName mentioned already exists.");
            (await _fixture.Context.Users.CountAsync(u => u.UserName == testDto.UserName)).Should().Be(1);
        }

        [Theory]
        [MemberData(nameof(UserTestData.GetCreateUserPasswordMismatchScenarios), MemberType = typeof(UserTestData))]
        public async Task CreateAsync_ShouldFail_WhenPasswordsMismatch(CreateUserDto testDto)
        {
            // Act
            var result = await _userRepository.CreateUserAsync(testDto);

            // Assert
            result.Code.Should().Be("400");
            result.ErrorMessage.Should().Be("Passwords do not match.");
        }

        [Theory]
        [MemberData(nameof(UserTestData.GetCreateUserValidScenarios), MemberType = typeof(UserTestData))]
        public async Task CreateAsync_ShouldCreateUser_WhenValid(CreateUserDto testDto)
        {
            // Act
            var result = await _userRepository.CreateUserAsync(testDto);
            await _fixture.Context.SaveChangesAsync();

            // Assert
            result.Data.Should().NotBeNull();
            var createdUser = await _fixture.Context.Users.FirstOrDefaultAsync(u => u.UserName == testDto.UserName);
            createdUser.Should().NotBeNull();
            createdUser.UserName.Should().Be(testDto.UserName);
        }


        // Testing UpdateAsync
        [Theory]
        [MemberData(nameof(UserTestData.GetUpdateUserScenariosWithData), MemberType = typeof(UserTestData))]
        public async Task UpdateAsync_ShouldModifyExistingUserWhenExpected(List<User> testData, UserDto testDto)
        {
            // Arrange - Seed users and ensure the one to update exists
            await SeedUsersAsync(testData);
            var existingUser = testData.First(u => u.Id == testDto.Id);

            // Act
            var result = await _userRepository.UpdateUserAsync(testDto.Id, testDto);
            await _fixture.Context.SaveChangesAsync();

            // Assert
            if (testDto.UserName != "alreadyExists")
            {
                result.Data.Should().NotBeNull();
                result.Data.Id.Should().Be(testDto.Id);
                result.Data.UserName.Should().Be(testDto.UserName);
            }
            else
            {
                result.Code.Should().Be("409");
                result.ErrorMessage.Should().Be("The UserName mentioned already exists.");
            }
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnNotFound_WhenUserNotExists()
        {
            // Arrange - Ensure no users exist
            var updateDto = new UserDto
            {
                Id = Guid.Empty.ToString(),
                UserName = "newuser",
                FullName = "New User",
                Role = "User"
            };

            // Act
            var result = await _userRepository.UpdateUserAsync(updateDto.Id, updateDto);

            // Assert
            result.Code.Should().Be("404");
            result.ErrorMessage.Should().Be("The Id mentioned can't be found.");
        }


        // Testing DeleteAsync
        [Theory]
        [MemberData(nameof(UserTestData.GetUsersScenarios), MemberType = typeof(UserTestData))]
        public async Task DeleteAsync_ShouldRemoveUser(List<User> testData)
        {
            // Arrange - Seed users and ensure the one to delete exists
            await SeedUsersAsync(testData);
            var existingUser = testData.FirstOrDefault(u => !string.IsNullOrEmpty(u.Id));
            if (existingUser == null) return;

            // Act
            var result = await _userRepository.DeleteUserAsync(existingUser.Id);
            await _fixture.Context.SaveChangesAsync();

            // Assert
            result.Data.Should().BeTrue();
            (await _fixture.Context.Users.FindAsync(existingUser.Id)).Should().BeNull();
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnFalse_WhenNotFound()
        {
            // Act
            var result = await _userRepository.DeleteUserAsync(Guid.Empty.ToString());

            // Assert
            result.Data.Should().BeFalse();
        }

        public void Dispose()
        {
            ClearUsers();
        }
    }
}
