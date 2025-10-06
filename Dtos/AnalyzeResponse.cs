using System.Collections.Generic;
using DotNetCoreSqlDb.Models.CvAnalysis;

namespace DotNetCoreSqlDb.Dtos
{
    public sealed class AnalyzeResponse
    {
        public CvAnalysisResult Result { get; set; } = new CvAnalysisResult();
        public List<StudentGroup> Groups { get; set; } = new List<StudentGroup>();
    }
}
