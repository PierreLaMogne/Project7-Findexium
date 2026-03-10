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
        public byte? OrderNumber { get; set; }
    }
}
