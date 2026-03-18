using FindexiumAPI.Domain;
using FindexiumAPI.Models;
using FindexiumAPI.Services;
using FindexiumAPI.Tests.ServicesTests;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FindexiumAPI.Tests.ServicesTests
{
    public class AuthServiceTests
    {
        [Theory]
        [InlineData("testuser", "Password123!")]
        [InlineData("wronguser", "Password123!")]
        [InlineData("testuser", "WrongPassword!")]
        public async Task Authenticate_ShouldReturnsExpected(string userName, string password)
        {
            using var scope = AuthServiceTestHelper.CreateCleanScope();
            var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();

            // Arrange
            var registerDto = new RegisterDto
            {
                UserName = "testuser",
                FullName = "Test User",
                Password = "Password123!",
                ConfirmPassword = "Password123!"
            };
            await authService.Register(registerDto);
            var loginDto = new LoginDto
            {
                UserName = userName,
                Password = password
            };

            // Act
            var result = await authService.Authenticate(loginDto);

            // Assert
            if (userName == "testuser" && password == "Password123!")
            {
                Assert.True(result.IsSuccess);
                Assert.NotNull(result.Data);
            }
            else
            {
                Assert.False(result.IsSuccess);
                Assert.Null(result.Data);
                Assert.Equal("Invalid username or password.", result.ErrorMessage);
                Assert.Equal("400", result.Code);
            }
        }

        [Theory]
        [InlineData("newuser", "New User", "Password123!", "Password123!")]
        [InlineData("alreadyexistsuser", "Already Exists User", "Password123!", "Password123!")]
        [InlineData("newuser", "New User", "Password123!", "WrongConfirm!")]
        public async Task Register_ShouldReturnsExpected(string userName, string fullName, string password, string confirmPassword)
        {
            using var scope = AuthServiceTestHelper.CreateCleanScope();
            var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();

            // Arrange
            if (userName == "alreadyexistsuser")
            {
                var registerDto = new RegisterDto
                {
                    UserName = "alreadyexistsuser",
                    FullName = "Already Exists User",
                    Password = "Password123!",
                    ConfirmPassword = "Password123!"
                };
                await authService.Register(registerDto);
            }
            var registerDtoToTest = new RegisterDto
            {
                UserName = userName,
                FullName = fullName,
                Password = password,
                ConfirmPassword = confirmPassword
            };

            // Act
            var result = await authService.Register(registerDtoToTest);

            // Assert
            if (userName == "newuser" && password == confirmPassword)
            {
                Assert.True(result.IsSuccess);
                Assert.NotNull(result.Data);
            }
            else if (userName == "alreadyexistsuser")
            {
                Assert.False(result.IsSuccess);
                Assert.Null(result.Data);
                Assert.Equal("Username already exists.", result.ErrorMessage);
                Assert.Equal("409", result.Code);
            }
            else
            {
                Assert.False(result.IsSuccess);
                Assert.Null(result.Data);
                Assert.Equal("Passwords do not match.", result.ErrorMessage);
                Assert.Equal("400", result.Code);
            }
        }

        [Theory]
        [InlineData("testuser", "Password123!", "NewPassword123!", "NewPassword123!")]
        [InlineData("nonexistentuser", "Password123!", "NewPassword123!", "NewPassword123!")]
        [InlineData("testuser", "Password123!", "NewPassword123!", "WrongConfirm!")]
        [InlineData("testuser", "WrongCurrentPassword!", "NewPassword123!", "NewPassword123!")]
        public async Task ChangePassword_ShouldReturnsExpected(string userName, string currentPassword, string newPassword, string confirmNewPassword)
        {
            using var scope = AuthServiceTestHelper.CreateCleanScope();
            var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            // Arrange
            var registerDto = new RegisterDto
            {
                UserName = "testuser",
                FullName = "Test User",
                Password = "Password123!",
                ConfirmPassword = "Password123!"
            };
            await authService.Register(registerDto);

            // Get the actual user ID
            var user = await userManager.FindByNameAsync(userName);
            var userId = user?.Id;

            var changePasswordDto = new ChangePasswordDto
            {
                Id = userId!,
                CurrentPassword = currentPassword,
                NewPassword = newPassword,
                ConfirmNewPassword = confirmNewPassword
            };

            // Act
            var result = await authService.ChangePassword(changePasswordDto);
            // Assert
            Assert.NotNull(result);
            if (userName == "testuser" && currentPassword == "Password123!" && newPassword == confirmNewPassword)
            {
                Assert.True(result.IsSuccess);
                Assert.NotNull(result.Data);
            }
            else if (userName == "nonexistentuser")
            {
                Assert.False(result.IsSuccess);
                Assert.Null(result.Data);
                Assert.Equal("User not found.", result.ErrorMessage);
                Assert.Equal("404", result.Code);
            }
            else if (newPassword != confirmNewPassword)
            {
                Assert.False(result.IsSuccess);
                Assert.Null(result.Data);
                Assert.Equal("New passwords do not match.", result.ErrorMessage);
                Assert.Equal("400", result.Code);
            }
            else
            {
                Assert.False(result.IsSuccess);
                Assert.Null(result.Data);
                Assert.Equal("Current password is incorrect.", result.ErrorMessage);
                Assert.Equal("400", result.Code);
            }
        }
    }
}