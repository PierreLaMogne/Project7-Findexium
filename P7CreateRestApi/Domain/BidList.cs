namespace FindexiumAPI.Domain
{
    public class BidList
    {
        // DONE: Map columns in data table BIDLIST with corresponding fields
        public int BidListId { get; set; }
        public string Account { get; set; } = string.Empty;
        public string BidType { get; set; } = string.Empty;
        public double? BidQuantity { get; set; }
        public double? AskQuantity { get; set; }
        public double? Bid { get; set; }
        public double? Ask { get; set; }
        public string Benchmark { get; set; } = string.Empty;
        public DateTime? BidListDate { get; set; }
        public string Commentary { get; set; } = string.Empty;
        public string BidSecurity { get; set; } = string.Empty;
        public string BidStatus { get; set; } = string.Empty;
        public string Trader { get; set; } = string.Empty;
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