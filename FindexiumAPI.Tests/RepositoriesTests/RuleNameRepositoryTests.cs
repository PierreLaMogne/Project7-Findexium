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

    public class RuleNameRepositoryTests : IClassFixture<LocalDbFixture>, IDisposable
    {
        private readonly LocalDbFixture _fixture;
        private readonly IRuleNameRepository _repository;

        // The constructor initializes the repository with the in-memory database context provided by the fixture.
        // It also ensures that the database is cleared before each test run to maintain test isolation.
        public RuleNameRepositoryTests(LocalDbFixture fixture)
        {
            _fixture = fixture;
            _repository = new RuleNameRepository(_fixture.Context);
            ClearDatabase();
        }

        private void ClearDatabase()
        {
            _fixture.Context.RuleNames.RemoveRange(_fixture.Context.RuleNames);
            _fixture.Context.SaveChanges();
        }

        // The SeedAsync method is a helper function that populates the in-memory database with test data before each test case.
        // It first clears the database to ensure a clean state, then adds the provided list of RuleName entities and saves the changes.
        private async Task SeedAsync(List<RuleName> data)
        {
            ClearDatabase();
            await _fixture.Context.RuleNames.AddRangeAsync(data);
            await _fixture.Context.SaveChangesAsync();
        }


        // Testing GetAllAsync
        [Theory]
        [MemberData(nameof(RuleNameTestData.GetRuleNamesScenarios), MemberType = typeof(RuleNameTestData))]
        public async Task GetAllAsync_ShouldReturnExpectedDtos(List<RuleName> testData)
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
        [MemberData(nameof(RuleNameTestData.GetRuleNamesScenarios), MemberType = typeof(RuleNameTestData))]
        public async Task GetByIdAsync_ShouldReturnExpectedDto(List<RuleName> testData)
        {
            await SeedAsync(testData);

            foreach (var ruleName in testData.Where(b => b.Id > 0))
            {
                var result = await _repository.GetByIdAsync(ruleName.Id);
                result.Should().NotBeNull();
                result.Should().BeEquivalentTo(
                    ruleName,
                    options => options.ExcludingMissingMembers()
                );
            }
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound()
        {
            await SeedAsync(new List<RuleName>());
            var result = await _repository.GetByIdAsync(-1);
            result.Should().BeNull();
        }

        // Testing CreateAsync
        [Theory]
        [MemberData(nameof(RuleNameTestData.GetRuleNameDtosForCreate), MemberType = typeof(RuleNameTestData))]
        public async Task CreateAsync_ShouldAddNewRuleName(RuleNameDto testDto)
        {
            await SeedAsync(new List<RuleName>());

            var createdRuleName = await _repository.AddAsync(testDto);

            createdRuleName.Should().NotBeNull();
            createdRuleName.Id.Should().BePositive();
            createdRuleName.Should().BeEquivalentTo(testDto, options => options
                .Excluding(b => b.Id)
                .ExcludingMissingMembers()
            );

            var dbCount = await _fixture.Context.RuleNames.CountAsync();
            dbCount.Should().Be(1);
        }

        // Testing UpdateAsync
        [Theory]
        [MemberData(nameof(RuleNameTestData.GetRuleNamesScenarios), MemberType = typeof(RuleNameTestData))]
        public async Task UpdateAsync_ShouldModifyExistingRuleName(List<RuleName> testData)
        {
            await SeedAsync(testData);
            var existingRuleName = testData.FirstOrDefault(b => b.Id > 0);

            if (existingRuleName == null) return;

            var updateDto = new RuleNameDto
            {
                Name = "Rule2",
                Description = "Description2",
                Json = "{}",
                Template = "Template2",
                SqlStr = "SELECT * FROM Table2",
                SqlPart = "WHERE Condition2"
            };

            var result = await _repository.UpdateAsync(existingRuleName.Id, updateDto);
            result.Should().BeTrue();

            var updatedRuleName = await _fixture.Context.RuleNames.FindAsync(existingRuleName.Id);
            updatedRuleName.Should().NotBeNull();
            updatedRuleName.Should().BeEquivalentTo(updateDto, options => options
                .Excluding(b => b.Id)
                .ExcludingMissingMembers()
            );
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnFalse_WhenNotFound()
        {
            await SeedAsync(new List<RuleName>());
            var updateDto = new RuleNameDto
            {
                Name = "Rule2",
                Description = "Description2",
                Json = "{}",
                Template = "Template2",
                SqlStr = "SELECT * FROM Table2",
                SqlPart = "WHERE Condition2"
            };

            var result = await _repository.UpdateAsync(-1, updateDto);
            result.Should().BeFalse();
        }

        // Testing DeleteAsync
        [Theory]
        [MemberData(nameof(RuleNameTestData.GetRuleNamesScenarios), MemberType = typeof(RuleNameTestData))]
        public async Task DeleteAsync_ShouldRemoveRuleName(List<RuleName> testData)
        {
            await SeedAsync(testData);
            var existingRuleName = testData.FirstOrDefault(b => b.Id > 0);

            if (existingRuleName == null) return;

            var result = await _repository.DeleteAsync(existingRuleName.Id);
            result.Should().BeTrue();

            var deletedRuleName = await _fixture.Context.RuleNames.FindAsync(existingRuleName.Id);
            deletedRuleName.Should().BeNull();
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnFalse_WhenNotFound()
        {
            await SeedAsync(new List<RuleName>());
            var result = await _repository.DeleteAsync(-1);
            result.Should().BeFalse();
        }

        public void Dispose()
        {
            ClearDatabase();
        }
    }
}
