using FindexiumAPI.Data;
using FindexiumAPI.Domain;
using FindexiumAPI.Models;
using FindexiumAPI.Repositories;
using FindexiumAPI.Tests.TestData;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FindexiumAPI.Tests.RespositoriesTests
{
    public class BidListRepositoryTests : IDisposable
    {
        private LocalDbContext _context;
        private readonly IBidListRepository _repository;

        public BidListRepositoryTests()
        {
            // Create a new in-memory database for each test
            var options = new DbContextOptionsBuilder<LocalDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new LocalDbContext(options);
            _repository = new BidListRepository(_context);
        }


        // Testing GetAllAsync
        [Theory]
        [MemberData(nameof(BidListTestData.GetBidListsScenarios), MemberType = typeof(BidListTestData))]
        public async Task GetAllAsync_ShouldReturnExpectedDtos(List<BidList> testData)
        {
            // Arrange
            _context.Database.EnsureCreated();
            await _context.BidLists.AddRangeAsync(testData);
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
        [MemberData(nameof(BidListTestData.GetBidListsScenarios), MemberType = typeof(BidListTestData))]
        public async Task GetByIdAsync_ShouldReturnExpectedDto(List<BidList> testData)
        {
            // Arrange
            _context.Database.EnsureCreated();
            await _context.BidLists.AddRangeAsync(testData);
            await _context.SaveChangesAsync();

            if (testData.Count == 0)
            {
                return; // Skip the test if there's no data to test
            }

            // Act & Assert
            foreach (var bidList in testData)
            {
                var result = await _repository.GetByIdAsync(bidList.BidListId);
                result.Should().NotBeNull();
                result.Should().BeEquivalentTo(
                    bidList,
                    options => options
                        .ExcludingMissingMembers()
                );
            }

        }

        [Theory]
        [MemberData(nameof(BidListTestData.GetBidListsScenarios), MemberType = typeof(BidListTestData))]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound(List<BidList> testData)
        {
            // Arrange
            _context.Database.EnsureCreated();
            await _context.BidLists.AddRangeAsync(testData);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByIdAsync(-1); // Use an ID that doesn't exist

            // Assert
            result.Should().BeNull();
        }


        // Testing CreateAsync
        [Fact]
        public async Task CreateAsync_ShouldAddNewBidList()
        {
            // Arrange
            _context.Database.EnsureCreated();

            var testDto = new BidListDto
            {
                Account = "TestAccount",
                BidType = "TestType",
                BidQuantity = 123
            };

            // Act
            var createdBidList = await _repository.AddAsync(testDto);

            // Assert
            createdBidList.Should().NotBeNull();
            createdBidList.BidListId.Should().BePositive();
            createdBidList.Should().BeEquivalentTo(testDto, options => options
             .Excluding(b => b.BidListId)
             .ExcludingMissingMembers()
            );

            var dbCount = await _context.BidLists.CountAsync();
            dbCount.Should().Be(1);
        }


        // Testing UpdateAsync
        [Theory]
        [MemberData(nameof(BidListTestData.GetBidListsScenarios), MemberType = typeof(BidListTestData))]
        public async Task UpdateAsync_ShouldModifyExistingBidList(List<BidList> testData)
        {
            // Arrange
            _context.Database.EnsureCreated();
            await _context.BidLists.AddRangeAsync(testData);
            await _context.SaveChangesAsync();

            if (testData.Count == 0)
            {
                return; // Skip the test if there's no data to test
            }

            var existingBidList = testData.First();
            var updateDto = new BidListDto
            {
                Account = "UpdatedAccount",
                BidType = "UpdatedType",
                BidQuantity = 999
            };

            // Act
            var result = await _repository.UpdateAsync(existingBidList.BidListId, updateDto);

            // Assert
            result.Should().Be(true);
            var updatedBidList = await _context.BidLists.FindAsync(existingBidList.BidListId);
            updatedBidList.Should().NotBeNull();
            updatedBidList.Should().BeEquivalentTo(updateDto, options => options
             .Excluding(b => b.BidListId)
             .ExcludingMissingMembers()
            );
        }

        [Theory]
        [MemberData(nameof(BidListTestData.GetBidListsScenarios), MemberType = typeof(BidListTestData))]
        public async Task UpdateAsync_ShouldReturnFalse_WhenNotFound(List<BidList> testData)
        {
            // Arrange
            _context.Database.EnsureCreated();
            await _context.BidLists.AddRangeAsync(testData);
            await _context.SaveChangesAsync();

            var updateDto = new BidListDto
            {
                Account = "UpdatedAccount",
                BidType = "UpdatedType",
                BidQuantity = 999
            };

            // Act
            var result = await _repository.UpdateAsync(-1, updateDto); // Use an ID that doesn't exist

            // Assert
            result.Should().Be(false);
        }


        // Testing DeleteAsync
        [Theory]
        [MemberData(nameof(BidListTestData.GetBidListsScenarios), MemberType = typeof(BidListTestData))]
        public async Task DeleteAsync_ShouldRemoveBidList(List<BidList> testData)
        {
            // Arrange
            _context.Database.EnsureCreated();
            await _context.BidLists.AddRangeAsync(testData);
            await _context.SaveChangesAsync();

            if (testData.Count == 0)
            {
                return; // Skip the test if there's no data to test
            }

            var existingBidList = testData.First();

            // Act
            var result = await _repository.DeleteAsync(existingBidList.BidListId);

            // Assert
            result.Should().Be(true);
            var deletedBidList = await _context.BidLists.FindAsync(existingBidList.BidListId);
            deletedBidList.Should().BeNull();
        }

        [Theory]
        [MemberData(nameof(BidListTestData.GetBidListsScenarios), MemberType = typeof(BidListTestData))]
        public async Task DeleteAsync_ShouldReturnFalse_WhenNotFound(List<BidList> testData)
        {
            // Arrange
            _context.Database.EnsureCreated();
            await _context.BidLists.AddRangeAsync(testData);
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
