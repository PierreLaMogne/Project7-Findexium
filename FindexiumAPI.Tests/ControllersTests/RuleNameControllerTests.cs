using FindexiumAPI.Controllers;
using FindexiumAPI.Models;
using FindexiumAPI.Repositories;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;

namespace FindexiumAPI.Tests.Controllers
{
    public class RuleNameControllerTests
    {
        // Helper method to create a controller with a specific user role
        private RuleNameController CreateControllerWithRole(string? role, IRuleNameRepository? repo = null)
        {
            repo ??= Substitute.For<IRuleNameRepository>();
            var controller = new RuleNameController(repo);

            switch (role)
            {
                case "Admin":
                    controller.InitializeAdminUser();
                    break;
                case "User":
                    controller.InitializeRegularUser();
                    break;
                default:
                    controller.InitializeUnauthenticatedUser();
                    break;
            }

            return controller;
        }


        // Tests for GetRuleNames
        [Theory]
        [InlineData("Admin")]
        [InlineData("User")]
        [InlineData("Unauthenticated")]
        public async Task GetRuleNames_AsDifferentRoles_ReturnsExpected(string? role)
        {
            // Arrange
            var repo = Substitute.For<IRuleNameRepository>();
            repo.GetAllAsync().Returns(new List<RuleNameDto> { new RuleNameDto { Id = 1 }, new RuleNameDto { Id = 2 } }); // Mocking the repository to return a list of RuleNameDto
            var controller = CreateControllerWithRole(role, repo);

            // Act
            ActionResult<IEnumerable<RuleNameDto>> result;

            // Simulate [Authorize(Policy = "Users")] behavior in unit tests
            if (!controller.IsAuthorizedForUsersPolicy())
                result = new ActionResult<IEnumerable<RuleNameDto>>(new UnauthorizedResult());
            else
                result = await controller.GetRuleNames();

            // Assert
            if (role == "Admin" || role == "User")
            {
                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var ruleNames = Assert.IsAssignableFrom<IEnumerable<RuleNameDto>>(okResult.Value);
                Assert.Equal(2, ruleNames.Count());
            }
            else
                Assert.IsType<UnauthorizedResult>(result.Result);
            }

        [Theory]
        [InlineData("Admin")]
        [InlineData("User")]
        [InlineData("Unauthenticated")]
        public async Task GetRuleNames_AsDifferentRoles_ReturnsNotFound(string? role)
        {
            // Arrange
            var repo = Substitute.For<IRuleNameRepository>();
            repo.GetAllAsync().Returns(new List<RuleNameDto>()); // Mocking the repository to return an empty list of RuleNameDto
            var controller = CreateControllerWithRole(role, repo);

            // Act
            ActionResult<IEnumerable<RuleNameDto>> result;

            // Simulate [Authorize(Policy = "Users")] behavior in unit tests
            if (!controller.IsAuthorizedForUsersPolicy())
                result = new ActionResult<IEnumerable<RuleNameDto>>(new UnauthorizedResult());
            else
                result = await controller.GetRuleNames();

            // Assert
            if (role == "Admin" ||role == "User")
            {
                var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
                Assert.Equal("No RuleName found.", notFoundResult.Value);
            }
            else
                Assert.IsType<UnauthorizedResult>(result.Result);
        }

        // Tests for GetRuleName
        [Theory]
        [InlineData("Admin")]
        [InlineData("User")]
        [InlineData("Unauthenticated")]
        public async Task GetRuleName_AsDifferentRoles_ReturnsExpected(string? role)
        {
            // Arrange
            var repo = Substitute.For<IRuleNameRepository>();
            repo.GetByIdAsync(1).Returns(new RuleNameDto { Id = 1 }); // Mocking the repository to return a RuleNameDto for the specified ID
            var controller = CreateControllerWithRole(role, repo);

            // Act
            ActionResult<RuleNameDto> result;

            // Simulate [Authorize(Policy = "Users")] behavior in unit tests
            if (!controller.IsAuthorizedForUsersPolicy())
                result = new ActionResult<RuleNameDto>(new UnauthorizedResult());
            else
                result = await controller.GetRuleName(1);

            // Assert
            if (role == "Admin" ||role == "User")
            {
                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var ruleName = Assert.IsAssignableFrom<RuleNameDto>(okResult.Value);
                Assert.Equal(1, ruleName.Id);
            }
            else 
                Assert.IsType<UnauthorizedResult>(result.Result);
        }

        [Theory]
        [InlineData("Admin")]
        [InlineData("User")]
        [InlineData("Unauthenticated")]
        public async Task GetRuleName_AsDifferentRoles_ReturnsNotFound(string? role)
        {
            // Arrange
            var repo = Substitute.For<IRuleNameRepository>();
            repo.GetByIdAsync(1).Returns((RuleNameDto?)null); // Mocking the repository to return null for the specified ID
            var controller = CreateControllerWithRole(role, repo);

            // Act
            ActionResult<RuleNameDto> result;

            // Simulate [Authorize(Policy = "Users")] behavior in unit tests
            if (!controller.IsAuthorizedForUsersPolicy())
                result = new ActionResult<RuleNameDto>(new UnauthorizedResult());
            else
                result = await controller.GetRuleName(1);

            // Assert
            if (role == "Admin" || role== "User")
            {
                var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
                Assert.Equal("The Id mentioned does not exist.", notFoundResult.Value);
            }
            else
                Assert.IsType<UnauthorizedResult>(result.Result);
        }

        // Tests for PostRuleName
        [Theory]
        [InlineData("Admin")]
        [InlineData("User")]
        [InlineData("Unauthenticated")]
        public async Task PostRuleName_AsDifferentRoles_ReturnsExcepted(string? role)
        {
            // Arrange
            var repo = Substitute.For<IRuleNameRepository>();
            repo.AddAsync(Arg.Any<RuleNameDto>()).Returns(callInfo =>
                {
                    var dto = callInfo.Arg<RuleNameDto>();
                    dto.Id = 1;
                    return dto;
                }); // Mocking the repository to return the created RuleNameDto with an assigned ID
            var controller = CreateControllerWithRole(role, repo);

            var newRuleName = new RuleNameDto { Id = 1, Name = "Rule1", Description = "Description1", Json = "{}", Template = "Template1", SqlStr = "SELECT * FROM Table1", SqlPart = "WHERE Condition1" };

            // Act
            ActionResult<RuleNameDto> result;

            // Simulate [Authorize(Roles = "Admin")] behavior in unit tests
            if (!controller.IsAuthorizedAsAdmin())
                result = new ActionResult<RuleNameDto>(new UnauthorizedResult());
            else
                result = await controller.PostRuleName(newRuleName);

            // Assert
            if (role == "Admin")
            {
                var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
                var createdRuleName = Assert.IsAssignableFrom<RuleNameDto>(createdAtActionResult.Value);
                Assert.Equal(1, createdRuleName.Id);
                Assert.Equal("Rule1", createdRuleName.Name);
                Assert.Equal("Description1", createdRuleName.Description);
                Assert.Equal("{}", createdRuleName.Json);
                Assert.Equal("Template1", createdRuleName.Template);
                Assert.Equal("SELECT * FROM Table1", createdRuleName.SqlStr);
                Assert.Equal("WHERE Condition1", createdRuleName.SqlPart);
            }
            else
                Assert.IsType<UnauthorizedResult>(result.Result);
        }

        [Theory]
        [InlineData("Admin")]
        [InlineData("User")]
        [InlineData("Unauthenticated")]
        public async Task PostRuleName_AsDifferentRoles_WhenModelStateIsNotValid_ReturnsBadRequest(string? role)
        {
            // Arrange
            var repo = Substitute.For<IRuleNameRepository>();
            var controller = CreateControllerWithRole(role, repo);

            var invalidRuleName = new RuleNameDto { Name = "", Description = "", Json = "{}", Template = "", SqlStr = "", SqlPart = "" };

            controller.ModelState.AddModelError("Name", "Name is required.");
            controller.ModelState.AddModelError("Description", "Description is required.");
            controller.ModelState.AddModelError("Template", "Template is required.");
            controller.ModelState.AddModelError("SqlStr", "SqlStr is required.");
            controller.ModelState.AddModelError("SqlPart", "SqlPart is required.");

            // Act
            ActionResult<RuleNameDto> result;

            // Simulate [Authorize(Roles = "Admin")] behavior in unit tests
            if (!controller.IsAuthorizedAsAdmin())
                result = new ActionResult<RuleNameDto>(new UnauthorizedResult());
            else
                result = await controller.PostRuleName(invalidRuleName);

            // Assert
            if (role == "Admin")
                Assert.IsType<BadRequestObjectResult>(result.Result);
            else
                Assert.IsType<UnauthorizedResult>(result.Result);
        }

        // Tests for PutRuleName
        [Theory]
        [InlineData("Admin")]
        [InlineData("User")]
        [InlineData("Unauthenticated")]
        public async Task PutRuleName_AsDifferentRoles_ReturnsExpected(string? role)
        {
            // Arrange
            var repo = Substitute.For<IRuleNameRepository>();
            repo.GetByIdAsync(1).Returns(new RuleNameDto { Id = 1 }); // Mocking the repository to return a RuleNameDto for the specified ID
            repo.UpdateAsync(1, Arg.Any<RuleNameDto>()).Returns(true); // Mocking the repository to return true for a successful update operation
            var controller = CreateControllerWithRole(role, repo);

            var updatedRuleName = new RuleNameDto { Id = 1, Name = "Rule2", Description = "Description2", Json = "{}", Template = "Template2", SqlStr = "SELECT * FROM Table2", SqlPart = "WHERE Condition2" };

            // Act
            IActionResult result;

            // Simulate [Authorize(Roles = "Admin")] behavior in unit tests
            if (!controller.IsAuthorizedAsAdmin())
                result = new UnauthorizedResult();
            else
                result = await controller.PutRuleName(1, updatedRuleName);

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
        public async Task PutRuleName_AsDifferentRoles_ReturnsNotFound(string? role)
        {
            // Arrange
            var repo = Substitute.For<IRuleNameRepository>();
            repo.GetByIdAsync(1).Returns((RuleNameDto?)null); // Mocking the repository to return null for the specified ID
            var controller = CreateControllerWithRole(role, repo);

            var updatedRuleName = new RuleNameDto { Id = 1, Name = "Rule2", Description = "Description2", Json = "{}", Template = "Template2", SqlStr = "SELECT * FROM Table2", SqlPart = "WHERE Condition2" };
            // Act
            IActionResult result;

            // Simulate [Authorize(Roles = "Admin")] behavior in unit tests
            if (!controller.IsAuthorizedAsAdmin())
                result = new UnauthorizedResult();
            else
                result = await controller.PutRuleName(1, updatedRuleName);

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
        public async Task PutRuleName_AsDifferentRoles_WhenModelStateIsNotValid_ReturnsBadRequest(string? role)
        {
            // Arrange
            var repo = Substitute.For<IRuleNameRepository>();
            repo.GetByIdAsync(1).Returns(new RuleNameDto { Id = 1 }); // Mocking the repository to return a RuleNameDto for the specified ID
            var controller = CreateControllerWithRole(role, repo);

            var invalidRuleName = new RuleNameDto { Id = 1, Name = "Rule2", Description = "Description2", Json = "{}", Template = "Template2", SqlStr = "SELECT * FROM Table2", SqlPart = "WHERE Condition2" };

            controller.ModelState.AddModelError("Name", "Name is required.");
            controller.ModelState.AddModelError("Description", "Description is required.");
            controller.ModelState.AddModelError("Template", "Template is required.");
            controller.ModelState.AddModelError("SqlStr", "SqlStr is required.");
            controller.ModelState.AddModelError("SqlPart", "SqlPart is required.");

            // Act
            IActionResult result;

            // Simulate [Authorize(Roles = "Admin")] behavior in unit tests
            if (!controller.IsAuthorizedAsAdmin())
                result = new UnauthorizedResult();
            else
                result = await controller.PutRuleName(1, invalidRuleName);

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
        public async Task PutRuleName_AsDifferentRoles_WhenFocusingWrongId_ReturnsBadRequest(string? role)
        {
            // Arrange
            var repo = Substitute.For<IRuleNameRepository>();
            repo.GetByIdAsync(1).Returns(new RuleNameDto { Id = 1 }); // Mocking the repository to return a RuleNameDto for the specified ID
            var controller = CreateControllerWithRole(role, repo);

            var updatedRuleName = new RuleNameDto { Id = 2, Name = "Rule1", Description = "Description1", Json = "{}", Template = "Template1", SqlStr = "SELECT * FROM Table1", SqlPart = "WHERE Condition1" };

            // Act
            IActionResult result;

            // Simulate [Authorize(Roles = "Admin")] behavior in unit tests
            if (!controller.IsAuthorizedAsAdmin())
                result = new UnauthorizedResult();
            else
                result = await controller.PutRuleName(1, updatedRuleName);

            // Assert
            if (role == "Admin")
                Assert.IsType<BadRequestObjectResult>(result);
            else
                Assert.IsType<UnauthorizedResult>(result);
        }

        // Tests for DeleteRuleName
        [Theory]
        [InlineData("Admin")]
        [InlineData("User")]
        [InlineData("Unauthenticated")]
        public async Task DeleteRuleName_AsDifferentRoles_ReturnsExpected(string? role)
        {
            // Arrange
            var repo = Substitute.For<IRuleNameRepository>();
            repo.GetByIdAsync(1).Returns(new RuleNameDto { Id = 1 }); // Mocking the repository to return a RuleNameDto for the specified ID
            repo.DeleteAsync(1).Returns(true); // Mocking the repository to return true for a successful delete operation
            var controller = CreateControllerWithRole(role, repo);

            // Act
            IActionResult result;

            // Simulate [Authorize(Roles = "Admin")] behavior in unit tests
            if (!controller.IsAuthorizedAsAdmin())
                result = new UnauthorizedResult();
            else
                result = await controller.DeleteRuleName(1);

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
        public async Task DeleteRuleName_AsDifferentRoles_ReturnsNotFound(string? role)
        {
            // Arrange
            var repo = Substitute.For<IRuleNameRepository>();
            repo.GetByIdAsync(1).Returns((RuleNameDto?)null); // Mocking the repository to return null for the specified ID
            var controller = CreateControllerWithRole(role, repo);

            // Act
            IActionResult result;

            // Simulate [Authorize(Roles = "Admin")] behavior in unit tests
            if (!controller.IsAuthorizedAsAdmin())
                result = new UnauthorizedResult();
            else
                result = await controller.DeleteRuleName(1);

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