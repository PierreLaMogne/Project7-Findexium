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
    public class RatingRepositoryTests : IDisposable
    {
        private LocalDbContext _context;
        private readonly IRatingRepository _repository;

        public RatingRepositoryTests()
        {
            // Create a new in-memory database for each test
            var options = new DbContextOptionsBuilder<LocalDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new LocalDbContext(options);
            _repository = new RatingRepository(_context);
        }


        // Testing GetAllAsync
        [Theory]
        [MemberData(nameof(RatingTestData.GetRatingsScenarios), MemberType = typeof(RatingTestData))]
        public async Task GetAllAsync_ShouldReturnExpectedDtos(List<Rating> testData)
        {
            // Arrange
            _context.Database.EnsureCreated();
            await _context.Ratings.AddRangeAsync(testData);
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
        [MemberData(nameof(RatingTestData.GetRatingsScenarios), MemberType = typeof(RatingTestData))]
        public async Task GetByIdAsync_ShouldReturnExpectedDto(List<Rating> testData)
        {
            // Arrange
            _context.Database.EnsureCreated();
            await _context.Ratings.AddRangeAsync(testData);
            await _context.SaveChangesAsync();

            if (testData.Count == 0)
            {
                return; // Skip the test if there's no data to test
            }

            // Act & Assert
            foreach (var rating in testData)
            {
                var result = await _repository.GetByIdAsync(rating.Id);
                result.Should().NotBeNull();
                result.Should().BeEquivalentTo(
                    rating,
                    options => options
                        .ExcludingMissingMembers()
                );
            }

        }

        [Theory]
        [MemberData(nameof(RatingTestData.GetRatingsScenarios), MemberType = typeof(RatingTestData))]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound(List<Rating> testData)
        {
            // Arrange
            _context.Database.EnsureCreated();
            await _context.Ratings.AddRangeAsync(testData);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByIdAsync(-1); // Use an ID that doesn't exist

            // Assert
            result.Should().BeNull();
        }


        // Testing CreateAsync
        [Fact]
        public async Task CreateAsync_ShouldAddNewRating()
        {
            // Arrange
            _context.Database.EnsureCreated();

            var testDto = new RatingDto
            {
                MoodysRating = "Aaa",
                SandPRating = "AAA",
                FitchRating = "AAA",
                OrderNumber = 1
            };

            // Act
            var createdRating = await _repository.AddAsync(testDto);

            // Assert
            createdRating.Should().NotBeNull();
            createdRating.Id.Should().BePositive();
            createdRating.Should().BeEquivalentTo(testDto, options => options
             .Excluding(b => b.Id)
             .ExcludingMissingMembers()
            );

            var dbCount = await _context.Ratings.CountAsync();
            dbCount.Should().Be(1);
        }


        // Testing UpdateAsync
        [Theory]
        [MemberData(nameof(RatingTestData.GetRatingsScenarios), MemberType = typeof(RatingTestData))]
        public async Task UpdateAsync_ShouldModifyExistingRating(List<Rating> testData)
        {
            // Arrange
            _context.Database.EnsureCreated();
            await _context.Ratings.AddRangeAsync(testData);
            await _context.SaveChangesAsync();

            if (testData.Count == 0)
            {
                return; // Skip the test if there's no data to test
            }

            var existingRating = testData.First();
            var updateDto = new RatingDto
            {
                MoodysRating = "Bbb",
                SandPRating = "BBB",
                FitchRating = "BBB",
                OrderNumber = 10
            };

            // Act
            var result = await _repository.UpdateAsync(existingRating.Id, updateDto);

            // Assert
            result.Should().Be(true);
            var updatedRating = await _context.Ratings.FindAsync(existingRating.Id);
            updatedRating.Should().NotBeNull();
            updatedRating.Should().BeEquivalentTo(updateDto, options => options
             .Excluding(b => b.Id)
             .ExcludingMissingMembers()
            );
        }

        [Theory]
        [MemberData(nameof(RatingTestData.GetRatingsScenarios), MemberType = typeof(RatingTestData))]
        public async Task UpdateAsync_ShouldReturnFalse_WhenNotFound(List<Rating> testData)
        {
            // Arrange
            _context.Database.EnsureCreated();
            await _context.Ratings.AddRangeAsync(testData);
            await _context.SaveChangesAsync();

            var updateDto = new RatingDto
            {
                MoodysRating = "Bbb",
                SandPRating = "BBB",
                FitchRating = "BBB",
                OrderNumber = 10
            };

            // Act
            var result = await _repository.UpdateAsync(-1, updateDto); // Use an ID that doesn't exist

            // Assert
            result.Should().Be(false);
        }


        // Testing DeleteAsync
        [Theory]
        [MemberData(nameof(RatingTestData.GetRatingsScenarios), MemberType = typeof(RatingTestData))]
        public async Task DeleteAsync_ShouldRemoveRating(List<Rating> testData)
        {
            // Arrange
            _context.Database.EnsureCreated();
            await _context.Ratings.AddRangeAsync(testData);
            await _context.SaveChangesAsync();

            if (testData.Count == 0)
            {
                return; // Skip the test if there's no data to test
            }

            var existingRating = testData.First();

            // Act
            var result = await _repository.DeleteAsync(existingRating.Id);

            // Assert
            result.Should().Be(true);
            var deletedRating = await _context.Ratings.FindAsync(existingRating.Id);
            deletedRating.Should().BeNull();
        }

        [Theory]
        [MemberData(nameof(RatingTestData.GetRatingsScenarios), MemberType = typeof(RatingTestData))]
        public async Task DeleteAsync_ShouldReturnFalse_WhenNotFound(List<Rating> testData)
        {
            // Arrange
            _context.Database.EnsureCreated();
            await _context.Ratings.AddRangeAsync(testData);
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
