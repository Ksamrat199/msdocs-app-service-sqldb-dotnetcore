using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DotNetCoreSqlDb.Models.CvAnalysis;
using DotNetCoreSqlDb.Services.CvAnalysis.Interfaces;

namespace DotNetCoreSqlDb.Services.CvAnalysis
{
    public sealed class SkillExtractor : ISkillExtractor
    {
        private readonly IReadOnlyDictionary<string, string[]> _categoryToSkills;
        private readonly Dictionary<string, string> _skillToCategory;
        private readonly Regex _wordBoundary = new Regex(@"\b", RegexOptions.Compiled);

        public SkillExtractor(IReadOnlyDictionary<string, string[]>? categoryToSkills = null)
        {
            _categoryToSkills = categoryToSkills ?? SkillTaxonomy.CategoryToSkills;
            _skillToCategory = _categoryToSkills
                .SelectMany(kv => kv.Value.Select(skill => new KeyValuePair<string, string>(skill, kv.Key)))
                .ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase);
        }

        public Task<IReadOnlyList<SkillOccurrence>> ExtractSkillsAsync(string plainText)
        {
            if (string.IsNullOrWhiteSpace(plainText))
            {
                return Task.FromResult<IReadOnlyList<SkillOccurrence>>(Array.Empty<SkillOccurrence>());
            }

            var text = plainText.ToLowerInvariant();
            var counts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            foreach (var skill in _skillToCategory.Keys)
            {
                // simple contains with word boundaries where possible
                var pattern = $@"(?<![a-z0-9+]){Regex.Escape(skill)}(?![a-z0-9+])";
                var matches = Regex.Matches(text, pattern, RegexOptions.IgnoreCase);
                if (matches.Count > 0)
                {
                    counts[skill] = matches.Count;
                }
            }

            var occurrences = counts
                .Select(kv => new SkillOccurrence
                {
                    Skill = kv.Key,
                    Category = _skillToCategory[kv.Key],
                    Count = kv.Value
                })
                .OrderByDescending(o => o.Count)
                .ThenBy(o => o.Skill)
                .ToList();

            return Task.FromResult<IReadOnlyList<SkillOccurrence>>(occurrences);
        }
    }
}
