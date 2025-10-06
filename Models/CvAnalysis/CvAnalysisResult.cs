using System.Collections.Generic;

namespace DotNetCoreSqlDb.Models.CvAnalysis
{
    public sealed class CvAnalysisResult
    {
        public List<StudentProfile> Profiles { get; set; } = new();
        public List<string> Warnings { get; set; } = new();
    }
}
