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
    public class BidListRepositoryTests : IClassFixture<LocalDbFixture>, IDisposable
    {
        private readonly LocalDbFixture _fixture;
        private readonly IBidListRepository _repository;

        // The constructor initializes the repository with the in-memory database context provided by the fixture.
        // It also ensures that the database is cleared before each test run to maintain test isolation.
        public BidListRepositoryTests(LocalDbFixture fixture)
        {
            _fixture = fixture;
            _repository = new BidListRepository(_fixture.Context);
            ClearDatabase();
        }

        private void ClearDatabase()
        {
            _fixture.Context.BidLists.RemoveRange(_fixture.Context.BidLists);
            _fixture.Context.SaveChanges();
        }

        // The SeedAsync method is a helper function that populates the in-memory database with test data before each test case.
        // It first clears the database to ensure a clean state, then adds the provided list of BidList entities and saves the changes.
        private async Task SeedAsync(List<BidList> data)
        {
            ClearDatabase();
            await _fixture.Context.BidLists.AddRangeAsync(data);
            await _fixture.Context.SaveChangesAsync();
        }


        // Testing GetAllAsync
        [Theory]
        [MemberData(nameof(BidListTestData.GetBidListsScenarios), MemberType = typeof(BidListTestData))]
        public async Task GetAllAsync_ShouldReturnExpectedDtos(List<BidList> testData)
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
        [MemberData(nameof(BidListTestData.GetBidListsScenarios), MemberType = typeof(BidListTestData))]
        public async Task GetByIdAsync_ShouldReturnExpectedDto(List<BidList> testData)
        {
            await SeedAsync(testData);

            foreach (var bidList in testData.Where(b => b.BidListId > 0))
            {
                var result = await _repository.GetByIdAsync(bidList.BidListId);
                result.Should().NotBeNull();
                result.Should().BeEquivalentTo(
                    bidList,
                    options => options.ExcludingMissingMembers()
                );
            }
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound()
        {
            await SeedAsync(new List<BidList>());
            var result = await _repository.GetByIdAsync(-1);
            result.Should().BeNull();
        }

        // Testing CreateAsync
        [Theory]
        [MemberData(nameof(BidListTestData.GetBidListDtosForCreate), MemberType = typeof(BidListTestData))]
        public async Task CreateAsync_ShouldAddNewBidList(BidListDto testDto)
        {
            await SeedAsync(new List<BidList>());

            var createdBidList = await _repository.AddAsync(testDto);

            createdBidList.Should().NotBeNull();
            createdBidList.BidListId.Should().BePositive();
            createdBidList.Should().BeEquivalentTo(testDto, options => options
                .Excluding(b => b.BidListId)
                .ExcludingMissingMembers()
            );

            var dbCount = await _fixture.Context.BidLists.CountAsync();
            dbCount.Should().Be(1);
        }

        // Testing UpdateAsync
        [Theory]
        [MemberData(nameof(BidListTestData.GetBidListsScenarios), MemberType = typeof(BidListTestData))]
        public async Task UpdateAsync_ShouldModifyExistingBidList(List<BidList> testData)
        {
            await SeedAsync(testData);
            var existingBidList = testData.FirstOrDefault(b => b.BidListId > 0);

            if (existingBidList == null) return;

            var updateDto = new BidListDto
            {
                Account = "UpdatedAccount",
                BidType = "UpdatedType",
                BidQuantity = 999
            };

            var result = await _repository.UpdateAsync(existingBidList.BidListId, updateDto);
            result.Should().BeTrue();

            var updatedBidList = await _fixture.Context.BidLists.FindAsync(existingBidList.BidListId);
            updatedBidList.Should().NotBeNull();
            updatedBidList.Should().BeEquivalentTo(updateDto, options => options
                .Excluding(b => b.BidListId)
                .ExcludingMissingMembers()
            );
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnFalse_WhenNotFound()
        {
            await SeedAsync(new List<BidList>());
            var updateDto = new BidListDto
            {
                Account = "UpdatedAccount",
                BidType = "UpdatedType",
                BidQuantity = 999
            };

            var result = await _repository.UpdateAsync(-1, updateDto);
            result.Should().BeFalse();
        }

        // Testing DeleteAsync
        [Theory]
        [MemberData(nameof(BidListTestData.GetBidListsScenarios), MemberType = typeof(BidListTestData))]
        public async Task DeleteAsync_ShouldRemoveBidList(List<BidList> testData)
        {
            await SeedAsync(testData);
            var existingBidList = testData.FirstOrDefault(b => b.BidListId > 0);

            if (existingBidList == null) return;

            var result = await _repository.DeleteAsync(existingBidList.BidListId);
            result.Should().BeTrue();

            var deletedBidList = await _fixture.Context.BidLists.FindAsync(existingBidList.BidListId);
            deletedBidList.Should().BeNull();
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnFalse_WhenNotFound()
        {
            await SeedAsync(new List<BidList>());
            var result = await _repository.DeleteAsync(-1);
            result.Should().BeFalse();
        }

        public void Dispose()
        {
            ClearDatabase();
        }
    }
}
