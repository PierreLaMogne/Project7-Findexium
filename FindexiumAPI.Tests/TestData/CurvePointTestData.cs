using FindexiumAPI.Domain;
using FindexiumAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FindexiumAPI.Tests.TestData
{
    public class CurvePointTestData
    {
        public static IEnumerable<object[]> GetCurvePointsScenarios()
        {
            yield return new object[]
            {
                new List<CurvePoint>()
            };
            yield return new object[]
            {
                new List<CurvePoint>
                {
                    new CurvePoint { Id = 1, CurveId = 10, Term = 1.0, CurvePointValue = 100.0 }
                }
            };
            yield return new object[]
            {
                new List<CurvePoint>
                {
                    new CurvePoint { Id = 2, CurveId = 20, Term = 2.0, CurvePointValue = 200.0 },
                    new CurvePoint { Id = 3, CurveId = 30, Term = 3.0, CurvePointValue = 300.0 }
                }
            };
        }

        public static IEnumerable<object[]> GetCurvePointDtosForCreate()
        {
            yield return new object[] { new CurvePointDto { CurveId = 10, Term = 1.0, CurvePointValue = 100.0 } };
            yield return new object[] { new CurvePointDto { CurveId = 20, Term = 2.0, CurvePointValue = 200.0 } };
        }
    }
}