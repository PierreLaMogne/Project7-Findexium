using FindexiumAPI.Data;
using FindexiumAPI.Domain;
using FindexiumAPI.Models;
using FindexiumAPI.Repositories;
using FindexiumAPI.Tests.TestData;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FindexiumAPI.Tests.RepositoriesTests
{
    public class TradeRepositoryTests : IDisposable
    {
        private LocalDbContext _context;
        private readonly ITradeRepository _repository;

        public TradeRepositoryTests()
        {
            // Create a new in-memory database for each test
            var options = new DbContextOptionsBuilder<LocalDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new LocalDbContext(options);
            _repository = new TradeRepository(_context);
        }


        // Testing GetAllAsync
        [Theory]
        [MemberData(nameof(TradeTestData.GetTradesScenarios), MemberType = typeof(TradeTestData))]
        public async Task GetAllAsync_ShouldReturnExpectedDtos(List<Trade> testData)
        {
            // Arrange
            _context.Database.EnsureCreated();
            await _context.Trades.AddRangeAsync(testData);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(testData.Count);
            result.Should().BeEquivalentTo(
                testData,
                options => options
                    .ExcludingMissingMembers()
            );
        }


        // Testing GetByIdAsync
        [Theory]
        [MemberData(nameof(TradeTestData.GetTradesScenarios), MemberType = typeof(TradeTestData))]
        public async Task GetByIdAsync_ShouldReturnExpectedDto(List<Trade> testData)
        {
            // Arrange
            _context.Database.EnsureCreated();
            await _context.Trades.AddRangeAsync(testData);
            await _context.SaveChangesAsync();

            if (testData.Count == 0)
            {
                return; // Skip the test if there's no data to test
            }

            // Act & Assert
            foreach (var trade in testData)
            {
                var result = await _repository.GetByIdAsync(trade.TradeId);
                result.Should().NotBeNull();
                result.Should().BeEquivalentTo(
                    trade,
                    options => options
                        .ExcludingMissingMembers()
                );
            }

        }

        [Theory]
        [MemberData(nameof(TradeTestData.GetTradesScenarios), MemberType = typeof(TradeTestData))]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound(List<Trade> testData)
        {
            // Arrange
            _context.Database.EnsureCreated();
            await _context.Trades.AddRangeAsync(testData);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByIdAsync(-1); // Use an ID that doesn't exist

            // Assert
            result.Should().BeNull();
        }


        // Testing CreateAsync
        [Fact]
        public async Task CreateAsync_ShouldAddNewTrade()
        {
            // Arrange
            _context.Database.EnsureCreated();

            var testDto = new TradeDto
            {
                Account = "Account1",
                AccountType = "Type1",
                BuyQuantity = 100,
                SellQuantity = 50,
                BuyPrice = 10.5,
                SellPrice = 20.5,
                TradeDate = DateTime.Now,
                TradeSecurity = "Security1",
                TradeStatus = "Status1",
                Trader = "Trader1",
                Benchmark = "Benchmark1",
                Book = "Book1",
                CreationName = "Creator1",
                CreationDate = DateTime.Now,
                RevisionName = "Reviser1",
                RevisionDate = DateTime.Now,
                DealName = "Deal1",
                DealType = "TypeA",
                SourceListId = "Source1",
                Side = "Buy"
            };

            // Act
            var createdTrade = await _repository.AddAsync(testDto);

            // Assert
            createdTrade.Should().NotBeNull();
            createdTrade.TradeId.Should().BePositive();
            createdTrade.Should().BeEquivalentTo(testDto, options => options
             .Excluding(b => b.TradeId)
             .ExcludingMissingMembers()
            );

            var dbCount = await _context.Trades.CountAsync();
            dbCount.Should().Be(1);
        }


        // Testing UpdateAsync
        [Theory]
        [MemberData(nameof(TradeTestData.GetTradesScenarios), MemberType = typeof(TradeTestData))]
        public async Task UpdateAsync_ShouldModifyExistingTrade(List<Trade> testData)
        {
            // Arrange
            _context.Database.EnsureCreated();
            await _context.Trades.AddRangeAsync(testData);
            await _context.SaveChangesAsync();

            if (testData.Count == 0)
            {
                return; // Skip the test if there's no data to test
            }

            var existingTrade = testData.First();
            var updateDto = new TradeDto
            {
                Account = "Account3",
                AccountType = "Type3",
                BuyQuantity = 300,
                SellQuantity = 250,
                BuyPrice = 20.5,
                SellPrice = 30.5,
                TradeDate = DateTime.Now,
                TradeSecurity = "Security3",
                TradeStatus = "Status3",
                Trader = "Trader3",
                Benchmark = "Benchmark3",
                Book = "Book3",
                CreationName = "Creator3",
                CreationDate = DateTime.Now,
                RevisionName = "Reviser3",
                RevisionDate = DateTime.Now,
                DealName = "Deal3",
                DealType = "TypeC",
                SourceListId = "Source3",
                Side = "Buy"
            };

            // Act
            var result = await _repository.UpdateAsync(existingTrade.TradeId, updateDto);

            // Assert
            result.Should().Be(true);
            var updatedTrade = await _context.Trades.FindAsync(existingTrade.TradeId);
            updatedTrade.Should().NotBeNull();
            updatedTrade.Should().BeEquivalentTo(updateDto, options => options
             .Excluding(b => b.TradeId)
             .ExcludingMissingMembers()
            );
        }

        [Theory]
        [MemberData(nameof(TradeTestData.GetTradesScenarios), MemberType = typeof(TradeTestData))]
        public async Task UpdateAsync_ShouldReturnFalse_WhenNotFound(List<Trade> testData)
        {
            // Arrange
            _context.Database.EnsureCreated();
            await _context.Trades.AddRangeAsync(testData);
            await _context.SaveChangesAsync();

            var updateDto = new TradeDto
            {
                Account = "Account3",
                AccountType = "Type3",
                BuyQuantity = 300,
                SellQuantity = 250,
                BuyPrice = 20.5,
                SellPrice = 30.5,
                TradeDate = DateTime.Now,
                TradeSecurity = "Security3",
                TradeStatus = "Status3",
                Trader = "Trader3",
                Benchmark = "Benchmark3",
                Book = "Book3",
                CreationName = "Creator3",
                CreationDate = DateTime.Now,
                RevisionName = "Reviser3",
                RevisionDate = DateTime.Now,
                DealName = "Deal3",
                DealType = "TypeC",
                SourceListId = "Source3",
                Side = "Buy"
            };

            // Act
            var result = await _repository.UpdateAsync(-1, updateDto); // Use an ID that doesn't exist

            // Assert
            result.Should().Be(false);
        }


        // Testing DeleteAsync
        [Theory]
        [MemberData(nameof(TradeTestData.GetTradesScenarios), MemberType = typeof(TradeTestData))]
        public async Task DeleteAsync_ShouldRemoveTrade(List<Trade> testData)
        {
            // Arrange
            _context.Database.EnsureCreated();
            await _context.Trades.AddRangeAsync(testData);
            await _context.SaveChangesAsync();

            if (testData.Count == 0)
            {
                return; // Skip the test if there's no data to test
            }

            var existingTrade = testData.First();

            // Act
            var result = await _repository.DeleteAsync(existingTrade.TradeId);

            // Assert
            result.Should().Be(true);
            var deletedTrade = await _context.Trades.FindAsync(existingTrade.TradeId);
            deletedTrade.Should().BeNull();
        }

        [Theory]
        [MemberData(nameof(TradeTestData.GetTradesScenarios), MemberType = typeof(TradeTestData))]
        public async Task DeleteAsync_ShouldReturnFalse_WhenNotFound(List<Trade> testData)
        {
            // Arrange
            _context.Database.EnsureCreated();
            await _context.Trades.AddRangeAsync(testData);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.DeleteAsync(-1); // Use an ID that doesn't exist

            // Assert
            result.Should().Be(false);
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
