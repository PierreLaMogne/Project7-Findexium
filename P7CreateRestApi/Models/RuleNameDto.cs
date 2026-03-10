using System.ComponentModel.DataAnnotations;

namespace FindexiumAPI.Models
{
    public class RuleNameDto
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Description { get; set; } = string.Empty;
        [Required]
        public string Json { get; set; } = string.Empty;
        [Required]
        public string Template { get; set; } = string.Empty;
        [Required]
        public string SqlStr { get; set; } = string.Empty;
        [Required]
        public string SqlPart { get; set; } = string.Empty;
    }
}
