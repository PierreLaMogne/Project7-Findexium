using FindexiumAPI.Controllers;
using FindexiumAPI.Domain;
using FindexiumAPI.Models;
using FindexiumAPI.Repositories;
using FindexiumAPI.Common;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using System.Data;
using Xunit;

namespace FindexiumAPI.Tests.Controllers
{
    public class UserControllerTests
    {
        // Tests for GetUsers
        [Theory]
        [InlineData("Admin")]
        [InlineData("User")]
        [InlineData("Unauthenticated")]
        public async Task GetUsers_AsDifferentRoles_ReturnsExpected(string? role)
        {
            // Arrange
            var repo = Substitute.For<IUserRepository>();
            repo
                .GetAllUsersAsync()
                .Returns(new List<UserDto> { new UserDto { Id = 1.ToString() }, new UserDto { Id = 2.ToString() } });
            var controller = new UserController(repo);

            if (role == "Admin")
                controller.InitializeAdminUser();
            else if (role == "User")
                controller.InitializeRegularUser();
            else if (role == "Unauthenticated")
                controller.InitializeUnauthenticatedUser();

            // Act
            ActionResult<IEnumerable<UserDto>> result;

            // Simulate [Authorize(Policy = "Users")] behavior in unit tests
            if (!controller.IsAuthorizedForUsersPolicy())
                result = new ActionResult<IEnumerable<UserDto>>(new UnauthorizedResult());
            else
                result = await controller.GetUsers();

            // Assert
            if (role == "Unauthenticated")
                Assert.IsType<UnauthorizedResult>(result.Result);
            else
            {
                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var users = Assert.IsAssignableFrom<IEnumerable<UserDto>>(okResult.Value);
                Assert.Equal(2, users.Count());
            }
        }

        [Theory]
        [InlineData("Admin")]
        [InlineData("User")]
        [InlineData("Unauthenticated")]
        public async Task GetUsers_AsDifferentRoles_ReturnsNotFound(string? role)
        {
            // Arrange
            var repo = Substitute.For<IUserRepository>();
            repo
                .GetAllUsersAsync()
                .Returns(new List<UserDto>());
            var controller = new UserController(repo);

            if (role == "Admin")
                controller.InitializeAdminUser();
            else if (role == "User")
                controller.InitializeRegularUser();
            else if (role == "Unauthenticated")
                controller.InitializeUnauthenticatedUser();

            // Act
            ActionResult<IEnumerable<UserDto>> result;

            // Simulate [Authorize(Policy = "Users")] behavior in unit tests
            if (!controller.IsAuthorizedForUsersPolicy())
                result = new ActionResult<IEnumerable<UserDto>>(new UnauthorizedResult());
            else
                result = await controller.GetUsers();

            // Assert
            if (role == "Unauthenticated")
                Assert.IsType<UnauthorizedResult>(result.Result);
            else
            {
                var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
                Assert.Equal("No User found.", notFoundResult.Value);
            }
        }

        // Tests for GetUser
        [Theory]
        [InlineData("Admin")]
        [InlineData("User")]
        [InlineData("Unauthenticated")]
        public async Task GetUser_AsDifferentRoles_ReturnsExpected(string? role)
        {
            // Arrange
            var repo = Substitute.For<IUserRepository>();
            repo
                .GetUserByIdAsync(1.ToString())
                .Returns(new UserDto { Id = 1.ToString() });
            var controller = new UserController(repo);

            if (role == "Admin")
                controller.InitializeAdminUser();
            else if (role == "User")
                controller.InitializeRegularUser();
            else if (role == "Unauthenticated")
                controller.InitializeUnauthenticatedUser();

            // Act
            ActionResult<UserDto> result;

            // Simulate [Authorize(Policy = "Users")] behavior in unit tests
            if (!controller.IsAuthorizedForUsersPolicy())
                result = new ActionResult<UserDto>(new UnauthorizedResult());
            else
                result = await controller.GetUser(1.ToString());

            // Assert
            if (role == "Unauthenticated")
                Assert.IsType<UnauthorizedResult>(result.Result);
            else
            {
                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var user = Assert.IsAssignableFrom<UserDto>(okResult.Value);
                Assert.Equal(1.ToString(), user.Id);

            }
        }

        [Theory]
        [InlineData("Admin")]
        [InlineData("User")]
        [InlineData("Unauthenticated")]
        public async Task GetUser_AsDifferentRoles_ReturnsNotFound(string? role)
        {
            // Arrange
            var repo = Substitute.For<IUserRepository>();
            repo
                .GetUserByIdAsync(1.ToString())
                .Returns((UserDto?)null);
            var controller = new UserController(repo);

            if (role == "Admin")
                controller.InitializeAdminUser();
            else if (role == "User")
                controller.InitializeRegularUser();
            else if (role == "Unauthenticated")
                controller.InitializeUnauthenticatedUser();

            // Act
            ActionResult<UserDto> result;

            // Simulate [Authorize(Policy = "Users")] behavior in unit tests
            if (!controller.IsAuthorizedForUsersPolicy())
                result = new ActionResult<UserDto>(new UnauthorizedResult());
            else
                result = await controller.GetUser(1.ToString());

            // Assert
            if (role == "Unauthenticated")
                Assert.IsType<UnauthorizedResult>(result.Result);
            else
            {
                var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
                Assert.Equal("The Id mentioned does not exist.", notFoundResult.Value);
            }
        }

        // Tests for PostUser
        [Theory]
        [InlineData("Admin")]
        [InlineData("User")]
        [InlineData("Unauthenticated")]
        public async Task PostUser_AsDifferentRoles_ReturnsExcepted(string? role)
        {
            // Arrange
            var repo = Substitute.For<IUserRepository>();
            repo
                .CreateUserAsync(Arg.Any<CreateUserDto>())
                .Returns(callInfo =>
                {
                    var dto = callInfo.Arg<CreateUserDto>();
                    return Result<UserDto>.Ok(new UserDto 
                    { 
                        Id = 1.ToString(),
                        UserName = dto.UserName,
                        FullName = dto.FullName,
                        Role = dto.Role
                    });
                });
            var controller = new UserController(repo);

            if (role == "Admin")
                controller.InitializeAdminUser();
            else if (role == "User")
                controller.InitializeRegularUser();
            else if (role == "Unauthenticated")
                controller.InitializeUnauthenticatedUser();

            var newUser = new CreateUserDto()
            {
                UserName = "abc",
                FullName = "Abc",
                Role = "Admin",
                Password = "Bonjour123!",
                ConfirmPassword = "Bonjour123!"
            };

            // Act
            ActionResult<UserDto> result;

            // Simulate [Authorize(Roles = "Admin")] behavior in unit tests
            if (!controller.IsAuthorizedAsAdmin())
                result = new ActionResult<UserDto>(new UnauthorizedResult());
            else
                result = await controller.PostUser(newUser);

            // Assert
            if (role == "Admin")
            {
                var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
                var createdUser = Assert.IsAssignableFrom<UserDto>(createdAtActionResult.Value);
                Assert.Equal(1.ToString(), createdUser.Id);
                Assert.Equal(newUser.UserName, createdUser.UserName);
                Assert.Equal(newUser.FullName, createdUser.FullName);
                Assert.Equal(newUser.Role, createdUser.Role);
            }
            else
                Assert.IsType<UnauthorizedResult>(result.Result);
        }

        [Theory]
        [InlineData("Admin")]
        [InlineData("User")]
        [InlineData("Unauthenticated")]
        public async Task PostUser_AsDifferentRoles_WhenModelStateIsNotValid_ReturnsBadRequest(string? role)
        {
            // Arrange
            var repo = Substitute.For<IUserRepository>();
            var controller = new UserController(repo);

            if (role == "Admin")
                controller.InitializeAdminUser();
            else if (role == "User")
                controller.InitializeRegularUser();
            else if (role == "Unauthenticated")
                controller.InitializeUnauthenticatedUser();

            var invalidUser = new CreateUserDto { UserName = "", FullName = "", Role = "", Password = "", ConfirmPassword = "" };

            controller.ModelState.AddModelError("UserName", "UserName is required.");
            controller.ModelState.AddModelError("FullName", "FullName is required.");
            controller.ModelState.AddModelError("Role", "Role is required.");

            // Act
            ActionResult<UserDto> result;

            // Simulate [Authorize(Roles = "Admin")] behavior in unit tests
            if (!controller.IsAuthorizedAsAdmin())
                result = new ActionResult<UserDto>(new UnauthorizedResult());
            else
                result = await controller.PostUser(invalidUser);

            // Assert
            if (role == "Admin")
                Assert.IsType<BadRequestObjectResult>(result.Result);
            else
                Assert.IsType<UnauthorizedResult>(result.Result);
        }

        // Tests for PutUser
        [Theory]
        [InlineData("Admin")]
        [InlineData("User")]
        [InlineData("Unauthenticated")]
        public async Task PutUser_AsDifferentRoles_ReturnsExpected(string? role)
        {
            // Arrange
            var repo = Substitute.For<IUserRepository>();
            repo
                .GetUserByIdAsync(1.ToString())
                .Returns(new UserDto { Id = 1.ToString() });
            repo
                .UpdateUserAsync(1.ToString(), Arg.Any<UserDto>())
                .Returns(Result<UserDto>.Ok(new UserDto { Id = 1.ToString() }));
            var controller = new UserController(repo);

            if (role == "Admin")
                controller.InitializeAdminUser();
            else if (role == "User")
                controller.InitializeRegularUser();
            else if (role == "Unauthenticated")
                controller.InitializeUnauthenticatedUser();

            var updatedUser = new UserDto { Id = 1.ToString(), UserName = "def_updated", FullName = "Def Updated", Role = ""};

            // Act
            IActionResult result;

            // Simulate [Authorize(Roles = "Admin")] behavior in unit tests
            if (!controller.IsAuthorizedAsAdmin())
                result = new UnauthorizedResult();
            else
                result = await controller.PutUser(1.ToString(), updatedUser);

            // Assert
            if (role == "Admin")
                Assert.IsType<OkObjectResult>(result);
            else
                Assert.IsType<UnauthorizedResult>(result);
        }

        [Theory]
        [InlineData("Admin")]
        [InlineData("User")]
        [InlineData("Unauthenticated")]
        public async Task PutUser_AsDifferentRoles_ReturnsNotFound(string? role)
        {
            // Arrange
            var repo = Substitute.For<IUserRepository>();
            repo
                .GetUserByIdAsync(1.ToString())
                .Returns((UserDto?)null);
            repo
                .UpdateUserAsync(1.ToString(), Arg.Any<UserDto>())
                .Returns(Result<UserDto>.Fail("The Id mentioned does not exist.", "404"));
            var controller = new UserController(repo);
            if (role == "Admin")
                controller.InitializeAdminUser();
            else if (role == "User")
                controller.InitializeRegularUser();
            else if (role == "Unauthenticated")
                controller.InitializeUnauthenticatedUser();
            var updatedUser = new UserDto { Id = 1.ToString(), UserName = "def_updated", FullName = "Def Updated", Role = "" };

            // Act
            IActionResult result;

            // Simulate [Authorize(Roles = "Admin")] behavior in unit tests
            if (!controller.IsAuthorizedAsAdmin())
                result = new UnauthorizedResult();
            else
                result = await controller.PutUser(1.ToString(), updatedUser);

            // Assert
            if (role == "Admin")
            {
                var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
                Assert.Equal("The Id mentioned does not exist.", notFoundResult.Value);
            }
            else
                Assert.IsType<UnauthorizedResult>(result);
        }

        [Theory]
        [InlineData("Admin")]
        [InlineData("User")]
        [InlineData("Unauthenticated")]
        public async Task PutUser_AsDifferentRoles_WhenModelStateIsNotValid_ReturnsBadRequest(string? role)
        {
            // Arrange
            var repo = Substitute.For<IUserRepository>();
            repo
                .GetUserByIdAsync(1.ToString())
                .Returns(new UserDto { Id = 1.ToString() });
            var controller = new UserController(repo);

            if (role == "Admin")
                controller.InitializeAdminUser();
            else if (role == "User")
                controller.InitializeRegularUser();
            else if (role == "Unauthenticated")
                controller.InitializeUnauthenticatedUser();

            var invalidUser = new UserDto { Id = 1.ToString(), UserName = "def_updated", FullName = "Def Updated", Role = "" };

            controller.ModelState.AddModelError("UserName", "UserName is required.");
            controller.ModelState.AddModelError("FullName", "FullName is required.");
            controller.ModelState.AddModelError("Role", "Role is required.");

            // Act
            IActionResult result;

            // Simulate [Authorize(Roles = "Admin")] behavior in unit tests
            if (!controller.IsAuthorizedAsAdmin())
                result = new UnauthorizedResult();
            else
                result = await controller.PutUser(1.ToString(), invalidUser);

            // Assert
            if (role == "Admin")
                Assert.IsType<BadRequestObjectResult>(result);
            else
                Assert.IsType<UnauthorizedResult>(result);
        }

        [Theory]
        [InlineData("Admin")]
        [InlineData("User")]
        [InlineData("Unauthenticated")]
        public async Task PutUser_AsDifferentRoles_WhenFocusingWrongId_ReturnsBadRequest(string? role)
        {
            // Arrange
            var repo = Substitute.For<IUserRepository>();
            repo
                .GetUserByIdAsync(1.ToString())
                .Returns(new UserDto { Id = 1.ToString() });
            var controller = new UserController(repo);

            if (role == "Admin")
                controller.InitializeAdminUser();
            else if (role == "User")
                controller.InitializeRegularUser();
            else if (role == "Unauthenticated")
                controller.InitializeUnauthenticatedUser();

            var updatedUser = new UserDto { Id = 2.ToString(), UserName = "def_updated", FullName = "Def Updated", Role = "" };

            // Act
            IActionResult result;

            // Simulate [Authorize(Roles = "Admin")] behavior in unit tests
            if (!controller.IsAuthorizedAsAdmin())
                result = new UnauthorizedResult();
            else
                result = await controller.PutUser(1.ToString(), updatedUser);

            // Assert
            if (role == "Admin")
                Assert.IsType<BadRequestObjectResult>(result);
            else
                Assert.IsType<UnauthorizedResult>(result);
        }

        // Tests for DeleteUser
        [Theory]
        [InlineData("Admin")]
        [InlineData("User")]
        [InlineData("Unauthenticated")]
        public async Task DeleteUser_AsDifferentRoles_ReturnsExpected(string? role)
        {
            // Arrange
            var repo = Substitute.For<IUserRepository>();
            repo
                .GetUserByIdAsync(1.ToString())
                .Returns(new UserDto { Id = 1.ToString() });
            repo
                .DeleteUserAsync(1.ToString())
                .Returns(Result<bool>.Ok(true));
            var controller = new UserController(repo);
            if (role == "Admin")
                controller.InitializeAdminUser();
            else if (role == "User")
                controller.InitializeRegularUser();
            else if (role == "Unauthenticated")
                controller.InitializeUnauthenticatedUser();

            // Act
            IActionResult result;

            // Simulate [Authorize(Roles = "Admin")] behavior in unit tests
            if (!controller.IsAuthorizedAsAdmin())
                result = new UnauthorizedResult();
            else
                result = await controller.DeleteUser(1.ToString());

            // Assert
            if (role == "Admin")
                Assert.IsType<OkObjectResult>(result);
            else
                Assert.IsType<UnauthorizedResult>(result);
        }

        [Theory]
        [InlineData("Admin")]
        [InlineData("User")]
        [InlineData("Unauthenticated")]
        public async Task DeleteUser_AsDifferentRoles_ReturnsNotFound(string? role)
        {
            // Arrange
            var repo = Substitute.For<IUserRepository>();
            repo
                .GetUserByIdAsync(1.ToString())
                .Returns((UserDto?)null);
            repo
                .DeleteUserAsync(Arg.Any<string>())
                .Returns(Task.FromResult(Result<bool>.Fail("The Id mentioned does not exist.", "404"))); 
            var controller = new UserController(repo);

            if (role == "Admin")
                controller.InitializeAdminUser();
            else if (role == "User")
                controller.InitializeRegularUser();
            else if (role == "Unauthenticated")
                controller.InitializeUnauthenticatedUser();

            // Act
            IActionResult result;

            // Simulate [Authorize(Roles = "Admin")] behavior in unit tests
            if (!controller.IsAuthorizedAsAdmin())
                result = new UnauthorizedResult();
            else
                result = await controller.DeleteUser(1.ToString());

            // Assert
            if (role == "Admin")
            {
                var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
                Assert.Equal("The Id mentioned does not exist.", notFoundResult.Value);
            }
            else
                Assert.IsType<UnauthorizedResult>(result);
        }
    }
}