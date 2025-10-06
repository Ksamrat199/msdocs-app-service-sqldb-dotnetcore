using System.Collections.Generic;
using System.Threading.Tasks;
using DotNetCoreSqlDb.Models.CvAnalysis;

namespace DotNetCoreSqlDb.Services.CvAnalysis.Interfaces
{
    public interface ICvAnalyzer
    {
        Task<CvAnalysisResult> AnalyzeAsync(IReadOnlyList<CvInput> inputs);
    }
}
