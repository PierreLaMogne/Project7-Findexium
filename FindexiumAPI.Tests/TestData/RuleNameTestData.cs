using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FindexiumAPI.Tests.TestData
{
    public class RuleNameTestData
    {
        public static IEnumerable<object[]> GetRuleNamesScenarios()
        {
            yield return new object[]
            {
                new List<Domain.RuleName>()
            };
            yield return new object[]
            {
                new List<Domain.RuleName>
                {
                    new Domain.RuleName { Id = 1, Name = "Rule1", Description = "Description1", Json = "{}", Template = "Template1", SqlStr = "SELECT * FROM Table1", SqlPart = "WHERE Condition1" }
                }
            };
            yield return new object[]
            {
                new List<Domain.RuleName>
                {
                    new Domain.RuleName { Id = 2, Name = "Rule2", Description = "Description2", Json = "{}", Template = "Template2", SqlStr = "SELECT * FROM Table2", SqlPart = "WHERE Condition2" },
                    new Domain.RuleName { Id = 3, Name = "Rule3", Description = "Description3", Json = "{}", Template = "Template3", SqlStr = "SELECT * FROM Table3", SqlPart = "WHERE Condition3" }
                }
            };
        }
    }
}
