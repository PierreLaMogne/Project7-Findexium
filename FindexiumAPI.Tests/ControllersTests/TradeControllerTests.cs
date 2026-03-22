using FindexiumAPI.Controllers;
using FindexiumAPI.Models;
using FindexiumAPI.Repositories;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;

namespace FindexiumAPI.Tests.Controllers
{
    public class TradeControllerTests
    {
        // Helper method to create a controller with a specific user role
        private TradeController CreateControllerWithRole(string? role, ITradeRepository? repo = null)
        {
            repo ??= Substitute.For<ITradeRepository>();
            var controller = new TradeController(repo);

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


        // Tests for GetTrades
        [Theory]
        [InlineData("Admin")]
        [InlineData("User")]
        [InlineData("Unauthenticated")]
        public async Task GetTrades_AsDifferentRoles_ReturnsExpected(string? role)
        {
            // Arrange
            var repo = Substitute.For<ITradeRepository>();
            repo.GetAllAsync().Returns(new List<TradeDto> { new TradeDto { TradeId = 1 }, new TradeDto { TradeId = 2 } }); // Mocking the repository to return a list of trades
            var controller = CreateControllerWithRole (role, repo);

            // Act
            ActionResult<IEnumerable<TradeDto>> result;

            // Simulate [Authorize(Policy = "Users")] behavior in unit tests
            if (!controller.IsAuthorizedForUsersPolicy())
                result = new ActionResult<IEnumerable<TradeDto>>(new UnauthorizedResult());
            else
                result = await controller.GetTrades();

            // Assert
            if (role == "Admin" || role == "User")
            {
                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var trades = Assert.IsAssignableFrom<IEnumerable<TradeDto>>(okResult.Value);
                Assert.Equal(2, trades.Count());
            }
            else
                Assert.IsType<UnauthorizedResult>(result.Result);    
        }

        [Theory]
        [InlineData("Admin")]
        [InlineData("User")]
        [InlineData("Unauthenticated")]
        public async Task GetTrades_AsDifferentRoles_ReturnsNotFound(string? role)
        {
            // Arrange
            var repo = Substitute.For<ITradeRepository>();
            repo.GetAllAsync().Returns(new List<TradeDto>()); // Mocking the repository to return an empty list of trades
            var controller = CreateControllerWithRole(role, repo);

            // Act
            ActionResult<IEnumerable<TradeDto>> result;

            // Simulate [Authorize(Policy = "Users")] behavior in unit tests
            if (!controller.IsAuthorizedForUsersPolicy())
                result = new ActionResult<IEnumerable<TradeDto>>(new UnauthorizedResult());
            else
                result = await controller.GetTrades();

            // Assert
            if (role == "Admin" || role == "User")
            {
                var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
                Assert.Equal("No Trade found.", notFoundResult.Value);
            }
            else
                Assert.IsType<UnauthorizedResult>(result.Result);
        }

        // Tests for GetTrade
        [Theory]
        [InlineData("Admin")]
        [InlineData("User")]
        [InlineData("Unauthenticated")]
        public async Task GetTrade_AsDifferentRoles_ReturnsExpected(string? role)
        {
            // Arrange
            var repo = Substitute.For<ITradeRepository>();
            repo.GetByIdAsync(1).Returns(new TradeDto { TradeId = 1 }); // Mocking the repository to return a trade for the specified ID
            var controller = CreateControllerWithRole(role, repo);

            // Act
            ActionResult<TradeDto> result;

            // Simulate [Authorize(Policy = "Users")] behavior in unit tests
            if (!controller.IsAuthorizedForUsersPolicy())
                result = new ActionResult<TradeDto>(new UnauthorizedResult());
            else
                result = await controller.GetTrade(1);

            // Assert
            if (role == "Admin" ||role == "User")
            {
                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var trade = Assert.IsAssignableFrom<TradeDto>(okResult.Value);
                Assert.Equal(1, trade.TradeId);
            }
            else
                Assert.IsType<UnauthorizedResult>(result.Result);
        }

        [Theory]
        [InlineData("Admin")]
        [InlineData("User")]
        [InlineData("Unauthenticated")]
        public async Task GetTrade_AsDifferentRoles_ReturnsNotFound(string? role)
        {
            // Arrange
            var repo = Substitute.For<ITradeRepository>();
            repo.GetByIdAsync(1).Returns((TradeDto?)null); // Mocking the repository to return null for the specified ID
            var controller = CreateControllerWithRole(role, repo);

            // Act
            ActionResult<TradeDto> result;

            // Simulate [Authorize(Policy = "Users")] behavior in unit tests
            if (!controller.IsAuthorizedForUsersPolicy())
                result = new ActionResult<TradeDto>(new UnauthorizedResult());
            else
                result = await controller.GetTrade(1);

            // Assert
            if (role == "Admin" ||role == "User")
            {
                var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
                Assert.Equal("The Id mentioned does not exist.", notFoundResult.Value);
            }
            else
                Assert.IsType<UnauthorizedResult>(result.Result);
        }

        // Tests for PostTrade
        [Theory]
        [InlineData("Admin")]
        [InlineData("User")]
        [InlineData("Unauthenticated")]
        public async Task PostTrade_AsDifferentRoles_ReturnsExcepted(string? role)
        {
            // Arrange
            var repo = Substitute.For<ITradeRepository>();
            repo.AddAsync(Arg.Any<TradeDto>()).Returns(callInfo =>
                {
                    var dto = callInfo.Arg<TradeDto>();
                    dto.TradeId = 1;
                    return dto;
                }); // Mocking the repository to return the created trade with an assigned ID
            var controller = CreateControllerWithRole(role, repo);

            var newTrade = new TradeDto { TradeId = 1, Account = "Account1", AccountType = "Type1", BuyQuantity = 100, SellQuantity = 50, BuyPrice = 10.5, SellPrice = 20.5, TradeDate = DateTime.Now, TradeSecurity = "Security1", TradeStatus = "Status1", Trader = "Trader1", Benchmark = "Benchmark1", Book = "Book1", CreationName = "Creator1", CreationDate = DateTime.Now, RevisionName = "Reviser1", RevisionDate = DateTime.Now, DealName = "Deal1", DealType = "TypeA", SourceListId = "Source1", Side = "Buy" };

            // Act
            ActionResult<TradeDto> result;

            // Simulate [Authorize(Roles = "Admin")] behavior in unit tests
            if (!controller.IsAuthorizedAsAdmin())
                result = new ActionResult<TradeDto>(new UnauthorizedResult());
            else
                result = await controller.PostTrade(newTrade);

            // Assert
            if (role == "Admin")
            {
                var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
                var createdTrade = Assert.IsAssignableFrom<TradeDto>(createdAtActionResult.Value);
                Assert.Equal(1, createdTrade.TradeId);
                Assert.Equal("Account1", createdTrade.Account);
                Assert.Equal("Type1", createdTrade.AccountType);
                Assert.Equal(100, createdTrade.BuyQuantity);
                Assert.Equal(50, createdTrade.SellQuantity);
                Assert.Equal(10.5, createdTrade.BuyPrice);
                Assert.Equal(20.5, createdTrade.SellPrice);
                Assert.Equal("Security1", createdTrade.TradeSecurity);
                Assert.Equal("Status1", createdTrade.TradeStatus);
                Assert.Equal("Trader1", createdTrade.Trader);
                Assert.Equal("Benchmark1", createdTrade.Benchmark);
                Assert.Equal("Book1", createdTrade.Book);
                Assert.Equal("Creator1", createdTrade.CreationName);
                Assert.Equal("Reviser1", createdTrade.RevisionName);
                Assert.Equal("Deal1", createdTrade.DealName);
                Assert.Equal("TypeA", createdTrade.DealType);
                Assert.Equal("Source1", createdTrade.SourceListId);
                Assert.Equal("Buy", createdTrade.Side);
            }
            else
                Assert.IsType<UnauthorizedResult>(result.Result);
        }

        [Theory]
        [InlineData("Admin")]
        [InlineData("User")]
        [InlineData("Unauthenticated")]
        public async Task PostTrade_AsDifferentRoles_WhenModelStateIsNotValid_ReturnsBadRequest(string? role)
        {
            // Arrange
            var repo = Substitute.For<ITradeRepository>();
            var controller = CreateControllerWithRole(role, repo);

            var invalidTrade = new TradeDto { Account = "", AccountType = "", BuyQuantity = null, SellQuantity = null, BuyPrice = null, SellPrice = null, TradeDate = DateTime.Now, TradeSecurity = "", TradeStatus = "", Trader = "", Benchmark = "", Book = "", CreationName = "", CreationDate = DateTime.Now, RevisionName = "", RevisionDate = DateTime.Now, DealName = "", DealType = "", SourceListId = "", Side = "" };

            controller.ModelState.AddModelError("Account", "Account is required.");
            controller.ModelState.AddModelError("AccountType", "AccountType is required.");
            controller.ModelState.AddModelError("BuyQuantity", "BuyQuantity is required.");
            controller.ModelState.AddModelError("SellQuantity", "SellQuantity is required.");
            controller.ModelState.AddModelError("BuyPrice", "BuyPrice is required.");
            controller.ModelState.AddModelError("SellPrice", "SellPrice is required.");
            controller.ModelState.AddModelError("TradeSecurity", "TradeSecurity is required.");
            controller.ModelState.AddModelError("TradeStatus", "TradeStatus is required.");
            controller.ModelState.AddModelError("Trader", "Trader is required.");
            controller.ModelState.AddModelError("Benchmark", "Benchmark is required.");
            controller.ModelState.AddModelError("Book", "Book is required.");
            controller.ModelState.AddModelError("CreationName", "CreationName is required.");
            controller.ModelState.AddModelError("RevisionName", "RevisionName is required.");
            controller.ModelState.AddModelError("DealName", "DealName is required.");
            controller.ModelState.AddModelError("DealType", "DealType is required.");
            controller.ModelState.AddModelError("SourceListId", "SourceListId is required.");
            controller.ModelState.AddModelError("Side", "Side is required.");

            // Act
            ActionResult<TradeDto> result;

            // Simulate [Authorize(Roles = "Admin")] behavior in unit tests
            if (!controller.IsAuthorizedAsAdmin())
                result = new ActionResult<TradeDto>(new UnauthorizedResult());
            else
                result = await controller.PostTrade(invalidTrade);

            // Assert
            if (role == "Admin")
                Assert.IsType<BadRequestObjectResult>(result.Result);
            else
                Assert.IsType<UnauthorizedResult>(result.Result);
        }

        // Tests for PutTrade
        [Theory]
        [InlineData("Admin")]
        [InlineData("User")]
        [InlineData("Unauthenticated")]
        public async Task PutTrade_AsDifferentRoles_ReturnsExpected(string? role)
        {
            // Arrange
            var repo = Substitute.For<ITradeRepository>();
            repo.GetByIdAsync(1).Returns(new TradeDto { TradeId = 1 }); // Mocking the repository to return a trade for the specified ID
            repo.UpdateAsync(1, Arg.Any<TradeDto>()).Returns(true); // Mocking the repository to return true for successful update
            var controller = CreateControllerWithRole(role, repo);

            var updatedTrade = new TradeDto { TradeId = 1, Account = "Account2", AccountType = "Type2", BuyQuantity = 200, SellQuantity = 150, BuyPrice = 15.5, SellPrice = 25.5, TradeDate = DateTime.Now, TradeSecurity = "Security2", TradeStatus = "Status2", Trader = "Trader2", Benchmark = "Benchmark2", Book = "Book2", CreationName = "Creator2", CreationDate = DateTime.Now, RevisionName = "Reviser2", RevisionDate = DateTime.Now, DealName = "Deal2", DealType = "TypeB", SourceListId = "Source2", Side = "Sell" };

            // Act
            IActionResult result;

            // Simulate [Authorize(Roles = "Admin")] behavior in unit tests
            if (!controller.IsAuthorizedAsAdmin())
                result = new UnauthorizedResult();
            else
                result = await controller.PutTrade(1, updatedTrade);

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
        public async Task PutTrade_AsDifferentRoles_ReturnsNotFound(string? role)
        {
            // Arrange
            var repo = Substitute.For<ITradeRepository>();
            repo.GetByIdAsync(1).Returns((TradeDto?)null); // Mocking the repository to return null for the specified ID
            var controller = CreateControllerWithRole(role, repo);

            var updatedTrade = new TradeDto { TradeId = 1, Account = "Account2", AccountType = "Type2", BuyQuantity = 200, SellQuantity = 150, BuyPrice = 15.5, SellPrice = 25.5, TradeDate = DateTime.Now, TradeSecurity = "Security2", TradeStatus = "Status2", Trader = "Trader2", Benchmark = "Benchmark2", Book = "Book2", CreationName = "Creator2", CreationDate = DateTime.Now, RevisionName = "Reviser2", RevisionDate = DateTime.Now, DealName = "Deal2", DealType = "TypeB", SourceListId = "Source2", Side = "Sell" };
            
            // Act
            IActionResult result;

            // Simulate [Authorize(Roles = "Admin")] behavior in unit tests
            if (!controller.IsAuthorizedAsAdmin())
                result = new UnauthorizedResult();
            else
                result = await controller.PutTrade(1, updatedTrade);

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
        public async Task PutTrade_AsDifferentRoles_WhenModelStateIsNotValid_ReturnsBadRequest(string? role)
        {
            // Arrange
            var repo = Substitute.For<ITradeRepository>();
            repo.GetByIdAsync(1).Returns(new TradeDto { TradeId = 1 }); // Mocking the repository to return a trade for the specified ID
            var controller = CreateControllerWithRole(role, repo);

            var invalidTrade = new TradeDto { TradeId = 1, Account = "Account2", AccountType = "Type2", BuyQuantity = 200, SellQuantity = 150, BuyPrice = 15.5, SellPrice = 25.5, TradeDate = DateTime.Now, TradeSecurity = "Security2", TradeStatus = "Status2", Trader = "Trader2", Benchmark = "Benchmark2", Book = "Book2", CreationName = "Creator2", CreationDate = DateTime.Now, RevisionName = "Reviser2", RevisionDate = DateTime.Now, DealName = "Deal2", DealType = "TypeB", SourceListId = "Source2", Side = "Sell" };

            controller.ModelState.AddModelError("Account", "Account is required.");
            controller.ModelState.AddModelError("AccountType", "AccountType is required.");
            controller.ModelState.AddModelError("BuyQuantity", "BuyQuantity is required.");
            controller.ModelState.AddModelError("SellQuantity", "SellQuantity is required.");
            controller.ModelState.AddModelError("BuyPrice", "BuyPrice is required.");
            controller.ModelState.AddModelError("SellPrice", "SellPrice is required.");
            controller.ModelState.AddModelError("TradeSecurity", "TradeSecurity is required.");
            controller.ModelState.AddModelError("TradeStatus", "TradeStatus is required.");
            controller.ModelState.AddModelError("Trader", "Trader is required.");
            controller.ModelState.AddModelError("Benchmark", "Benchmark is required.");
            controller.ModelState.AddModelError("Book", "Book is required.");
            controller.ModelState.AddModelError("CreationName", "CreationName is required.");
            controller.ModelState.AddModelError("RevisionName", "RevisionName is required.");
            controller.ModelState.AddModelError("DealName", "DealName is required.");
            controller.ModelState.AddModelError("DealType", "DealType is required.");
            controller.ModelState.AddModelError("SourceListId", "SourceListId is required.");
            controller.ModelState.AddModelError("Side", "Side is required.");

            // Act
            IActionResult result;

            // Simulate [Authorize(Roles = "Admin")] behavior in unit tests
            if (!controller.IsAuthorizedAsAdmin())
                result = new UnauthorizedResult();
            else
                result = await controller.PutTrade(1, invalidTrade);

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
        public async Task PutTrade_AsDifferentRoles_WhenFocusingWrongId_ReturnsBadRequest(string? role)
        {
            // Arrange
            var repo = Substitute.For<ITradeRepository>();
            repo.GetByIdAsync(1).Returns(new TradeDto { TradeId = 1 }); // Mocking the repository to return a trade for the specified ID
            var controller = CreateControllerWithRole(role, repo);

            var updatedTrade = new TradeDto { TradeId = 2, Account = "Account2", AccountType = "Type2", BuyQuantity = 200, SellQuantity = 150, BuyPrice = 15.5, SellPrice = 25.5, TradeDate = DateTime.Now, TradeSecurity = "Security2", TradeStatus = "Status2", Trader = "Trader2", Benchmark = "Benchmark2", Book = "Book2", CreationName = "Creator2", CreationDate = DateTime.Now, RevisionName = "Reviser2", RevisionDate = DateTime.Now, DealName = "Deal2", DealType = "TypeB", SourceListId = "Source2", Side = "Sell" };

            // Act
            IActionResult result;

            // Simulate [Authorize(Roles = "Admin")] behavior in unit tests
            if (!controller.IsAuthorizedAsAdmin())
                result = new UnauthorizedResult();
            else
                result = await controller.PutTrade(1, updatedTrade);

            // Assert
            if (role == "Admin")
                Assert.IsType<BadRequestObjectResult>(result);
            else
                Assert.IsType<UnauthorizedResult>(result);
        }

        // Tests for DeleteTrade
        [Theory]
        [InlineData("Admin")]
        [InlineData("User")]
        [InlineData("Unauthenticated")]
        public async Task DeleteTrade_AsDifferentRoles_ReturnsExpected(string? role)
        {
            // Arrange
            var repo = Substitute.For<ITradeRepository>();
            repo.GetByIdAsync(1).Returns(new TradeDto { TradeId = 1 }); // Mocking the repository to return a trade for the specified ID
            repo.DeleteAsync(1).Returns(true); // Mocking the repository to return true for successful deletion
            var controller = CreateControllerWithRole(role, repo);

            // Act
            IActionResult result;

            // Simulate [Authorize(Roles = "Admin")] behavior in unit tests
            if (!controller.IsAuthorizedAsAdmin())
                result = new UnauthorizedResult();
            else
                result = await controller.DeleteTrade(1);

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
        public async Task DeleteTrade_AsDifferentRoles_ReturnsNotFound(string? role)
        {
            // Arrange
            var repo = Substitute.For<ITradeRepository>();
            repo.GetByIdAsync(1).Returns((TradeDto?)null); // Mocking the repository to return null for the specified ID
            var controller = CreateControllerWithRole(role, repo);

            // Act
            IActionResult result;

            // Simulate [Authorize(Roles = "Admin")] behavior in unit tests
            if (!controller.IsAuthorizedAsAdmin())
                result = new UnauthorizedResult();
            else
                result = await controller.DeleteTrade(1);

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