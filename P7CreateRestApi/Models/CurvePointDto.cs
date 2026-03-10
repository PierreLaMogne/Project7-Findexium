using System.ComponentModel.DataAnnotations;

namespace FindexiumAPI.Models
{
    public class CurvePointDto
    {
        public int Id { get; set; }
        [Required]
        public byte? CurveId { get; set; }
        [Required]
        public double? Term { get; set; }
        [Required]
        public double? CurvePointValue { get; set; }
    }
}
