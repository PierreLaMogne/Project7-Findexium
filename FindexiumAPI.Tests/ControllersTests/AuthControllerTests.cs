using FindexiumAPI.Models;
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

            // Creating a user with the same username to trigger the conflict
            await AuthControllerTestHelper.RegisterUserAsync(scope, "existinguser");

            var dto = new RegisterDto
            {
                UserName = "existinguser",
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
                ConfirmPassword = "DifferentPassword!"
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

            // Creating a user to authenticate
            await AuthControllerTestHelper.RegisterUserAsync(scope, "testuser");

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

            // Creating a user and authenticating as that user
            var user = await AuthControllerTestHelper.RegisterUserAsync(scope, "testuser");
            var userId = user.Id;
            controller.ControllerContext = AuthControllerTestHelper.CreateAuthenticatedContext(userId, "User");

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
        public async Task ChangePassword_ShouldReturnUnauthorized_WhenUserIsNotOwner()
        {
            // Arrange
            var (controller, scope) = AuthControllerTestHelper.CreateControllerScope();

            // Creating two users and authenticating as the first user
            await AuthControllerTestHelper.RegisterUserAsync(scope, "user1");
            await AuthControllerTestHelper.RegisterUserAsync(scope, "user2");
            controller.ControllerContext = AuthControllerTestHelper.CreateAuthenticatedContext("user1-id", "User");

            var dto = new ChangePasswordDto
            {
                Id = "user2-id",
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

            // Authenticate as a user that does not exist
            controller.ControllerContext = AuthControllerTestHelper.CreateAuthenticatedContext("nonexistent-user-id", "User");

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

            // Creating a user and authenticating as that user
            var user = await AuthControllerTestHelper.RegisterUserAsync(scope, "testuser");
            var userId = user.Id;
            controller.ControllerContext = AuthControllerTestHelper.CreateAuthenticatedContext(userId, "User");

            var dto = new ChangePasswordDto
            {
                Id = userId,
                CurrentPassword = "WrongCurrentPassword!",
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
