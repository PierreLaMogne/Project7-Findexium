using System.ComponentModel.DataAnnotations;

namespace FindexiumAPI.Models
{
    public class RatingDto
    {
        public int Id { get; set; }
        [Required]
        public string MoodysRating { get; set; } = string.Empty;
        [Required]
        public string SandPRating { get; set; } = string.Empty;
        [Required]
        public string FitchRating { get; set; } = string.Empty;
        [Required]
        [RegularExpression(@"^\d+$", ErrorMessage = "Only digits are allowed.")]
        [Range(0, 255, ErrorMessage = "CurveId must be between 0 and 255.")]
        public byte? OrderNumber { get; set; }
    }
}
