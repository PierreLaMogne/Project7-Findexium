namespace FindexiumAPI.Domain
{
    public class RuleName
    {
        // DONE: Map columns in data table RULENAME with corresponding fields
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Json { get; set; } = string.Empty;
        public string Template { get; set; } = string.Empty;
        public string SqlStr { get; set; } = string.Empty;
        public string SqlPart { get; set; } = string.Empty;
    }
}