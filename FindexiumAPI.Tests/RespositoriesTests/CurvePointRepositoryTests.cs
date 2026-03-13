using FindexiumAPI.Data;
using FindexiumAPI.Domain;
using FindexiumAPI.Models;
using FindexiumAPI.Repositories;
using FindexiumAPI.Tests.TestData;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Xunit;
using Xunit.Sdk;

namespace FindexiumAPI.Tests.RespositoriesTests
{
    public class CurvePointRepositoryTests : IDisposable
    {
        private LocalDbContext _context;
        private readonly ICurvePointRepository _repository;

        public CurvePointRepositoryTests()
        {
            // Create a new in-memory database for each test
            var options = new DbContextOptionsBuilder<LocalDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new LocalDbContext(options);
            _repository = new CurvePointRepository(_context);
        }


        // Testing GetAllAsync
        [Theory]
        [MemberData(nameof(CurvePointTestData.GetCurvePointsScenarios), MemberType = typeof(CurvePointTestData))]
        public async Task GetAllAsync_ShouldReturnExpectedDtos(List<CurvePoint> testData)
        {
            // Arrange
            _context.Database.EnsureCreated();
            await _context.CurvePoints.AddRangeAsync(testData);
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
        [MemberData(nameof(CurvePointTestData.GetCurvePointsScenarios), MemberType = typeof(CurvePointTestData))]
        public async Task GetByIdAsync_ShouldReturnExpectedDto(List<CurvePoint> testData)
        {
            // Arrange
            _context.Database.EnsureCreated();
            await _context.CurvePoints.AddRangeAsync(testData);
            await _context.SaveChangesAsync();

            if (testData.Count == 0)
            {
                return; // Skip the test if there's no data to test
            }

            // Act & Assert
            foreach (var curvePoint in testData)
            {
                var result = await _repository.GetByIdAsync(curvePoint.Id);
                result.Should().NotBeNull();
                result.Should().BeEquivalentTo(
                    curvePoint,
                    options => options
                        .ExcludingMissingMembers()
                );
            }

        }

        [Theory]
        [MemberData(nameof(CurvePointTestData.GetCurvePointsScenarios), MemberType = typeof(CurvePointTestData))]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound(List<CurvePoint> testData)
        {
            // Arrange
            _context.Database.EnsureCreated();
            await _context.CurvePoints.AddRangeAsync(testData);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByIdAsync(-1); // Use an ID that doesn't exist

            // Assert
            result.Should().BeNull();
        }


        // Testing CreateAsync
        [Fact]
        public async Task CreateAsync_ShouldAddNewCurvePoint()
        {
            // Arrange
            _context.Database.EnsureCreated();

            var testDto = new CurvePointDto
            {
                CurveId = 10,
                Term = 1.0,
                CurvePointValue = 100.0
            };

            // Act
            var createdCurvePoint = await _repository.AddAsync(testDto);

            // Assert
            createdCurvePoint.Should().NotBeNull();
            createdCurvePoint.Id.Should().BePositive();
            createdCurvePoint.Should().BeEquivalentTo(testDto, options => options
             .Excluding(b => b.Id)
             .ExcludingMissingMembers()
            );

            var dbCount = await _context.CurvePoints.CountAsync();
            dbCount.Should().Be(1);
        }


        // Testing UpdateAsync
        [Theory]
        [MemberData(nameof(CurvePointTestData.GetCurvePointsScenarios), MemberType = typeof(CurvePointTestData))]
        public async Task UpdateAsync_ShouldModifyExistingCurvePoint(List<CurvePoint> testData)
        {
            // Arrange
            _context.Database.EnsureCreated();
            await _context.CurvePoints.AddRangeAsync(testData);
            await _context.SaveChangesAsync();

            if (testData.Count == 0)
            {
                return; // Skip the test if there's no data to test
            }

            var existingCurvePoint = testData.First();
            var updateDto = new CurvePointDto
            {
                CurveId = 20,
                Term = 2.0,
                CurvePointValue = 200.0
            };

            // Act
            var result = await _repository.UpdateAsync(existingCurvePoint.Id, updateDto);

            // Assert
            result.Should().Be(true);
            var updatedCurvePoint = await _context.CurvePoints.FindAsync(existingCurvePoint.Id);
            updatedCurvePoint.Should().NotBeNull();
            updatedCurvePoint.Should().BeEquivalentTo(updateDto, options => options
             .Excluding(b => b.Id)
             .ExcludingMissingMembers()
            );
        }

        [Theory]
        [MemberData(nameof(CurvePointTestData.GetCurvePointsScenarios), MemberType = typeof(CurvePointTestData))]
        public async Task UpdateAsync_ShouldReturnFalse_WhenNotFound(List<CurvePoint> testData)
        {
            // Arrange
            _context.Database.EnsureCreated();
            await _context.CurvePoints.AddRangeAsync(testData);
            await _context.SaveChangesAsync();

            var updateDto = new CurvePointDto
            {
                CurveId = 20,
                Term = 2.0,
                CurvePointValue = 200.0
            };

            // Act
            var result = await _repository.UpdateAsync(-1, updateDto); // Use an ID that doesn't exist

            // Assert
            result.Should().Be(false);
        }


        // Testing DeleteAsync
        [Theory]
        [MemberData(nameof(CurvePointTestData.GetCurvePointsScenarios), MemberType = typeof(CurvePointTestData))]
        public async Task DeleteAsync_ShouldRemoveCurvePoint(List<CurvePoint> testData)
        {
            // Arrange
            _context.Database.EnsureCreated();
            await _context.CurvePoints.AddRangeAsync(testData);
            await _context.SaveChangesAsync();

            if (testData.Count == 0)
            {
                return; // Skip the test if there's no data to test
            }

            var existingCurvePoint = testData.First();

            // Act
            var result = await _repository.DeleteAsync(existingCurvePoint.Id);

            // Assert
            result.Should().Be(true);
            var deletedCurvePoint = await _context.CurvePoints.FindAsync(existingCurvePoint.Id);
            deletedCurvePoint.Should().BeNull();
        }

        [Theory]
        [MemberData(nameof(CurvePointTestData.GetCurvePointsScenarios), MemberType = typeof(CurvePointTestData))]
        public async Task DeleteAsync_ShouldReturnFalse_WhenNotFound(List<CurvePoint> testData)
        {
            // Arrange
            _context.Database.EnsureCreated();
            await _context.CurvePoints.AddRangeAsync(testData);
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
