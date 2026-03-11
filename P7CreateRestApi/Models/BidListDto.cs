using System.ComponentModel.DataAnnotations;

namespace FindexiumAPI.Models
{
    public class BidListDto
    {    
        public int BidListId { get; set; }
        [Required]
        public string Account { get; set; } = string.Empty;
        [Required]
        public string BidType { get; set; } = string.Empty;
        [Required]
        [RegularExpression(@"^\d+$", ErrorMessage = "Only digits are allowed.")]
        public double? BidQuantity { get; set; }
    }
}
