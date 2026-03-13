using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FindexiumAPI.Tests.TestData
{
    public class TradeTestData
    {
        public static IEnumerable<object[]> GetTradesScenarios()
        {
            yield return new object[]
            {
                new List<Domain.Trade>()
            };
            yield return new object[]
            {
                new List<Domain.Trade>
                {
                    new Domain.Trade { TradeId = 1, Account = "Account1", AccountType = "Type1", BuyQuantity = 100, SellQuantity = 50, BuyPrice = 10.5, SellPrice = 20.5, TradeDate = DateTime.Now, TradeSecurity = "Security1", TradeStatus = "Status1", Trader = "Trader1", Benchmark = "Benchmark1", Book = "Book1", CreationName = "Creator1", CreationDate = DateTime.Now, RevisionName = "Reviser1", RevisionDate = DateTime.Now, DealName = "Deal1", DealType = "TypeA", SourceListId = "Source1", Side = "Buy" }
                }
            };
            yield return new object[]
            {
                new List<Domain.Trade>
                {
                    new Domain.Trade { TradeId = 2, Account = "Account2", AccountType = "Type2", BuyQuantity = 200, SellQuantity = 150, BuyPrice = 15.5, SellPrice = 25.5, TradeDate = DateTime.Now, TradeSecurity = "Security2", TradeStatus = "Status2", Trader = "Trader2", Benchmark = "Benchmark2", Book = "Book2", CreationName = "Creator2", CreationDate = DateTime.Now, RevisionName = "Reviser2", RevisionDate = DateTime.Now, DealName = "Deal2", DealType = "TypeB", SourceListId = "Source2", Side = "Sell" },
                    new Domain.Trade { TradeId = 3, Account = "Account3", AccountType = "Type3", BuyQuantity = 300, SellQuantity = 250, BuyPrice = 20.5, SellPrice = 30.5, TradeDate = DateTime.Now, TradeSecurity = "Security3", TradeStatus = "Status3", Trader = "Trader3", Benchmark = "Benchmark3", Book = "Book3", CreationName = "Creator3", CreationDate = DateTime.Now, RevisionName = "Reviser3", RevisionDate = DateTime.Now, DealName = "Deal3", DealType = "TypeC", SourceListId = "Source3", Side = "Buy" }
                }
            };
        }
    }
}