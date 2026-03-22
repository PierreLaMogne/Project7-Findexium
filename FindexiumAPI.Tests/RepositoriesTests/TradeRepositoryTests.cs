using FindexiumAPI.Data;
using FindexiumAPI.Domain;
using FindexiumAPI.Models;
using FindexiumAPI.Repositories;
using FindexiumAPI.Tests.TestData;
using FindexiumAPI.Tests.TestUtilities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FindexiumAPI.Tests.RepositoriesTests
{
    public class TradeRepositoryTests : IClassFixture<LocalDbFixture>, IDisposable
    {
        private readonly LocalDbFixture _fixture;
        private readonly ITradeRepository _repository;

        // The constructor initializes the repository with the in-memory database context provided by the fixture.
        // It also ensures that the database is cleared before each test run to maintain test isolation.
        public TradeRepositoryTests(LocalDbFixture fixture)
        {
            _fixture = fixture;
            _repository = new TradeRepository(_fixture.Context);
            ClearDatabase();
        }

        private void ClearDatabase()
        {
            _fixture.Context.Trades.RemoveRange(_fixture.Context.Trades);
            _fixture.Context.SaveChanges();
        }

        // The SeedAsync method is a helper function that populates the in-memory database with test data before each test case.
        // It first clears the database to ensure a clean state, then adds the provided list of Trade entities and saves the changes.
        private async Task SeedAsync(List<Trade> data)
        {
            ClearDatabase();
            await _fixture.Context.Trades.AddRangeAsync(data);
            await _fixture.Context.SaveChangesAsync();
        }


        // Testing GetAllAsync
        [Theory]
        [MemberData(nameof(TradeTestData.GetTradesScenarios), MemberType = typeof(TradeTestData))]
        public async Task GetAllAsync_ShouldReturnExpectedDtos(List<Trade> testData)
        {
            await SeedAsync(testData);
            var result = await _repository.GetAllAsync();

            result.Should().HaveCount(testData.Count);
            if (testData.Count > 0)
            {
                result.Should().BeEquivalentTo(
                    testData,
                    options => options.ExcludingMissingMembers()
                );
            }
            else
                result.Should().BeEmpty();
        }

        // Testing GetByIdAsync - Existing
        [Theory]
        [MemberData(nameof(TradeTestData.GetTradesScenarios), MemberType = typeof(TradeTestData))]
        public async Task GetByIdAsync_ShouldReturnExpectedDto(List<Trade> testData)
        {
            await SeedAsync(testData);

            foreach (var trade in testData.Where(b => b.TradeId > 0))
            {
                var result = await _repository.GetByIdAsync(trade.TradeId);
                result.Should().NotBeNull();
                result.Should().BeEquivalentTo(
                    trade,
                    options => options.ExcludingMissingMembers()
                );
            }
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound()
        {
            await SeedAsync(new List<Trade>());
            var result = await _repository.GetByIdAsync(-1);
            result.Should().BeNull();
        }

        // Testing CreateAsync
        [Theory]
        [MemberData(nameof(TradeTestData.GetTradeDtosForCreate), MemberType = typeof(TradeTestData))]
        public async Task CreateAsync_ShouldAddNewTrade(TradeDto testDto)
        {
            await SeedAsync(new List<Trade>());

            var createdTrade = await _repository.AddAsync(testDto);

            createdTrade.Should().NotBeNull();
            createdTrade.TradeId.Should().BePositive();
            createdTrade.Should().BeEquivalentTo(testDto, options => options
                .Excluding(b => b.TradeId)
                .ExcludingMissingMembers()
            );

            var dbCount = await _fixture.Context.Trades.CountAsync();
            dbCount.Should().Be(1);
        }

        // Testing UpdateAsync
        [Theory]
        [MemberData(nameof(TradeTestData.GetTradesScenarios), MemberType = typeof(TradeTestData))]
        public async Task UpdateAsync_ShouldModifyExistingTrade(List<Trade> testData)
        {
            await SeedAsync(testData);
            var existingTrade = testData.FirstOrDefault(b => b.TradeId > 0);

            if (existingTrade == null) return;

            var updateDto = new TradeDto
            {
                Account = "Account2",
                AccountType = "Type2",
                BuyQuantity = 200,
                SellQuantity = 150,
                BuyPrice = 15.5,
                SellPrice = 25.5,
                TradeDate = DateTime.Now,
                TradeSecurity = "Security2",
                TradeStatus = "Status2",
                Trader = "Trader2",
                Benchmark = "Benchmark2",
                Book = "Book2",
                CreationName = "Creator2",
                CreationDate = DateTime.Now,
                RevisionName = "Reviser2",
                RevisionDate = DateTime.Now,
                DealName = "Deal2",
                DealType = "TypeB",
                SourceListId = "Source2",
                Side = "Sell"
            };

            var result = await _repository.UpdateAsync(existingTrade.TradeId, updateDto);
            result.Should().BeTrue();

            var updatedTrade = await _fixture.Context.Trades.FindAsync(existingTrade.TradeId);
            updatedTrade.Should().NotBeNull();
            updatedTrade.Should().BeEquivalentTo(updateDto, options => options
                .Excluding(b => b.TradeId)
                .ExcludingMissingMembers()
            );
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnFalse_WhenNotFound()
        {
            await SeedAsync(new List<Trade>());
            var updateDto = new TradeDto
            {
                Account = "Account2",
                AccountType = "Type2",
                BuyQuantity = 200,
                SellQuantity = 150,
                BuyPrice = 15.5,
                SellPrice = 25.5,
                TradeDate = DateTime.Now,
                TradeSecurity = "Security2",
                TradeStatus = "Status2",
                Trader = "Trader2",
                Benchmark = "Benchmark2",
                Book = "Book2",
                CreationName = "Creator2",
                CreationDate = DateTime.Now,
                RevisionName = "Reviser2",
                RevisionDate = DateTime.Now,
                DealName = "Deal2",
                DealType = "TypeB",
                SourceListId = "Source2",
                Side = "Sell"
            };

            var result = await _repository.UpdateAsync(-1, updateDto);
            result.Should().BeFalse();
        }

        // Testing DeleteAsync
        [Theory]
        [MemberData(nameof(TradeTestData.GetTradesScenarios), MemberType = typeof(TradeTestData))]
        public async Task DeleteAsync_ShouldRemoveTrade(List<Trade> testData)
        {
            await SeedAsync(testData);
            var existingTrade = testData.FirstOrDefault(b => b.TradeId > 0);

            if (existingTrade == null) return;

            var result = await _repository.DeleteAsync(existingTrade.TradeId);
            result.Should().BeTrue();

            var deletedTrade = await _fixture.Context.Trades.FindAsync(existingTrade.TradeId);
            deletedTrade.Should().BeNull();
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnFalse_WhenNotFound()
        {
            await SeedAsync(new List<Trade>());
            var result = await _repository.DeleteAsync(-1);
            result.Should().BeFalse();
        }

        public void Dispose()
        {
            ClearDatabase();
        }
    }
}
