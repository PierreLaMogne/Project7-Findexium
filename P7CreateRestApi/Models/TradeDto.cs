namespace FindexiumAPI.Models
{
    public class TradeDto
    {
        public int TradeId { get; set; }
        public string Account { get; set; } = string.Empty;
        public string AccountType { get; set; } = string.Empty;
        public double? BuyQuantity { get; set; }
        public double? SellQuantity { get; set; }
        public double? BuyPrice { get; set; }
        public double? SellPrice { get; set; }
        public DateTime? TradeDate { get; set; }
        public string TradeSecurity { get; set; } = string.Empty;
        public string TradeStatus { get; set; } = string.Empty;
        public string Trader { get; set; } = string.Empty;
        public string Benchmark { get; set; } = string.Empty;
        public string Book { get; set; } = string.Empty;
        public string CreationName { get; set; } = string.Empty;
        public DateTime? CreationDate { get; set; }
        public string RevisionName { get; set; } = string.Empty;
        public DateTime? RevisionDate { get; set; }
        public string DealName { get; set; } = string.Empty;
        public string DealType { get; set; } = string.Empty;
        public string SourceListId { get; set; } = string.Empty;
        public string Side { get; set; } = string.Empty;
    }

}
