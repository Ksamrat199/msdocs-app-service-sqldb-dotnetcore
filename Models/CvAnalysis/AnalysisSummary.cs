using System;
using System.Collections.Generic;
using System.Linq;

namespace DotNetCoreSqlDb.Models.CvAnalysis
{
    public sealed class AnalysisSummary
    {
        public IReadOnlyDictionary<string, int> CategoryScores { get; init; } = new Dictionary<string, int>();

        public IReadOnlyList<string> TopCategories { get; init; } = Array.Empty<string>();

        public static AnalysisSummary FromCategoryScores(Dictionary<string, int> categoryScores, int topN = 3)
        {
            var sorted = categoryScores
                .OrderByDescending(kv => kv.Value)
                .ThenBy(kv => kv.Key)
                .ToList();

            var top = sorted
                .Where(kv => kv.Value > 0)
                .Take(topN)
                .Select(kv => kv.Key)
                .ToList();

            return new AnalysisSummary
            {
                CategoryScores = new Dictionary<string, int>(categoryScores),
                TopCategories = top
            };
        }
    }
}
