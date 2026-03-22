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
        [InlineData("testuser", "Password123!", true)]
        [InlineData("wronguser", "Password123!", false)]
        [InlineData("testuser", "WrongPassword!", false)]
        public async Task Authenticate_ShouldReturnsExpected(string userName, string password, bool shouldSucceed)
        {
            // Arrange
            using var scope = AuthServiceTestHelper.CreateCleanScope();
            var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();

            if (shouldSucceed) // Define the user only if the test should succeed
            {
                await authService.Register(new RegisterDto
                {
                    UserName = "testuser",
                    FullName = "Test User",
                    Password = "Password123!",
                    ConfirmPassword = "Password123!"
                });
            }

            // Act
            var result = await authService.Authenticate(new LoginDto
            {
                UserName = userName,
                Password = password
            });

            // Assert
            Assert.NotNull(result);
            Assert.Equal(shouldSucceed, result.IsSuccess);

            if (shouldSucceed) // Verify token only if authentication should succeed
            {
                Assert.NotNull(result.Data);
                Assert.NotEmpty(result.Data);
            }
            else // Verify error message and code only if authentication should fail
            {
                Assert.Null(result.Data);
                Assert.Equal("Invalid username or password.", result.ErrorMessage);
                Assert.Equal("400", result.Code);
            }
        }

        [Theory]
        [InlineData("newuser", "New User", "Password123!", "Password123!", true)]
        [InlineData("newuser", "New User", "Password123!", "WrongConfirm!", false)]
        [InlineData("alreadyexistsuser", "Already Exists User", "Password123!", "Password123!", false)]
        public async Task Register_ShouldReturnsExpected(
            string userName, string fullName, string password, string confirmPassword, bool shouldSucceed)
        {
            // Arrange
            using var scope = AuthServiceTestHelper.CreateCleanScope();
            var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();

            
            if (userName == "alreadyexistsuser") // Pre-create the user for the duplicate username test case
            {
                await authService.Register(new RegisterDto
                {
                    UserName = "alreadyexistsuser",
                    FullName = "Already Exists User",
                    Password = "Password123!",
                    ConfirmPassword = "Password123!"
                });
            }

            // Act
            var result = await authService.Register(new RegisterDto
            {
                UserName = userName,
                FullName = fullName,
                Password = password,
                ConfirmPassword = confirmPassword
            });

            // Assert
            Assert.NotNull(result);
            Assert.Equal(shouldSucceed, result.IsSuccess);

            if (shouldSucceed) // Verify token only if registration should succeed
            {
                Assert.NotNull(result.Data);
                Assert.NotEmpty(result.Data);
            }
            else // Verify error message and code only if registration should fail
            {
                Assert.Null(result.Data);
                if (password != confirmPassword)
                {
                    Assert.Equal("Passwords do not match.", result.ErrorMessage);
                    Assert.Equal("400", result.Code);
                }
                else
                {
                    Assert.Equal("Username already exists.", result.ErrorMessage);
                    Assert.Equal("409", result.Code);
                }
            }
        }

        [Theory]
        [InlineData("testuser", "Password123!", "NewPassword123!", "NewPassword123!", true)]
        [InlineData("testuser", "WrongCurrent!", "NewPassword123!", "NewPassword123!", false)]
        [InlineData("testuser", "Password123!", "NewPassword123!", "WrongConfirm!", false)]
        public async Task ChangePassword_ShouldReturnsExpected(
            string userName, string currentPassword, string newPassword, string confirmNewPassword, bool shouldSucceed)
        {
            // Arrange
            using var scope = AuthServiceTestHelper.CreateCleanScope();
            var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            // Create a user to change password for
            await authService.Register(new RegisterDto
            {
                UserName = "testuser",
                FullName = "Test User",
                Password = "Password123!",
                ConfirmPassword = "Password123!"
            });

            // Retrieve the user to get the ID
            var user = await userManager.FindByNameAsync("testuser");
            Assert.NotNull(user);
            Assert.NotNull(user.Id);

            // Act
            var result = await authService.ChangePassword(new ChangePasswordDto
            {
                Id = user.Id,
                CurrentPassword = currentPassword,
                NewPassword = newPassword,
                ConfirmNewPassword = confirmNewPassword
            });

            // Assert
            Assert.NotNull(result);
            Assert.Equal(shouldSucceed, result.IsSuccess);

            if (shouldSucceed) // Verify success and that the password was actually changed
            {
                Assert.NotNull(result.Data);
                Assert.True(await userManager.CheckPasswordAsync(user, newPassword));
            }
            else // Verify error message only if password change should fail
            {
                Assert.Null(result.Data);
                if (currentPassword != "Password123!")
                    Assert.Equal("Current password is incorrect.", result.ErrorMessage);
                else
                    Assert.Equal("New passwords do not match.", result.ErrorMessage);
            }
        }

        // This test ensures that the test helper is correctly setting up the UserManager and that we can create a user in the test database
        [Fact]
        public async Task TestHelper_Configuration_IsValid()
        {
            using var scope = AuthServiceTestHelper.CreateCleanScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            var user = new User { UserName = "diag", FullName = "Diagnostic" };
            var result = await userManager.CreateAsync(user, "Password123!");
            Assert.True(result.Succeeded);
            Assert.NotNull(user.Id);
        }
    }
}
