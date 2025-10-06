using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetCoreSqlDb.Models.CvAnalysis;
using DotNetCoreSqlDb.Services.CvAnalysis.Interfaces;

namespace DotNetCoreSqlDb.Services.CvAnalysis
{
    public sealed class GroupingService : IGroupingService
    {
        public Task<IReadOnlyList<StudentGroup>> GroupBySkillsAsync(IReadOnlyList<StudentProfile> profiles, string strategy = "category-top")
        {
            if (profiles == null || profiles.Count == 0)
            {
                return Task.FromResult<IReadOnlyList<StudentGroup>>(Array.Empty<StudentGroup>());
            }

            if (string.Equals(strategy, "category-top", StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult<IReadOnlyList<StudentGroup>>(GroupByTopCategory(profiles));
            }

            // default fallback same as category-top
            return Task.FromResult<IReadOnlyList<StudentGroup>>(GroupByTopCategory(profiles));
        }

        private static IReadOnlyList<StudentGroup> GroupByTopCategory(IReadOnlyList<StudentProfile> profiles)
        {
            var buckets = new Dictionary<string, List<StudentProfile>>(StringComparer.OrdinalIgnoreCase);

            foreach (var profile in profiles)
            {
                var top = profile.Summary?.TopCategories?.FirstOrDefault();
                var key = string.IsNullOrWhiteSpace(top) ? "Uncategorized" : top;
                if (!buckets.TryGetValue(key, out var list))
                {
                    list = new List<StudentProfile>();
                    buckets[key] = list;
                }
                list.Add(profile);
            }

            return buckets
                .OrderByDescending(kv => kv.Value.Count)
                .Select(kv => new StudentGroup
                {
                    Label = kv.Key,
                    Strategy = "category-top",
                    Members = kv.Value
                })
                .ToList();
        }
    }
}
