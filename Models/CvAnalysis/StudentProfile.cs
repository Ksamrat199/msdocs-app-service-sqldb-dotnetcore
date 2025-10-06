using System;
using System.Collections.Generic;

namespace DotNetCoreSqlDb.Models.CvAnalysis
{
    public sealed class StudentProfile
    {
        public Guid Id { get; init; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string ExtractedText { get; set; } = string.Empty;
        public List<SkillOccurrence> Skills { get; set; } = new();
        public AnalysisSummary Summary { get; set; } = new AnalysisSummary();
    }
}
