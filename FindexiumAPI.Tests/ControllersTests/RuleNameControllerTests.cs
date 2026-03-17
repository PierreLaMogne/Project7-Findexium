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
        // Tests for GetRuleNames
        [Theory]
        [InlineData("Admin")]
        [InlineData("User")]
        [InlineData("Unauthenticated")]
        public async Task GetRuleNames_AsDifferentRoles_ReturnsExpected(string? role)
        {
            // Arrange
            var repo = Substitute.For<IRuleNameRepository>();
            repo
                .GetAllAsync()
                .Returns(new List<RuleNameDto> { new RuleNameDto { Id = 1 }, new RuleNameDto { Id = 2 } });
            var controller = new RuleNameController(repo);

            if (role == "Admin")
                controller.InitializeAdminUser();
            else if (role == "User")
                controller.InitializeRegularUser();
            else if (role == "Unauthenticated")
                controller.InitializeUnauthenticatedUser();

            // Act
            ActionResult<IEnumerable<RuleNameDto>> result;

            // Simulate [Authorize(Policy = "Users")] behavior in unit tests
            if (!controller.IsAuthorizedForUsersPolicy())
                result = new ActionResult<IEnumerable<RuleNameDto>>(new UnauthorizedResult());
            else
                result = await controller.GetRuleNames();

            // Assert
            if (role == "Unauthenticated")
                Assert.IsType<UnauthorizedResult>(result.Result);
            else
            {
                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var ruleNames = Assert.IsAssignableFrom<IEnumerable<RuleNameDto>>(okResult.Value);
                Assert.Equal(2, ruleNames.Count());
            }
        }

        [Theory]
        [InlineData("Admin")]
        [InlineData("User")]
        [InlineData("Unauthenticated")]
        public async Task GetRuleNames_AsDifferentRoles_ReturnsNotFound(string? role)
        {
            // Arrange
            var repo = Substitute.For<IRuleNameRepository>();
            repo
                .GetAllAsync()
                .Returns(new List<RuleNameDto>());
            var controller = new RuleNameController(repo);

            if (role == "Admin")
                controller.InitializeAdminUser();
            else if (role == "User")
                controller.InitializeRegularUser();
            else if (role == "Unauthenticated")
                controller.InitializeUnauthenticatedUser();

            // Act
            ActionResult<IEnumerable<RuleNameDto>> result;

            // Simulate [Authorize(Policy = "Users")] behavior in unit tests
            if (!controller.IsAuthorizedForUsersPolicy())
                result = new ActionResult<IEnumerable<RuleNameDto>>(new UnauthorizedResult());
            else
                result = await controller.GetRuleNames();

            // Assert
            if (role == "Unauthenticated")
                Assert.IsType<UnauthorizedResult>(result.Result);
            else
            {
                var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
                Assert.Equal("No RuleName found.", notFoundResult.Value);
            }
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
            repo
                .GetByIdAsync(1)
                .Returns(new RuleNameDto { Id = 1 });
            var controller = new RuleNameController(repo);

            if (role == "Admin")
                controller.InitializeAdminUser();
            else if (role == "User")
                controller.InitializeRegularUser();
            else if (role == "Unauthenticated")
                controller.InitializeUnauthenticatedUser();

            // Act
            ActionResult<RuleNameDto> result;

            // Simulate [Authorize(Policy = "Users")] behavior in unit tests
            if (!controller.IsAuthorizedForUsersPolicy())
                result = new ActionResult<RuleNameDto>(new UnauthorizedResult());
            else
                result = await controller.GetRuleName(1);

            // Assert
            if (role == "Unauthenticated")
                Assert.IsType<UnauthorizedResult>(result.Result);
            else
            {
                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var ruleName = Assert.IsAssignableFrom<RuleNameDto>(okResult.Value);
                Assert.Equal(1, ruleName.Id);

            }
        }

        [Theory]
        [InlineData("Admin")]
        [InlineData("User")]
        [InlineData("Unauthenticated")]
        public async Task GetRuleName_AsDifferentRoles_ReturnsNotFound(string? role)
        {
            // Arrange
            var repo = Substitute.For<IRuleNameRepository>();
            repo
                .GetByIdAsync(1)
                .Returns((RuleNameDto?)null);
            var controller = new RuleNameController(repo);

            if (role == "Admin")
                controller.InitializeAdminUser();
            else if (role == "User")
                controller.InitializeRegularUser();
            else if (role == "Unauthenticated")
                controller.InitializeUnauthenticatedUser();

            // Act
            ActionResult<RuleNameDto> result;

            // Simulate [Authorize(Policy = "Users")] behavior in unit tests
            if (!controller.IsAuthorizedForUsersPolicy())
                result = new ActionResult<RuleNameDto>(new UnauthorizedResult());
            else
                result = await controller.GetRuleName(1);

            // Assert
            if (role == "Unauthenticated")
                Assert.IsType<UnauthorizedResult>(result.Result);
            else
            {
                var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
                Assert.Equal("The Id mentioned does not exist.", notFoundResult.Value);
            }
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
            repo
                .AddAsync(Arg.Any<RuleNameDto>())
                .Returns(callInfo =>
                {
                    var dto = callInfo.Arg<RuleNameDto>();
                    dto.Id = 1;
                    return dto;
                });
            var controller = new RuleNameController(repo);

            if (role == "Admin")
                controller.InitializeAdminUser();
            else if (role == "User")
                controller.InitializeRegularUser();
            else if (role == "Unauthenticated")
                controller.InitializeUnauthenticatedUser();

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
            var controller = new RuleNameController(repo);

            if (role == "Admin")
                controller.InitializeAdminUser();
            else if (role == "User")
                controller.InitializeRegularUser();
            else if (role == "Unauthenticated")
                controller.InitializeUnauthenticatedUser();

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
            repo
                .GetByIdAsync(1)
                .Returns(new RuleNameDto { Id = 1 });
            repo
                .UpdateAsync(1, Arg.Any<RuleNameDto>())
                .Returns(true);
            var controller = new RuleNameController(repo);

            if (role == "Admin")
                controller.InitializeAdminUser();
            else if (role == "User")
                controller.InitializeRegularUser();
            else if (role == "Unauthenticated")
                controller.InitializeUnauthenticatedUser();

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
            repo
                .GetByIdAsync(1)
                .Returns((RuleNameDto?)null);
            var controller = new RuleNameController(repo);
            if (role == "Admin")
                controller.InitializeAdminUser();
            else if (role == "User")
                controller.InitializeRegularUser();
            else if (role == "Unauthenticated")
                controller.InitializeUnauthenticatedUser();
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
            repo
                .GetByIdAsync(1)
                .Returns(new RuleNameDto { Id = 1 });
            var controller = new RuleNameController(repo);

            if (role == "Admin")
                controller.InitializeAdminUser();
            else if (role == "User")
                controller.InitializeRegularUser();
            else if (role == "Unauthenticated")
                controller.InitializeUnauthenticatedUser();

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
            repo
                .GetByIdAsync(1)
                .Returns(new RuleNameDto { Id = 1 });
            var controller = new RuleNameController(repo);

            if (role == "Admin")
                controller.InitializeAdminUser();
            else if (role == "User")
                controller.InitializeRegularUser();
            else if (role == "Unauthenticated")
                controller.InitializeUnauthenticatedUser();

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
            repo
                .GetByIdAsync(1)
                .Returns(new RuleNameDto { Id = 1 });
            repo
                .DeleteAsync(1)
                .Returns(true);
            var controller = new RuleNameController(repo);
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
            repo
                .GetByIdAsync(1)
                .Returns((RuleNameDto?)null);
            var controller = new RuleNameController(repo);

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