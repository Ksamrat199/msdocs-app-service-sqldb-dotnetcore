using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetCoreSqlDb.Models.CvAnalysis;
using DotNetCoreSqlDb.Services.CvAnalysis.Interfaces;

namespace DotNetCoreSqlDb.Services.CvAnalysis
{
    public sealed class CvAnalyzer : ICvAnalyzer
    {
        private readonly ICvTextExtractor _textExtractor;
        private readonly ISkillExtractor _skillExtractor;

        public CvAnalyzer(ICvTextExtractor textExtractor, ISkillExtractor skillExtractor)
        {
            _textExtractor = textExtractor;
            _skillExtractor = skillExtractor;
        }

        public async Task<CvAnalysisResult> AnalyzeAsync(IReadOnlyList<CvInput> inputs)
        {
            var result = new CvAnalysisResult();
            if (inputs == null || inputs.Count == 0)
            {
                result.Warnings.Add("No inputs provided.");
                return result;
            }

            foreach (var input in inputs)
            {
                var text = await _textExtractor.ExtractTextAsync(input.Bytes, input.FileName);
                var skills = await _skillExtractor.ExtractSkillsAsync(text);

                var categoryScores = skills
                    .GroupBy(s => s.Category, StringComparer.OrdinalIgnoreCase)
                    .ToDictionary(g => g.Key, g => g.Sum(s => s.Count), StringComparer.OrdinalIgnoreCase);

                var summary = AnalysisSummary.FromCategoryScores(categoryScores);

                var profile = new StudentProfile
                {
                    Name = string.IsNullOrWhiteSpace(input.Name) ? System.IO.Path.GetFileNameWithoutExtension(input.FileName) : input.Name,
                    FileName = input.FileName,
                    ExtractedText = text,
                    Skills = skills.ToList(),
                    Summary = summary
                };

                result.Profiles.Add(profile);
            }

            return result;
        }
    }
}
