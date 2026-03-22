using FindexiumAPI.Domain;
using FindexiumAPI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Sdk;

namespace FindexiumAPI.Tests.TestData
{
    public class RatingTestData
    {
        public static IEnumerable<object[]> GetRatingsScenarios()
        {
            yield return new object[]
            {
                new List<Rating>()
            };
            yield return new object[]
            {
                new List<Rating>
                {
                    new Rating { Id = 1, MoodysRating = "Aaa", SandPRating = "AAA", FitchRating = "AAA", OrderNumber = 1 }
                }
            };
            yield return new object[]
            {
                new List<Domain.Rating>
                {
                    new Rating { Id = 2, MoodysRating = "Aa2", SandPRating = "AA+", FitchRating = "AA+", OrderNumber = 2 },
                    new Rating { Id = 3, MoodysRating = "A1", SandPRating = "A-", FitchRating = "A-", OrderNumber = 3 }
                }
            };
        }

        public static IEnumerable<object[]> GetRatingDtosForCreate()
        {
            yield return new object[] { new RatingDto { MoodysRating = "Aaa", SandPRating = "AAA", FitchRating = "AAA", OrderNumber = 1 } };
            yield return new object[] { new RatingDto { MoodysRating = "Aa2", SandPRating = "AA+", FitchRating = "AA+", OrderNumber = 2 } };
        }
    }
}