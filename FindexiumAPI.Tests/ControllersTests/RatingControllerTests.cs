using FindexiumAPI.Controllers;
using FindexiumAPI.Models;
using FindexiumAPI.Repositories;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;

namespace FindexiumAPI.Tests.Controllers
{
    public class RatingControllerTests
    {
        // Tests for GetRatings
        [Theory]
        [InlineData("Admin")]
        [InlineData("User")]
        [InlineData("Unauthenticated")]
        public async Task GetRatings_AsDifferentRoles_ReturnsExpected(string? role)
        {
            // Arrange
            var repo = Substitute.For<IRatingRepository>();
            repo
                .GetAllAsync()
                .Returns(new List<RatingDto> { new RatingDto { Id = 1 }, new RatingDto { Id = 2 } });
            var controller = new RatingController(repo);

            if (role == "Admin")
                controller.InitializeAdminUser();
            else if (role == "User")
                controller.InitializeRegularUser();
            else if (role == "Unauthenticated")
                controller.InitializeUnauthenticatedUser();

            // Act
            ActionResult<IEnumerable<RatingDto>> result;

            // Simulate [Authorize(Policy = "Users")] behavior in unit tests
            if (!controller.IsAuthorizedForUsersPolicy())
                result = new ActionResult<IEnumerable<RatingDto>>(new UnauthorizedResult());
            else
                result = await controller.GetRatings();

            // Assert
            if (role == "Unauthenticated")
                Assert.IsType<UnauthorizedResult>(result.Result);
            else
            {
                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var ratings = Assert.IsAssignableFrom<IEnumerable<RatingDto>>(okResult.Value);
                Assert.Equal(2, ratings.Count());
            }
        }

        [Theory]
        [InlineData("Admin")]
        [InlineData("User")]
        [InlineData("Unauthenticated")]
        public async Task GetRatings_AsDifferentRoles_ReturnsNotFound(string? role)
        {
            // Arrange
            var repo = Substitute.For<IRatingRepository>();
            repo
                .GetAllAsync()
                .Returns(new List<RatingDto>());
            var controller = new RatingController(repo);

            if (role == "Admin")
                controller.InitializeAdminUser();
            else if (role == "User")
                controller.InitializeRegularUser();
            else if (role == "Unauthenticated")
                controller.InitializeUnauthenticatedUser();

            // Act
            ActionResult<IEnumerable<RatingDto>> result;

            // Simulate [Authorize(Policy = "Users")] behavior in unit tests
            if (!controller.IsAuthorizedForUsersPolicy())
                result = new ActionResult<IEnumerable<RatingDto>>(new UnauthorizedResult());
            else
                result = await controller.GetRatings();

            // Assert
            if (role == "Unauthenticated")
                Assert.IsType<UnauthorizedResult>(result.Result);
            else
            {
                var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
                Assert.Equal("No Rating found.", notFoundResult.Value);
            }
        }

        // Tests for GetRating
        [Theory]
        [InlineData("Admin")]
        [InlineData("User")]
        [InlineData("Unauthenticated")]
        public async Task GetRating_AsDifferentRoles_ReturnsExpected(string? role)
        {
            // Arrange
            var repo = Substitute.For<IRatingRepository>();
            repo
                .GetByIdAsync(1)
                .Returns(new RatingDto { Id = 1 });
            var controller = new RatingController(repo);

            if (role == "Admin")
                controller.InitializeAdminUser();
            else if (role == "User")
                controller.InitializeRegularUser();
            else if (role == "Unauthenticated")
                controller.InitializeUnauthenticatedUser();

            // Act
            ActionResult<RatingDto> result;

            // Simulate [Authorize(Policy = "Users")] behavior in unit tests
            if (!controller.IsAuthorizedForUsersPolicy())
                result = new ActionResult<RatingDto>(new UnauthorizedResult());
            else
                result = await controller.GetRating(1);

            // Assert
            if (role == "Unauthenticated")
                Assert.IsType<UnauthorizedResult>(result.Result);
            else
            {
                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var rating = Assert.IsAssignableFrom<RatingDto>(okResult.Value);
                Assert.Equal(1, rating.Id);

            }
        }

        [Theory]
        [InlineData("Admin")]
        [InlineData("User")]
        [InlineData("Unauthenticated")]
        public async Task GetRating_AsDifferentRoles_ReturnsNotFound(string? role)
        {
            // Arrange
            var repo = Substitute.For<IRatingRepository>();
            repo
                .GetByIdAsync(1)
                .Returns((RatingDto?)null);
            var controller = new RatingController(repo);

            if (role == "Admin")
                controller.InitializeAdminUser();
            else if (role == "User")
                controller.InitializeRegularUser();
            else if (role == "Unauthenticated")
                controller.InitializeUnauthenticatedUser();

            // Act
            ActionResult<RatingDto> result;

            // Simulate [Authorize(Policy = "Users")] behavior in unit tests
            if (!controller.IsAuthorizedForUsersPolicy())
                result = new ActionResult<RatingDto>(new UnauthorizedResult());
            else
                result = await controller.GetRating(1);

            // Assert
            if (role == "Unauthenticated")
                Assert.IsType<UnauthorizedResult>(result.Result);
            else
            {
                var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
                Assert.Equal("The Id mentioned does not exist.", notFoundResult.Value);
            }
        }

        // Tests for PostRating
        [Theory]
        [InlineData("Admin")]
        [InlineData("User")]
        [InlineData("Unauthenticated")]
        public async Task PostRating_AsDifferentRoles_ReturnsExcepted(string? role)
        {
            // Arrange
            var repo = Substitute.For<IRatingRepository>();
            repo
                .AddAsync(Arg.Any<RatingDto>())
                .Returns(callInfo =>
                {
                    var dto = callInfo.Arg<RatingDto>();
                    dto.Id = 1;
                    return dto;
                });
            var controller = new RatingController(repo);

            if (role == "Admin")
                controller.InitializeAdminUser();
            else if (role == "User")
                controller.InitializeRegularUser();
            else if (role == "Unauthenticated")
                controller.InitializeUnauthenticatedUser();

            var newRating = new RatingDto { Id = 1, MoodysRating = "Aaa", SandPRating = "AAA", FitchRating = "AAA", OrderNumber = 1 };

            // Act
            ActionResult<RatingDto> result;

            // Simulate [Authorize(Roles = "Admin")] behavior in unit tests
            if (!controller.IsAuthorizedAsAdmin())
                result = new ActionResult<RatingDto>(new UnauthorizedResult());
            else
                result = await controller.PostRating(newRating);

            // Assert
            if (role == "Admin")
            {
                var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
                var createdRating = Assert.IsAssignableFrom<RatingDto>(createdAtActionResult.Value);
                Assert.Equal(1, createdRating.Id);
                Assert.Equal("Aaa", createdRating.MoodysRating);
                Assert.Equal("AAA", createdRating.SandPRating);
                Assert.Equal("AAA", createdRating.FitchRating);
                Assert.Equal((byte)1, createdRating.OrderNumber);
            }
            else
                Assert.IsType<UnauthorizedResult>(result.Result);
        }

        [Theory]
        [InlineData("Admin")]
        [InlineData("User")]
        [InlineData("Unauthenticated")]
        public async Task PostRating_AsDifferentRoles_WhenModelStateIsNotValid_ReturnsBadRequest(string? role)
        {
            // Arrange
            var repo = Substitute.For<IRatingRepository>();
            var controller = new RatingController(repo);

            if (role == "Admin")
                controller.InitializeAdminUser();
            else if (role == "User")
                controller.InitializeRegularUser();
            else if (role == "Unauthenticated")
                controller.InitializeUnauthenticatedUser();

            var invalidRating = new RatingDto { MoodysRating = "", SandPRating = "", FitchRating = "", OrderNumber = null };

            controller.ModelState.AddModelError("MoodysRating", "MoodysRating is required.");
            controller.ModelState.AddModelError("SandPRating", "SandPRating is required.");
            controller.ModelState.AddModelError("FitchRating", "FitchRating is required.");
            controller.ModelState.AddModelError("OrderNumber", "OrderNumber is required.");

            // Act
            ActionResult<RatingDto> result;

            // Simulate [Authorize(Roles = "Admin")] behavior in unit tests
            if (!controller.IsAuthorizedAsAdmin())
                result = new ActionResult<RatingDto>(new UnauthorizedResult());
            else
                result = await controller.PostRating(invalidRating);

            // Assert
            if (role == "Admin")
                Assert.IsType<BadRequestObjectResult>(result.Result);
            else
                Assert.IsType<UnauthorizedResult>(result.Result);
        }

        // Tests for PutRating
        [Theory]
        [InlineData("Admin")]
        [InlineData("User")]
        [InlineData("Unauthenticated")]
        public async Task PutRating_AsDifferentRoles_ReturnsExpected(string? role)
        {
            // Arrange
            var repo = Substitute.For<IRatingRepository>();
            repo
                .GetByIdAsync(1)
                .Returns(new RatingDto { Id = 1 });
            repo
                .UpdateAsync(1, Arg.Any<RatingDto>())
                .Returns(true);
            var controller = new RatingController(repo);

            if (role == "Admin")
                controller.InitializeAdminUser();
            else if (role == "User")
                controller.InitializeRegularUser();
            else if (role == "Unauthenticated")
                controller.InitializeUnauthenticatedUser();

            var updatedRating = new RatingDto { Id = 1, MoodysRating = "Bbb", SandPRating = "BBB", FitchRating = "BBB", OrderNumber = 2 };

            // Act
            IActionResult result;

            // Simulate [Authorize(Roles = "Admin")] behavior in unit tests
            if (!controller.IsAuthorizedAsAdmin())
                result = new UnauthorizedResult();
            else
                result = await controller.PutRating(1, updatedRating);

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
        public async Task PutRating_AsDifferentRoles_ReturnsNotFound(string? role)
        {
            // Arrange
            var repo = Substitute.For<IRatingRepository>();
            repo
                .GetByIdAsync(1)
                .Returns((RatingDto?)null);
            var controller = new RatingController(repo);
            if (role == "Admin")
                controller.InitializeAdminUser();
            else if (role == "User")
                controller.InitializeRegularUser();
            else if (role == "Unauthenticated")
                controller.InitializeUnauthenticatedUser();
            var updatedRating = new RatingDto { Id = 1, MoodysRating = "Bbb", SandPRating = "BBB", FitchRating = "BBB", OrderNumber = 2 };
            // Act
            IActionResult result;

            // Simulate [Authorize(Roles = "Admin")] behavior in unit tests
            if (!controller.IsAuthorizedAsAdmin())
                result = new UnauthorizedResult();
            else
                result = await controller.PutRating(1, updatedRating);

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
        public async Task PutRating_AsDifferentRoles_WhenModelStateIsNotValid_ReturnsBadRequest(string? role)
        {
            // Arrange
            var repo = Substitute.For<IRatingRepository>();
            repo
                .GetByIdAsync(1)
                .Returns(new RatingDto { Id = 1 });
            var controller = new RatingController(repo);

            if (role == "Admin")
                controller.InitializeAdminUser();
            else if (role == "User")
                controller.InitializeRegularUser();
            else if (role == "Unauthenticated")
                controller.InitializeUnauthenticatedUser();

            var invalidRating = new RatingDto { Id = 1, MoodysRating = "Bbb", SandPRating = "BBB", FitchRating = "BBB", OrderNumber = 2 };

            controller.ModelState.AddModelError("CurveId", "CurveId is required.");
            controller.ModelState.AddModelError("Term", "Term is required.");
            controller.ModelState.AddModelError("RatingValue", "RatingValue is required.");

            // Act
            IActionResult result;

            // Simulate [Authorize(Roles = "Admin")] behavior in unit tests
            if (!controller.IsAuthorizedAsAdmin())
                result = new UnauthorizedResult();
            else
                result = await controller.PutRating(1, invalidRating);

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
        public async Task PutRating_AsDifferentRoles_WhenFocusingWrongId_ReturnsBadRequest(string? role)
        {
            // Arrange
            var repo = Substitute.For<IRatingRepository>();
            repo
                .GetByIdAsync(1)
                .Returns(new RatingDto { Id = 1 });
            var controller = new RatingController(repo);

            if (role == "Admin")
                controller.InitializeAdminUser();
            else if (role == "User")
                controller.InitializeRegularUser();
            else if (role == "Unauthenticated")
                controller.InitializeUnauthenticatedUser();

            var updatedRating = new RatingDto { Id = 2, MoodysRating = "Bbb", SandPRating = "BBB", FitchRating = "BBB", OrderNumber = 2 };

            // Act
            IActionResult result;

            // Simulate [Authorize(Roles = "Admin")] behavior in unit tests
            if (!controller.IsAuthorizedAsAdmin())
                result = new UnauthorizedResult();
            else
                result = await controller.PutRating(1, updatedRating);

            // Assert
            if (role == "Admin")
                Assert.IsType<BadRequestObjectResult>(result);
            else
                Assert.IsType<UnauthorizedResult>(result);
        }

        // Tests for DeleteRating
        [Theory]
        [InlineData("Admin")]
        [InlineData("User")]
        [InlineData("Unauthenticated")]
        public async Task DeleteRating_AsDifferentRoles_ReturnsExpected(string? role)
        {
            // Arrange
            var repo = Substitute.For<IRatingRepository>();
            repo
                .GetByIdAsync(1)
                .Returns(new RatingDto { Id = 1 });
            repo
                .DeleteAsync(1)
                .Returns(true);
            var controller = new RatingController(repo);
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
                result = await controller.DeleteRating(1);

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
        public async Task DeleteRating_AsDifferentRoles_ReturnsNotFound(string? role)
        {
            // Arrange
            var repo = Substitute.For<IRatingRepository>();
            repo
                .GetByIdAsync(1)
                .Returns((RatingDto?)null);
            var controller = new RatingController(repo);

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
                result = await controller.DeleteRating(1);

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