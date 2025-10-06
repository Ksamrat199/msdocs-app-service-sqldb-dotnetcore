using System;

namespace DotNetCoreSqlDb.Models.CvAnalysis
{
    public sealed class SkillOccurrence
    {
        public string Skill { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}
