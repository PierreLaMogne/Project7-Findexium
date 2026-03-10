using System.ComponentModel.DataAnnotations;

namespace FindexiumAPI.Models
{
    public class UserDto
    {
        public string Id { get; set; } = string.Empty;

        [Required]
        public string UserName { get; set; } = string.Empty;
        [Required]
        public string FullName { get; set; } = string.Empty;
        [Required]
        public string Role { get; set; } = string.Empty;
    }
}
