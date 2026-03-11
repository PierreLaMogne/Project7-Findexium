using System.ComponentModel.DataAnnotations;

namespace FindexiumAPI.Models
{
    public class TradeDto
    {
        public int TradeId { get; set; }
        [Required]
        public string Account { get; set; } = string.Empty;
        [Required]
        public string AccountType { get; set; } = string.Empty;
        [Required]
        [RegularExpression(@"^\d+$", ErrorMessage = "Only digits are allowed.")]
        public double? BuyQuantity { get; set; }
        [Required]
        [RegularExpression(@"^\d+$", ErrorMessage = "Only digits are allowed.")]
        public double? SellQuantity { get; set; }
        [Required]
        [RegularExpression(@"^\d+$", ErrorMessage = "Only digits are allowed.")]
        public double? BuyPrice { get; set; }
        [Required]
        [RegularExpression(@"^\d+$", ErrorMessage = "Only digits are allowed.")]
        public double? SellPrice { get; set; }
        [Required]
        public DateTime? TradeDate { get; set; }
        [Required]
        public string TradeSecurity { get; set; } = string.Empty;
        [Required]
        public string TradeStatus { get; set; } = string.Empty;
        [Required]
        public string Trader { get; set; } = string.Empty;
        [Required]
        public string Benchmark { get; set; } = string.Empty;
        [Required]
        public string Book { get; set; } = string.Empty;
        [Required]
        public string CreationName { get; set; } = string.Empty;
        [Required]
        public DateTime? CreationDate { get; set; }
        [Required]
        public string RevisionName { get; set; } = string.Empty;
        [Required]
        public DateTime? RevisionDate { get; set; }
        [Required]
        public string DealName { get; set; } = string.Empty;
        [Required]
        public string DealType { get; set; } = string.Empty;
        [Required]
        public string SourceListId { get; set; } = string.Empty;
        [Required]
        public string Side { get; set; } = string.Empty;
    }

}
