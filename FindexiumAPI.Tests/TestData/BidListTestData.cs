using FindexiumAPI.Domain;

namespace FindexiumAPI.Tests.TestData
{
    public class BidListTestData
    {
        public static IEnumerable<object[]> GetBidListsScenarios()
        {
            yield return new object[]
            {
                new List<BidList>()
            };

            yield return new object[]
            {
                new List<BidList>
                {
                    new BidList { BidListId = 1, Account = "Account1", BidType = "Type1", BidQuantity = 100 }
                }
            };

            yield return new object[]
            {
                new List<BidList>
                {
                    new BidList { BidListId = 2, Account = "Account2", BidType = "Type2", BidQuantity = 200 },
                    new BidList { BidListId = 3, Account = "Account3", BidType = "Type3", BidQuantity = 300 }
                }
            };

        }
    }
}
