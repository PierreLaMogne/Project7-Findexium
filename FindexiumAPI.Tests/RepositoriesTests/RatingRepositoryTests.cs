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
    public class RatingRepositoryTests : IClassFixture<LocalDbFixture>, IDisposable
    {
        private readonly LocalDbFixture _fixture;
        private readonly IRatingRepository _repository;

        // The constructor initializes the repository with the in-memory database context provided by the fixture.
        // It also ensures that the database is cleared before each test run to maintain test isolation.
        public RatingRepositoryTests(LocalDbFixture fixture)
        {
            _fixture = fixture;
            _repository = new RatingRepository(_fixture.Context);
            ClearDatabase();
        }

        private void ClearDatabase()
        {
            _fixture.Context.Ratings.RemoveRange(_fixture.Context.Ratings);
            _fixture.Context.SaveChanges();
        }

        // The SeedAsync method is a helper function that populates the in-memory database with test data before each test case.
        // It first clears the database to ensure a clean state, then adds the provided list of Rating entities and saves the changes.
        private async Task SeedAsync(List<Rating> data)
        {
            ClearDatabase();
            await _fixture.Context.Ratings.AddRangeAsync(data);
            await _fixture.Context.SaveChangesAsync();
        }


        // Testing GetAllAsync
        [Theory]
        [MemberData(nameof(RatingTestData.GetRatingsScenarios), MemberType = typeof(RatingTestData))]
        public async Task GetAllAsync_ShouldReturnExpectedDtos(List<Rating> testData)
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
        [MemberData(nameof(RatingTestData.GetRatingsScenarios), MemberType = typeof(RatingTestData))]
        public async Task GetByIdAsync_ShouldReturnExpectedDto(List<Rating> testData)
        {
            await SeedAsync(testData);

            foreach (var rating in testData.Where(b => b.Id > 0))
            {
                var result = await _repository.GetByIdAsync(rating.Id);
                result.Should().NotBeNull();
                result.Should().BeEquivalentTo(
                    rating,
                    options => options.ExcludingMissingMembers()
                );
            }
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound()
        {
            await SeedAsync(new List<Rating>());
            var result = await _repository.GetByIdAsync(-1);
            result.Should().BeNull();
        }

        // Testing CreateAsync
        [Theory]
        [MemberData(nameof(RatingTestData.GetRatingDtosForCreate), MemberType = typeof(RatingTestData))]
        public async Task CreateAsync_ShouldAddNewRating(RatingDto testDto)
        {
            await SeedAsync(new List<Rating>());

            var createdRating = await _repository.AddAsync(testDto);

            createdRating.Should().NotBeNull();
            createdRating.Id.Should().BePositive();
            createdRating.Should().BeEquivalentTo(testDto, options => options
                .Excluding(b => b.Id)
                .ExcludingMissingMembers()
            );

            var dbCount = await _fixture.Context.Ratings.CountAsync();
            dbCount.Should().Be(1);
        }

        // Testing UpdateAsync
        [Theory]
        [MemberData(nameof(RatingTestData.GetRatingsScenarios), MemberType = typeof(RatingTestData))]
        public async Task UpdateAsync_ShouldModifyExistingRating(List<Rating> testData)
        {
            await SeedAsync(testData);
            var existingRating = testData.FirstOrDefault(b => b.Id > 0);

            if (existingRating == null) return;

            var updateDto = new RatingDto
            {
                MoodysRating = "Aa2",
                SandPRating = "AA+",
                FitchRating = "AA+",
                OrderNumber = 2
            };

            var result = await _repository.UpdateAsync(existingRating.Id, updateDto);
            result.Should().BeTrue();

            var updatedRating = await _fixture.Context.Ratings.FindAsync(existingRating.Id);
            updatedRating.Should().NotBeNull();
            updatedRating.Should().BeEquivalentTo(updateDto, options => options
                .Excluding(b => b.Id)
                .ExcludingMissingMembers()
            );
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnFalse_WhenNotFound()
        {
            await SeedAsync(new List<Rating>());
            var updateDto = new RatingDto
            {
                MoodysRating = "Aa2",
                SandPRating = "AA+",
                FitchRating = "AA+",
                OrderNumber = 2
            };

            var result = await _repository.UpdateAsync(-1, updateDto);
            result.Should().BeFalse();
        }

        // Testing DeleteAsync
        [Theory]
        [MemberData(nameof(RatingTestData.GetRatingsScenarios), MemberType = typeof(RatingTestData))]
        public async Task DeleteAsync_ShouldRemoveRating(List<Rating> testData)
        {
            await SeedAsync(testData);
            var existingRating = testData.FirstOrDefault(b => b.Id > 0);

            if (existingRating == null) return;

            var result = await _repository.DeleteAsync(existingRating.Id);
            result.Should().BeTrue();

            var deletedRating = await _fixture.Context.Ratings.FindAsync(existingRating.Id);
            deletedRating.Should().BeNull();
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnFalse_WhenNotFound()
        {
            await SeedAsync(new List<Rating>());
            var result = await _repository.DeleteAsync(-1);
            result.Should().BeFalse();
        }

        public void Dispose()
        {
            ClearDatabase();
        }
    }
}
