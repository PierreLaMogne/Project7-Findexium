using System.ComponentModel.DataAnnotations;

namespace FindexiumAPI.Models
{
    public class ChangePasswordDto
    {
        [Required]
        public string Id { get; set; } = string.Empty;
        [Required]
        public string CurrentPassword { get; set; } = string.Empty;
        [Required]
        public string NewPassword { get; set; } = string.Empty;
        [Required]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
