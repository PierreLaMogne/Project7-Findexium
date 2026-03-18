using FindexiumAPI.Controllers;
using FindexiumAPI.Models;
using FindexiumAPI.Repositories;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;

namespace FindexiumAPI.Tests.Controllers
{
    public class CurvePointControllerTests
    {
        // Tests for GetCurvePoints
        [Theory]
        [InlineData("Admin")]
        [InlineData("User")]
        [InlineData("Unauthenticated")]
        public async Task GetCurvePoints_AsDifferentRoles_ReturnsExpected(string? role)
        {
            // Arrange
            var repo = Substitute.For<ICurvePointRepository>();
            repo
                .GetAllAsync()
                .Returns(new List<CurvePointDto> { new CurvePointDto { Id = 1 }, new CurvePointDto { Id = 2 } });
            var controller = new CurvePointController(repo);

            if (role == "Admin")
                controller.InitializeAdminUser();
            else if (role == "User")
                controller.InitializeRegularUser();
            else if (role == "Unauthenticated")
                controller.InitializeUnauthenticatedUser();

            // Act
            ActionResult<IEnumerable<CurvePointDto>> result;

            // Simulate [Authorize(Policy = "Users")] behavior in unit tests
            if (!controller.IsAuthorizedForUsersPolicy())
                result = new ActionResult<IEnumerable<CurvePointDto>>(new UnauthorizedResult());
            else
                result = await controller.GetCurvePoints();

            // Assert
            if (role == "Unauthenticated")
                Assert.IsType<UnauthorizedResult>(result.Result);
            else
            {
                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var curvePoints = Assert.IsAssignableFrom<IEnumerable<CurvePointDto>>(okResult.Value);
                Assert.Equal(2, curvePoints.Count());
            }
        }

        [Theory]
        [InlineData("Admin")]
        [InlineData("User")]
        [InlineData("Unauthenticated")]
        public async Task GetCurvePoints_AsDifferentRoles_ReturnsNotFound(string? role)
        {
            // Arrange
            var repo = Substitute.For<ICurvePointRepository>();
            repo
                .GetAllAsync()
                .Returns(new List<CurvePointDto>());
            var controller = new CurvePointController(repo);

            if (role == "Admin")
                controller.InitializeAdminUser();
            else if (role == "User")
                controller.InitializeRegularUser();
            else if (role == "Unauthenticated")
                controller.InitializeUnauthenticatedUser();

            // Act
            ActionResult<IEnumerable<CurvePointDto>> result;

            // Simulate [Authorize(Policy = "Users")] behavior in unit tests
            if (!controller.IsAuthorizedForUsersPolicy())
                result = new ActionResult<IEnumerable<CurvePointDto>>(new UnauthorizedResult());
            else
                result = await controller.GetCurvePoints();

            // Assert
            if (role == "Unauthenticated")
                Assert.IsType<UnauthorizedResult>(result.Result);
            else
            {
                var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
                Assert.Equal("No CurvePoint found.", notFoundResult.Value);
            }
        }

        // Tests for GetCurvePoint
        [Theory]
        [InlineData("Admin")]
        [InlineData("User")]
        [InlineData("Unauthenticated")]
        public async Task GetCurvePoint_AsDifferentRoles_ReturnsExpected(string? role)
        {
            // Arrange
            var repo = Substitute.For<ICurvePointRepository>();
            repo
                .GetByIdAsync(1)
                .Returns(new CurvePointDto { Id = 1 });
            var controller = new CurvePointController(repo);

            if (role == "Admin")
                controller.InitializeAdminUser();
            else if (role == "User")
                controller.InitializeRegularUser();
            else if (role == "Unauthenticated")
                controller.InitializeUnauthenticatedUser();

            // Act
            ActionResult<CurvePointDto> result;

            // Simulate [Authorize(Policy = "Users")] behavior in unit tests
            if (!controller.IsAuthorizedForUsersPolicy())
                result = new ActionResult<CurvePointDto>(new UnauthorizedResult());
            else
                result = await controller.GetCurvePoint(1);

            // Assert
            if (role == "Unauthenticated")
                Assert.IsType<UnauthorizedResult>(result.Result);
            else
            {
                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var curvePoint = Assert.IsAssignableFrom<CurvePointDto>(okResult.Value);
                Assert.Equal(1, curvePoint.Id);

            }
        }

        [Theory]
        [InlineData("Admin")]
        [InlineData("User")]
        [InlineData("Unauthenticated")]
        public async Task GetCurvePoint_AsDifferentRoles_ReturnsNotFound(string? role)
        {
            // Arrange
            var repo = Substitute.For<ICurvePointRepository>();
            repo
                .GetByIdAsync(1)
                .Returns((CurvePointDto?)null);
            var controller = new CurvePointController(repo);

            if (role == "Admin")
                controller.InitializeAdminUser();
            else if (role == "User")
                controller.InitializeRegularUser();
            else if (role == "Unauthenticated")
                controller.InitializeUnauthenticatedUser();

            // Act
            ActionResult<CurvePointDto> result;

            // Simulate [Authorize(Policy = "Users")] behavior in unit tests
            if (!controller.IsAuthorizedForUsersPolicy())
                result = new ActionResult<CurvePointDto>(new UnauthorizedResult());
            else
                result = await controller.GetCurvePoint(1);

            // Assert
            if (role == "Unauthenticated")
                Assert.IsType<UnauthorizedResult>(result.Result);
            else
            {
                var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
                Assert.Equal("The Id mentioned does not exist.", notFoundResult.Value);
            }
        }

        // Tests for PostCurvePoint
        [Theory]
        [InlineData("Admin")]
        [InlineData("User")]
        [InlineData("Unauthenticated")]
        public async Task PostCurvePoint_AsDifferentRoles_ReturnsExcepted(string? role)
        {
            // Arrange
            var repo = Substitute.For<ICurvePointRepository>();
            repo
                .AddAsync(Arg.Any<CurvePointDto>())
                .Returns(callInfo =>
                {
                    var dto = callInfo.Arg<CurvePointDto>();
                    dto.Id = 1;
                    return dto;
                });
            var controller = new CurvePointController(repo);

            if (role == "Admin")
                controller.InitializeAdminUser();
            else if (role == "User")
                controller.InitializeRegularUser();
            else if (role == "Unauthenticated")
                controller.InitializeUnauthenticatedUser();

            var newCurvePoint = new CurvePointDto { Id = 1, CurveId = 10, Term = 1.0, CurvePointValue = 100.0 };

            // Act
            ActionResult<CurvePointDto> result;

            // Simulate [Authorize(Roles = "Admin")] behavior in unit tests
            if (!controller.IsAuthorizedAsAdmin())
                result = new ActionResult<CurvePointDto>(new UnauthorizedResult());
            else
                result = await controller.PostCurvePoint(newCurvePoint);

            // Assert
            if (role == "Admin")
            {
                var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
                var createdCurvePoint = Assert.IsAssignableFrom<CurvePointDto>(createdAtActionResult.Value);
                Assert.Equal(1, createdCurvePoint.Id);
                Assert.Equal((byte)10, createdCurvePoint.CurveId);
                Assert.Equal(1.0, createdCurvePoint.Term);
                Assert.Equal(100.0, createdCurvePoint.CurvePointValue);
            }
            else
                Assert.IsType<UnauthorizedResult>(result.Result);
        }

        [Theory]
        [InlineData("Admin")]
        [InlineData("User")]
        [InlineData("Unauthenticated")]
        public async Task PostCurvePoint_AsDifferentRoles_WhenModelStateIsNotValid_ReturnsBadRequest(string? role)
        {
            // Arrange
            var repo = Substitute.For<ICurvePointRepository>();
            var controller = new CurvePointController(repo);

            if (role == "Admin")
                controller.InitializeAdminUser();
            else if (role == "User")
                controller.InitializeRegularUser();
            else if (role == "Unauthenticated")
                controller.InitializeUnauthenticatedUser();

            var invalidCurvePoint = new CurvePointDto { CurveId = null, Term = null, CurvePointValue = null };

            controller.ModelState.AddModelError("CurveId", "CurveId is required.");
            controller.ModelState.AddModelError("Term", "Term is required.");
            controller.ModelState.AddModelError("CurvePointValue", "CurvePointValue is required.");

            // Act
            ActionResult<CurvePointDto> result;

            // Simulate [Authorize(Roles = "Admin")] behavior in unit tests
            if (!controller.IsAuthorizedAsAdmin())
                result = new ActionResult<CurvePointDto>(new UnauthorizedResult());
            else
                result = await controller.PostCurvePoint(invalidCurvePoint);

            // Assert
            if (role == "Admin")
                Assert.IsType<BadRequestObjectResult>(result.Result);
            else
                Assert.IsType<UnauthorizedResult>(result.Result);
        }

        // Tests for PutCurvePoint
        [Theory]
        [InlineData("Admin")]
        [InlineData("User")]
        [InlineData("Unauthenticated")]
        public async Task PutCurvePoint_AsDifferentRoles_ReturnsExpected(string? role)
        {
            // Arrange
            var repo = Substitute.For<ICurvePointRepository>();
            repo
                .GetByIdAsync(1)
                .Returns(new CurvePointDto { Id = 1 });
            repo
                .UpdateAsync(1, Arg.Any<CurvePointDto>())
                .Returns(true);
            var controller = new CurvePointController(repo);

            if (role == "Admin")
                controller.InitializeAdminUser();
            else if (role == "User")
                controller.InitializeRegularUser();
            else if (role == "Unauthenticated")
                controller.InitializeUnauthenticatedUser();

            var updatedCurvePoint = new CurvePointDto { Id = 1, CurveId = 20, Term = 2.0, CurvePointValue = 200.0 };

            // Act
            IActionResult result;

            // Simulate [Authorize(Roles = "Admin")] behavior in unit tests
            if (!controller.IsAuthorizedAsAdmin())
                result = new UnauthorizedResult();
            else
                result = await controller.PutCurvePoint(1, updatedCurvePoint);

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
        public async Task PutCurvePoint_AsDifferentRoles_ReturnsNotFound(string? role)
        {
            // Arrange
            var repo = Substitute.For<ICurvePointRepository>();
            repo
                .GetByIdAsync(1)
                .Returns((CurvePointDto?)null);
            var controller = new CurvePointController(repo);
            if (role == "Admin")
                controller.InitializeAdminUser();
            else if (role == "User")
                controller.InitializeRegularUser();
            else if (role == "Unauthenticated")
                controller.InitializeUnauthenticatedUser();
            var updatedCurvePoint = new CurvePointDto { Id = 1, CurveId = 20, Term = 2.0, CurvePointValue = 200.0 };
            // Act
            IActionResult result;

            // Simulate [Authorize(Roles = "Admin")] behavior in unit tests
            if (!controller.IsAuthorizedAsAdmin())
                result = new UnauthorizedResult();
            else
                result = await controller.PutCurvePoint(1, updatedCurvePoint);

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
        public async Task PutCurvePoint_AsDifferentRoles_WhenModelStateIsNotValid_ReturnsBadRequest(string? role)
        {
            // Arrange
            var repo = Substitute.For<ICurvePointRepository>();
            repo
                .GetByIdAsync(1)
                .Returns(new CurvePointDto { Id = 1 });
            var controller = new CurvePointController(repo);

            if (role == "Admin")
                controller.InitializeAdminUser();
            else if (role == "User")
                controller.InitializeRegularUser();
            else if (role == "Unauthenticated")
                controller.InitializeUnauthenticatedUser();

            var invalidCurvePoint = new CurvePointDto { Id = 1, CurveId = 20, Term = 2.0, CurvePointValue = 200.0 };

            controller.ModelState.AddModelError("CurveId", "CurveId is required.");
            controller.ModelState.AddModelError("Term", "Term is required.");
            controller.ModelState.AddModelError("CurvePointValue", "CurvePointValue is required.");

            // Act
            IActionResult result;

            // Simulate [Authorize(Roles = "Admin")] behavior in unit tests
            if (!controller.IsAuthorizedAsAdmin())
                result = new UnauthorizedResult();
            else
                result = await controller.PutCurvePoint(1, invalidCurvePoint);

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
        public async Task PutCurvePoint_AsDifferentRoles_WhenFocusingWrongId_ReturnsBadRequest(string? role)
        {
            // Arrange
            var repo = Substitute.For<ICurvePointRepository>();
            repo
                .GetByIdAsync(1)
                .Returns(new CurvePointDto { Id = 1 });
            var controller = new CurvePointController(repo);

            if (role == "Admin")
                controller.InitializeAdminUser();
            else if (role == "User")
                controller.InitializeRegularUser();
            else if (role == "Unauthenticated")
                controller.InitializeUnauthenticatedUser();

            var updatedCurvePoint = new CurvePointDto { Id = 2, CurveId = 20, Term = 2.0, CurvePointValue = 200.0 };

            // Act
            IActionResult result;

            // Simulate [Authorize(Roles = "Admin")] behavior in unit tests
            if (!controller.IsAuthorizedAsAdmin())
                result = new UnauthorizedResult();
            else
                result = await controller.PutCurvePoint(1, updatedCurvePoint);

            // Assert
            if (role == "Admin")
                Assert.IsType<BadRequestObjectResult>(result);
            else
                Assert.IsType<UnauthorizedResult>(result);
        }

        // Tests for DeleteCurvePoint
        [Theory]
        [InlineData("Admin")]
        [InlineData("User")]
        [InlineData("Unauthenticated")]
        public async Task DeleteCurvePoint_AsDifferentRoles_ReturnsExpected(string? role)
        {
            // Arrange
            var repo = Substitute.For<ICurvePointRepository>();
            repo
                .GetByIdAsync(1)
                .Returns(new CurvePointDto { Id = 1 });
            repo
                .DeleteAsync(1)
                .Returns(true);
            var controller = new CurvePointController(repo);
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
                result = await controller.DeleteCurvePoint(1);

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
        public async Task DeleteCurvePoint_AsDifferentRoles_ReturnsNotFound(string? role)
        {
            // Arrange
            var repo = Substitute.For<ICurvePointRepository>();
            repo
                .GetByIdAsync(1)
                .Returns((CurvePointDto?)null);
            var controller = new CurvePointController(repo);

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
                result = await controller.DeleteCurvePoint(1);

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