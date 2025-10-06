using System.Collections.Generic;

namespace DotNetCoreSqlDb.Models.CvAnalysis
{
    public sealed class StudentGroup
    {
        public string Label { get; set; } = string.Empty; // Category or Group ID
        public string Strategy { get; set; } = string.Empty;
        public List<StudentProfile> Members { get; set; } = new();
    }
}
