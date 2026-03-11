using System.ComponentModel.DataAnnotations;

namespace FindexiumAPI.Models
{
    public class CurvePointDto
    {
        public int Id { get; set; }
        [Required]
        [RegularExpression(@"^\d+$", ErrorMessage = "Only digits are allowed.")]
        [Range(0,255, ErrorMessage = "CurveId must be between 0 and 255.")]
        public byte? CurveId { get; set; }
        [Required]
        [RegularExpression(@"^\d+$", ErrorMessage = "Only digits are allowed.")]
        public double? Term { get; set; }
        [Required]
        [RegularExpression(@"^\d+$", ErrorMessage = "Only digits are allowed.")]
        public double? CurvePointValue { get; set; }
    }
}
