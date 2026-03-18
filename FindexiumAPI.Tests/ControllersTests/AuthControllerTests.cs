using FindexiumAPI.Models;
using FindexiumAPI.Services;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FindexiumAPI.Tests.ControllersTests
{
    public class AuthControllerTests
    {
        [Fact]
        public async Task Register_ShouldReturnOk_WhenRegistrationIsSuccessful()
        {
            // Arrange
            var (controller, scope) = AuthControllerTestHelper.CreateControllerScope();
            var dto = new RegisterDto
            {
                UserName = "testuser",
                FullName = "Test User",
                Password = "Password123!",
                ConfirmPassword = "Password123!"
            };
            // Act
            var result = await controller.Register(dto);

            // Assert
            Assert.IsType<Microsoft.AspNetCore.Mvc.OkObjectResult>(result);
        }

        [Fact]
        public async Task Register_ShouldReturnConflict_WhenUsernameAlreadyExists()
        {
            // Arrange
            var (controller, scope) = AuthControllerTestHelper.CreateControllerScope();
            var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();

            // Creating a user with the same username to cause a conflict
            await authService.Register(new RegisterDto
            {
                UserName = "existinguser",
                FullName = "Existing User",
                Password = "Password123!",
                ConfirmPassword = "Password123!"
            });
            var dto = new RegisterDto
            {
                UserName = "existinguser", // Same username as the existing user
                FullName = "New User",
                Password = "Password123!",
                ConfirmPassword = "Password123!"
            };
            // Act
            var result = await controller.Register(dto);

            // Assert
            Assert.IsType<Microsoft.AspNetCore.Mvc.ConflictObjectResult>(result);
        }

        [Fact]
        public async Task Register_ShouldReturnBadRequest_WhenPasswordsDoNotMatch()
        {
            // Arrange
            var (controller, scope) = AuthControllerTestHelper.CreateControllerScope();
            var dto = new RegisterDto
            {
                UserName = "newuser",
                FullName = "New User",
                Password = "Password123!",
                ConfirmPassword = "DifferentPassword!" // Wrong confirm password
            };
            // Act
            var result = await controller.Register(dto);

            // Assert
            Assert.IsType<Microsoft.AspNetCore.Mvc.BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Login_ShouldReturnOk_WhenAuthenticationIsSuccessful()
        {
            // Arrange
            var (controller, scope) = AuthControllerTestHelper.CreateControllerScope();
            var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();

            // Creating a user for the test
            await authService.Register(new RegisterDto
            {
                UserName = "testuser",
                FullName = "Test User",
                Password = "Password123!",
                ConfirmPassword = "Password123!"
            });
            var dto = new LoginDto
            {
                UserName = "testuser",
                Password = "Password123!"
            };
            // Act
            var result = await controller.Login(dto);

            // Assert
            Assert.IsType<Microsoft.AspNetCore.Mvc.OkObjectResult>(result);
        }

        [Fact]
        public async Task Login_ShouldReturnBadRequest_WhenAuthenticationFails()
        {
            // Arrange
            var (controller, scope) = AuthControllerTestHelper.CreateControllerScope();
            var dto = new LoginDto
            {
                UserName = "nonexistentuser",
                Password = "WrongPassword!"
            };
            // Act
            var result = await controller.Login(dto);

            // Assert
            Assert.IsType<Microsoft.AspNetCore.Mvc.BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task ChangePassword_ShouldReturnOk_WhenPasswordChangeIsSuccessful()
        {
            // Arrange
            var (controller, scope) = AuthControllerTestHelper.CreateControllerScope();
            var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();

            // Creating a user for the test
            await authService.Register(new RegisterDto
            {
                UserName = "testuser",
                FullName = "Test User",
                Password = "Password123!",
                ConfirmPassword = "Password123!"
            });

            // Get the actual user ID of the created user
            var userManager = scope.ServiceProvider.GetRequiredService<Microsoft.AspNetCore.Identity.UserManager<FindexiumAPI.Domain.User>>();
            var user = await userManager.FindByNameAsync("testuser");
            var userId = user.Id;

            var context = AuthControllerTestHelper.CreateAuthenticatedContext(userId, "User");
            controller.ControllerContext = context;
            var dto = new ChangePasswordDto
            {
                Id = userId,
                CurrentPassword = "Password123!",
                NewPassword = "NewPassword123!",
                ConfirmNewPassword = "NewPassword123!"
            };
            // Act
            var result = await controller.ChangePassword(dto);

            // Assert
            Assert.IsType<Microsoft.AspNetCore.Mvc.OkObjectResult>(result);
        }

        [Fact]
        public async Task ChangePassword_ShouldReturnUnauthorized_WhenUserTriesToChangeAnotherUsersPassword()
        {
            // Arrange
            var (controller, scope) = AuthControllerTestHelper.CreateControllerScope();
            var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();

            // Creating two users for the test
            await authService.Register(new RegisterDto
            {
                UserName = "user1",
                FullName = "User One",
                Password = "Password123!",
                ConfirmPassword = "Password123!"
            });
            await authService.Register(new RegisterDto
            {
                UserName = "user2",
                FullName = "User Two",
                Password = "Password123!",
                ConfirmPassword = "Password123!"
            });

            // Simulate authentication as user1
            var context = AuthControllerTestHelper.CreateAuthenticatedContext("user1-id", "User");
            controller.ControllerContext = context;
            var dto = new ChangePasswordDto
            {
                Id = "user2-id", // Trying to change user2's password while authenticated as user1
                CurrentPassword = "Password123!",
                NewPassword = "NewPassword123!",
                ConfirmNewPassword = "NewPassword123!"
            };
            // Act
            var result = await controller.ChangePassword(dto);

            // Assert
            Assert.IsType<Microsoft.AspNetCore.Mvc.UnauthorizedObjectResult>(result);
        }

        [Fact]
        public async Task ChangePassword_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var (controller, scope) = AuthControllerTestHelper.CreateControllerScope();
            var context = AuthControllerTestHelper.CreateAuthenticatedContext("nonexistent-user-id", "User");
            controller.ControllerContext = context;
            var dto = new ChangePasswordDto
            {
                Id = "nonexistent-user-id",
                CurrentPassword = "Password123!",
                NewPassword = "NewPassword123!",
                ConfirmNewPassword = "NewPassword123!"
            };
            // Act
            var result = await controller.ChangePassword(dto);

            // Assert
            Assert.IsType<Microsoft.AspNetCore.Mvc.NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task ChangePassword_ShouldReturnBadRequest_WhenCurrentPasswordIsIncorrect()
        {
            // Arrange
            var (controller, scope) = AuthControllerTestHelper.CreateControllerScope();
            var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();

            // Creating a user for the test
            await authService.Register(new RegisterDto
            {
                UserName = "testuser",
                FullName = "Test User",
                Password = "Password123!",
                ConfirmPassword = "Password123!"
            });

            // Get the actual user ID of the created user
            var userManager = scope.ServiceProvider.GetRequiredService<Microsoft.AspNetCore.Identity.UserManager<FindexiumAPI.Domain.User>>();
            var user = await userManager.FindByNameAsync("testuser");
            var userId = user.Id;

            var context = AuthControllerTestHelper.CreateAuthenticatedContext(userId, "User");
            controller.ControllerContext = context;
            var dto = new ChangePasswordDto
            {
                Id = userId,
                CurrentPassword = "WrongCurrentPassword!", // Wrong current password
                NewPassword = "NewPassword123!",
                ConfirmNewPassword = "NewPassword123!"
            };
            // Act
            var result = await controller.ChangePassword(dto);

            // Assert
            Assert.IsType<Microsoft.AspNetCore.Mvc.BadRequestObjectResult>(result);
        }
    }
}