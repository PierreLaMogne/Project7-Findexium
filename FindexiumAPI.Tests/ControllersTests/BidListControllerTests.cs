using FindexiumAPI.Controllers;
using FindexiumAPI.Models;
using FindexiumAPI.Repositories;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;

namespace FindexiumAPI.Tests.Controllers
{
    public class BidListControllerTests
    {
        // Helper method to create a controller with a specific user role
        private BidListController CreateControllerWithRole(string? role, IBidListRepository? repo = null)
        {
            repo ??= Substitute.For<IBidListRepository>();
            var controller = new BidListController(repo);

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


        // Tests for GetBidLists
        [Theory]
        [InlineData("Admin")]
        [InlineData("User")]
        [InlineData("Unauthenticated")]
        public async Task GetBidLists_AsDifferentRoles_ReturnsExpected(string? role)
        {
            // Arrange
            var repo = Substitute.For<IBidListRepository>();
            repo.GetAllAsync().Returns(new List<BidListDto> { new() { BidListId = 1 }, new() { BidListId = 2 } }); // Simulate two BidList entries in the repository
            var controller = CreateControllerWithRole(role, repo);

            // Act
            ActionResult<IEnumerable<BidListDto>> result;

            // Simulate [Authorize(Policy = "Users")] behavior in unit tests
            if (!controller.IsAuthorizedForUsersPolicy())
                result = new ActionResult<IEnumerable<BidListDto>>(new UnauthorizedResult());
            else
                result = await controller.GetBidLists();

            // Assert
            if (role == "Admin" || role == "User")
            {
                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var bidLists = Assert.IsAssignableFrom<IEnumerable<BidListDto>>(okResult.Value);
                Assert.Equal(2, bidLists.Count());
            }
            else
                Assert.IsType<UnauthorizedResult>(result.Result);
        }

        [Theory]
        [InlineData("Admin")]
        [InlineData("User")]
        [InlineData("Unauthenticated")]
        public async Task GetBidLists_AsDifferentRoles_ReturnsNotFound(string? role)
        {
            // Arrange
            var repo = Substitute.For<IBidListRepository>();
            repo.GetAllAsync().Returns(new List<BidListDto>()); // Simulate no BidList entries in the repository
            var controller = CreateControllerWithRole(role, repo);

            // Act
            ActionResult<IEnumerable<BidListDto>> result;

            // Simulate [Authorize(Policy = "Users")] behavior in unit tests
            if (!controller.IsAuthorizedForUsersPolicy())
                result = new ActionResult<IEnumerable<BidListDto>>(new UnauthorizedResult());
            else
                result = await controller.GetBidLists();

            // Assert
            if (role == "Admin" || role == "User")
            {
                var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
                Assert.Equal("No BidList found.", notFoundResult.Value);
            }
            else
            Assert.IsType<UnauthorizedResult>(result.Result);
        }

        // Tests for GetBidList
        [Theory]
        [InlineData("Admin")]
        [InlineData("User")]
        [InlineData("Unauthenticated")]
        public async Task GetBidList_AsDifferentRoles_ReturnsExpected(string? role)
        {
            // Arrange
            var repo = Substitute.For<IBidListRepository>();
            repo.GetByIdAsync(1).Returns(new BidListDto { BidListId = 1 }); // Simulate one BidList entry in the repository
            var controller = CreateControllerWithRole(role, repo);

            // Act
            ActionResult<BidListDto> result;

            // Simulate [Authorize(Policy = "Users")] behavior in unit tests
            if (!controller.IsAuthorizedForUsersPolicy())
                result = new ActionResult<BidListDto>(new UnauthorizedResult());
            else
                result = await controller.GetBidList(1);

            // Assert
            if (role == "Admin" || role == "User")
            {
                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var bidList = Assert.IsAssignableFrom<BidListDto>(okResult.Value);
                Assert.Equal(1, bidList.BidListId);

            }
            else
                Assert.IsType<UnauthorizedResult>(result.Result);
        }

        [Theory]
        [InlineData("Admin")]
        [InlineData("User")]
        [InlineData("Unauthenticated")]
        public async Task GetBidList_AsDifferentRoles_ReturnsNotFound(string? role)
        {
            // Arrange
            var repo = Substitute.For<IBidListRepository>();
            repo.GetByIdAsync(1).Returns((BidListDto?)null); // Simulate no BidList entry with the specified ID
            var controller = CreateControllerWithRole(role, repo);

            // Act
            ActionResult<BidListDto> result;

            // Simulate [Authorize(Policy = "Users")] behavior in unit tests
            if (!controller.IsAuthorizedForUsersPolicy())
                result = new ActionResult<BidListDto>(new UnauthorizedResult());
            else
                result = await controller.GetBidList(1);

            // Assert
            if (role == "Admin" || role == "User")
            {
                var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
                Assert.Equal("The Id mentioned does not exist.", notFoundResult.Value);
            }
            else
                Assert.IsType<UnauthorizedResult>(result.Result);
        }


        // Tests for PostBidList
        [Theory]
        [InlineData("Admin")]
        [InlineData("User")]
        [InlineData("Unauthenticated")]
        public async Task PostBidList_AsDifferentRoles_ReturnsExcepted(string? role)
        {
            // Arrange
            var repo = Substitute.For<IBidListRepository>();
            repo.AddAsync(Arg.Any<BidListDto>()).Returns(callInfo => 
                    {
                        var dto = callInfo.Arg<BidListDto>();
                        dto.BidListId = 1;
                        return dto;
                    }); // Simulate adding a new BidList entry to the repository and returning it with an assigned ID
            var controller = CreateControllerWithRole(role, repo);

            var newBidList = new BidListDto { Account = "TestAccount", BidType = "TestType", BidQuantity = 100 };

            // Act
            ActionResult<BidListDto> result;

            // Simulate [Authorize(Roles = "Admin")] behavior in unit tests
            if (!controller.IsAuthorizedAsAdmin())
                result = new ActionResult<BidListDto>(new UnauthorizedResult());
            else
                result = await controller.PostBidList(newBidList);

            // Assert
            if (role == "Admin")
            {
                var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
                var createdBidList = Assert.IsAssignableFrom<BidListDto>(createdAtActionResult.Value);
                Assert.Equal(1, createdBidList.BidListId);
                Assert.Equal("TestAccount", createdBidList.Account);
                Assert.Equal("TestType", createdBidList.BidType);
                Assert.Equal(100, createdBidList.BidQuantity);
            }
            else
                Assert.IsType<UnauthorizedResult>(result.Result);
        }

        [Theory]
        [InlineData("Admin")]
        [InlineData("User")]
        [InlineData("Unauthenticated")]
        public async Task PostBidList_AsDifferentRoles_WhenModelStateIsNotValid_ReturnsBadRequest(string? role)
        {
            // Arrange
            var repo = Substitute.For<IBidListRepository>();
            var controller = CreateControllerWithRole(role, repo);

            var invalidBidList = new BidListDto { Account = "", BidType = "", BidQuantity = null };

            controller.ModelState.AddModelError("Account", "Account is required.");
            controller.ModelState.AddModelError("BidType", "BidType is required.");
            controller.ModelState.AddModelError("BidQuantity", "BidQuantity is required.");

            // Act
            ActionResult<BidListDto> result;

            // Simulate [Authorize(Roles = "Admin")] behavior in unit tests
            if (!controller.IsAuthorizedAsAdmin())
                result = new ActionResult<BidListDto>(new UnauthorizedResult());
            else
                result = await controller.PostBidList(invalidBidList);

            // Assert
            if (role == "Admin")
                Assert.IsType<BadRequestObjectResult>(result.Result);
            else
                Assert.IsType<UnauthorizedResult>(result.Result);
        }

        // Tests for PutBidList
        [Theory]
        [InlineData("Admin")]
        [InlineData("User")]
        [InlineData("Unauthenticated")]
        public async Task PutBidList_AsDifferentRoles_ReturnsExpected(string? role)
        {
            // Arrange
            var repo = Substitute.For<IBidListRepository>();
            repo.GetByIdAsync(1).Returns(new BidListDto { BidListId = 1 }); // Simulate an existing BidList entry with the specified ID in the repository
            repo.UpdateAsync(1, Arg.Any<BidListDto>()).Returns(true); // Simulate updating an existing BidList entry in the repository successfully
            var controller = CreateControllerWithRole(role, repo);

            var updatedBidList = new BidListDto { BidListId = 1, Account = "UpdatedAccount", BidType = "UpdatedType", BidQuantity = 200 };

            // Act
            IActionResult result;

            // Simulate [Authorize(Roles = "Admin")] behavior in unit tests
            if (!controller.IsAuthorizedAsAdmin())
                result = new UnauthorizedResult();
            else
                result = await controller.PutBidList(1, updatedBidList);

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
        public async Task PutBidList_AsDifferentRoles_ReturnsNotFound(string? role)
        {
            // Arrange
            var repo = Substitute.For<IBidListRepository>();
            repo.GetByIdAsync(1).Returns((BidListDto?)null); // Simulate no existing BidList entry with the specified ID in the repository
            var controller = CreateControllerWithRole(role, repo);

            var updatedBidList = new BidListDto { BidListId = 1, Account = "UpdatedAccount", BidType = "UpdatedType", BidQuantity = 200 };

            // Act
            IActionResult result;

            // Simulate [Authorize(Roles = "Admin")] behavior in unit tests
            if (!controller.IsAuthorizedAsAdmin())
                result = new UnauthorizedResult();
            else
                result = await controller.PutBidList(1, updatedBidList);

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
        public async Task PutBidList_AsDifferentRoles_WhenModelStateIsNotValid_ReturnsBadRequest(string? role)
        {
            // Arrange
            var repo = Substitute.For<IBidListRepository>();
            repo.GetByIdAsync(1).Returns(new BidListDto { BidListId = 1 }); // Simulate an existing BidList entry with the specified ID in the repository
            var controller = CreateControllerWithRole(role, repo);

            var invalidBidList = new BidListDto { BidListId = 1, Account = "", BidType = "", BidQuantity = null };

            controller.ModelState.AddModelError("Account", "Account is required.");
            controller.ModelState.AddModelError("BidType", "BidType is required.");
            controller.ModelState.AddModelError("BidQuantity", "BidQuantity is required.");

            // Act
            IActionResult result;

            // Simulate [Authorize(Roles = "Admin")] behavior in unit tests
            if (!controller.IsAuthorizedAsAdmin())
                result = new UnauthorizedResult();
            else
                result = await controller.PutBidList(1, invalidBidList);

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
        public async Task PutBidList_AsDifferentRoles_WhenFocusingWrongId_ReturnsBadRequest(string? role)
        {
            // Arrange
            var repo = Substitute.For<IBidListRepository>();
            repo.GetByIdAsync(1).Returns(new BidListDto { BidListId = 1 }); // Simulate an existing BidList entry with the specified ID in the repository
            var controller = CreateControllerWithRole(role, repo);

            var updatedBidList = new BidListDto { BidListId = 2, Account = "UpdatedAccount", BidType = "UpdatedType", BidQuantity = 200 };

            // Act
            IActionResult result;

            // Simulate [Authorize(Roles = "Admin")] behavior in unit tests
            if (!controller.IsAuthorizedAsAdmin())
                result = new UnauthorizedResult();
            else
                result = await controller.PutBidList(1, updatedBidList);

            // Assert
            if (role == "Admin")
                Assert.IsType<BadRequestObjectResult>(result);
            else
                Assert.IsType<UnauthorizedResult>(result);
        }

        // Tests for DeleteBidList
        [Theory]
        [InlineData("Admin")]
        [InlineData("User")]
        [InlineData("Unauthenticated")]
        public async Task DeleteBidList_AsDifferentRoles_ReturnsExpected(string? role)
        {
            // Arrange
            var repo = Substitute.For<IBidListRepository>();
            repo.GetByIdAsync(1).Returns(new BidListDto { BidListId = 1 }); // Simulate an existing BidList entry with the specified ID in the repository
            repo.DeleteAsync(1).Returns(true); // Simulate deleting an existing BidList entry from the repository successfully
            var controller = CreateControllerWithRole(role, repo);

            // Act
            IActionResult result;

            // Simulate [Authorize(Roles = "Admin")] behavior in unit tests
            if (!controller.IsAuthorizedAsAdmin())
                result = new UnauthorizedResult();
            else
                result = await controller.DeleteBidList(1);

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
        public async Task DeleteBidList_AsDifferentRoles_ReturnsNotFound(string? role)
        {
            // Arrange
            var repo = Substitute.For<IBidListRepository>();
            repo.GetByIdAsync(1).Returns((BidListDto?)null); // Simulate no existing BidList entry with the specified ID in the repository
            var controller = CreateControllerWithRole(role, repo);

            // Act
            IActionResult result;

            // Simulate [Authorize(Roles = "Admin")] behavior in unit tests
            if (!controller.IsAuthorizedAsAdmin())
                result = new UnauthorizedResult();
            else
                result = await controller.DeleteBidList(1);

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