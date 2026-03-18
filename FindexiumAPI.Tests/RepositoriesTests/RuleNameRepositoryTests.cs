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
    public class RuleNameRepositoryTests : IDisposable
    {
        private LocalDbContext _context;
        private readonly IRuleNameRepository _repository;

        public RuleNameRepositoryTests()
        {
            // Create a new in-memory database for each test
            var options = new DbContextOptionsBuilder<LocalDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new LocalDbContext(options);
            _repository = new RuleNameRepository(_context);
        }


        // Testing GetAllAsync
        [Theory]
        [MemberData(nameof(RuleNameTestData.GetRuleNamesScenarios), MemberType = typeof(RuleNameTestData))]
        public async Task GetAllAsync_ShouldReturnExpectedDtos(List<RuleName> testData)
        {
            // Arrange
            _context.Database.EnsureCreated();
            await _context.RuleNames.AddRangeAsync(testData);
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
        [MemberData(nameof(RuleNameTestData.GetRuleNamesScenarios), MemberType = typeof(RuleNameTestData))]
        public async Task GetByIdAsync_ShouldReturnExpectedDto(List<RuleName> testData)
        {
            // Arrange
            _context.Database.EnsureCreated();
            await _context.RuleNames.AddRangeAsync(testData);
            await _context.SaveChangesAsync();

            if (testData.Count == 0)
            {
                return; // Skip the test if there's no data to test
            }

            // Act & Assert
            foreach (var ruleName in testData)
            {
                var result = await _repository.GetByIdAsync(ruleName.Id);
                result.Should().NotBeNull();
                result.Should().BeEquivalentTo(
                    ruleName,
                    options => options
                        .ExcludingMissingMembers()
                );
            }

        }

        [Theory]
        [MemberData(nameof(RuleNameTestData.GetRuleNamesScenarios), MemberType = typeof(RuleNameTestData))]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound(List<RuleName> testData)
        {
            // Arrange
            _context.Database.EnsureCreated();
            await _context.RuleNames.AddRangeAsync(testData);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByIdAsync(-1); // Use an ID that doesn't exist

            // Assert
            result.Should().BeNull();
        }


        // Testing CreateAsync
        [Fact]
        public async Task CreateAsync_ShouldAddNewRuleName()
        {
            // Arrange
            _context.Database.EnsureCreated();

            var testDto = new RuleNameDto
            {
                Name = "Rule1",
                Description = "Description1",
                Json = "{}",
                Template = "Template1",
                SqlStr = "SELECT * FROM Table1",
                SqlPart = "WHERE Condition1"
            };

            // Act
            var createdRuleName = await _repository.AddAsync(testDto);

            // Assert
            createdRuleName.Should().NotBeNull();
            createdRuleName.Id.Should().BePositive();
            createdRuleName.Should().BeEquivalentTo(testDto, options => options
             .Excluding(b => b.Id)
             .ExcludingMissingMembers()
            );

            var dbCount = await _context.RuleNames.CountAsync();
            dbCount.Should().Be(1);
        }


        // Testing UpdateAsync
        [Theory]
        [MemberData(nameof(RuleNameTestData.GetRuleNamesScenarios), MemberType = typeof(RuleNameTestData))]
        public async Task UpdateAsync_ShouldModifyExistingRuleName(List<RuleName> testData)
        {
            // Arrange
            _context.Database.EnsureCreated();
            await _context.RuleNames.AddRangeAsync(testData);
            await _context.SaveChangesAsync();

            if (testData.Count == 0)
            {
                return; // Skip the test if there's no data to test
            }

            var existingRuleName = testData.First();
            var updateDto = new RuleNameDto
            {
                Name = "Rule10",
                Description = "Description10",
                Json = "{}",
                Template = "Template10",
                SqlStr = "SELECT * FROM Table10",
                SqlPart = "WHERE Condition10"
            };

            // Act
            var result = await _repository.UpdateAsync(existingRuleName.Id, updateDto);

            // Assert
            result.Should().Be(true);
            var updatedRuleName = await _context.RuleNames.FindAsync(existingRuleName.Id);
            updatedRuleName.Should().NotBeNull();
            updatedRuleName.Should().BeEquivalentTo(updateDto, options => options
             .Excluding(b => b.Id)
             .ExcludingMissingMembers()
            );
        }

        [Theory]
        [MemberData(nameof(RuleNameTestData.GetRuleNamesScenarios), MemberType = typeof(RuleNameTestData))]
        public async Task UpdateAsync_ShouldReturnFalse_WhenNotFound(List<RuleName> testData)
        {
            // Arrange
            _context.Database.EnsureCreated();
            await _context.RuleNames.AddRangeAsync(testData);
            await _context.SaveChangesAsync();

            var updateDto = new RuleNameDto
            {
                Name = "Rule10",
                Description = "Description10",
                Json = "{}",
                Template = "Template10",
                SqlStr = "SELECT * FROM Table10",
                SqlPart = "WHERE Condition10"
            };

            // Act
            var result = await _repository.UpdateAsync(-1, updateDto); // Use an ID that doesn't exist

            // Assert
            result.Should().Be(false);
        }


        // Testing DeleteAsync
        [Theory]
        [MemberData(nameof(RuleNameTestData.GetRuleNamesScenarios), MemberType = typeof(RuleNameTestData))]
        public async Task DeleteAsync_ShouldRemoveRuleName(List<RuleName> testData)
        {
            // Arrange
            _context.Database.EnsureCreated();
            await _context.RuleNames.AddRangeAsync(testData);
            await _context.SaveChangesAsync();

            if (testData.Count == 0)
            {
                return; // Skip the test if there's no data to test
            }

            var existingRuleName = testData.First();

            // Act
            var result = await _repository.DeleteAsync(existingRuleName.Id);

            // Assert
            result.Should().Be(true);
            var deletedRuleName = await _context.RuleNames.FindAsync(existingRuleName.Id);
            deletedRuleName.Should().BeNull();
        }

        [Theory]
        [MemberData(nameof(RuleNameTestData.GetRuleNamesScenarios), MemberType = typeof(RuleNameTestData))]
        public async Task DeleteAsync_ShouldReturnFalse_WhenNotFound(List<RuleName> testData)
        {
            // Arrange
            _context.Database.EnsureCreated();
            await _context.RuleNames.AddRangeAsync(testData);
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
