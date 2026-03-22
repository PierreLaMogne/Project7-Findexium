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
    public class CurvePointRepositoryTests : IClassFixture<LocalDbFixture>, IDisposable
    {
        private readonly LocalDbFixture _fixture;
        private readonly ICurvePointRepository _repository;

        // The constructor initializes the repository with the in-memory database context provided by the fixture.
        // It also ensures that the database is cleared before each test run to maintain test isolation.
        public CurvePointRepositoryTests(LocalDbFixture fixture)
        {
            _fixture = fixture;
            _repository = new CurvePointRepository(_fixture.Context);
            ClearDatabase();
        }

        private void ClearDatabase()
        {
            _fixture.Context.CurvePoints.RemoveRange(_fixture.Context.CurvePoints);
            _fixture.Context.SaveChanges();
        }

        // The SeedAsync method is a helper function that populates the in-memory database with test data before each test case.
        // It first clears the database to ensure a clean state, then adds the provided list of CurvePoint entities and saves the changes.
        private async Task SeedAsync(List<CurvePoint> data)
        {
            ClearDatabase();
            await _fixture.Context.CurvePoints.AddRangeAsync(data);
            await _fixture.Context.SaveChangesAsync();
        }


        // Testing GetAllAsync
        [Theory]
        [MemberData(nameof(CurvePointTestData.GetCurvePointsScenarios), MemberType = typeof(CurvePointTestData))]
        public async Task GetAllAsync_ShouldReturnExpectedDtos(List<CurvePoint> testData)
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
        [MemberData(nameof(CurvePointTestData.GetCurvePointsScenarios), MemberType = typeof(CurvePointTestData))]
        public async Task GetByIdAsync_ShouldReturnExpectedDto(List<CurvePoint> testData)
        {
            await SeedAsync(testData);

            foreach (var curvePoint in testData.Where(b => b.Id > 0))
            {
                var result = await _repository.GetByIdAsync(curvePoint.Id);
                result.Should().NotBeNull();
                result.Should().BeEquivalentTo(
                    curvePoint,
                    options => options.ExcludingMissingMembers()
                );
            }
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound()
        {
            await SeedAsync(new List<CurvePoint>());
            var result = await _repository.GetByIdAsync(-1);
            result.Should().BeNull();
        }

        // Testing CreateAsync
        [Theory]
        [MemberData(nameof(CurvePointTestData.GetCurvePointDtosForCreate), MemberType = typeof(CurvePointTestData))]
        public async Task CreateAsync_ShouldAddNewCurvePoint(CurvePointDto testDto)
        {
            await SeedAsync(new List<CurvePoint>());

            var createdCurvePoint = await _repository.AddAsync(testDto);

            createdCurvePoint.Should().NotBeNull();
            createdCurvePoint.Id.Should().BePositive();
            createdCurvePoint.Should().BeEquivalentTo(testDto, options => options
                .Excluding(b => b.Id)
                .ExcludingMissingMembers()
            );

            var dbCount = await _fixture.Context.CurvePoints.CountAsync();
            dbCount.Should().Be(1);
        }

        // Testing UpdateAsync
        [Theory]
        [MemberData(nameof(CurvePointTestData.GetCurvePointsScenarios), MemberType = typeof(CurvePointTestData))]
        public async Task UpdateAsync_ShouldModifyExistingCurvePoint(List<CurvePoint> testData)
        {
            await SeedAsync(testData);
            var existingCurvePoint = testData.FirstOrDefault(b => b.Id > 0);

            if (existingCurvePoint == null) return;

            var updateDto = new CurvePointDto
            {
                CurveId = 20,
                Term = 2.0,
                CurvePointValue = 200.0
            };

            var result = await _repository.UpdateAsync(existingCurvePoint.Id, updateDto);
            result.Should().BeTrue();

            var updatedCurvePoint = await _fixture.Context.CurvePoints.FindAsync(existingCurvePoint.Id);
            updatedCurvePoint.Should().NotBeNull();
            updatedCurvePoint.Should().BeEquivalentTo(updateDto, options => options
                .Excluding(b => b.Id)
                .ExcludingMissingMembers()
            );
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnFalse_WhenNotFound()
        {
            await SeedAsync(new List<CurvePoint>());
            var updateDto = new CurvePointDto
            {
                CurveId = 20,
                Term = 2.0,
                CurvePointValue = 200.0
            };

            var result = await _repository.UpdateAsync(-1, updateDto);
            result.Should().BeFalse();
        }

        // Testing DeleteAsync
        [Theory]
        [MemberData(nameof(CurvePointTestData.GetCurvePointsScenarios), MemberType = typeof(CurvePointTestData))]
        public async Task DeleteAsync_ShouldRemoveCurvePoint(List<CurvePoint> testData)
        {
            await SeedAsync(testData);
            var existingCurvePoint = testData.FirstOrDefault(b => b.Id > 0);

            if (existingCurvePoint == null) return;

            var result = await _repository.DeleteAsync(existingCurvePoint.Id);
            result.Should().BeTrue();

            var deletedCurvePoint = await _fixture.Context.CurvePoints.FindAsync(existingCurvePoint.Id);
            deletedCurvePoint.Should().BeNull();
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnFalse_WhenNotFound()
        {
            await SeedAsync(new List<CurvePoint>());
            var result = await _repository.DeleteAsync(-1);
            result.Should().BeFalse();
        }

        public void Dispose()
        {
            ClearDatabase();
        }
    }
}
