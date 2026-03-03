namespace FindexiumAPI.Models
{
    public class RuleNameDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Json { get; set; } = string.Empty;
        public string Template { get; set; } = string.Empty;
        public string SqlStr { get; set; } = string.Empty;
        public string SqlPart { get; set; } = string.Empty;
    }
}
