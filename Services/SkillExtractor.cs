using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DotNetCoreSqlDb.Services
{
    public class SkillExtractor : ISkillExtractor
    {
        private readonly HashSet<string> _canonicalSkills;
        private readonly Dictionary<string, string> _aliasToCanonical;
        private readonly Regex _tokenizer;

        public SkillExtractor()
        {
            // Lazy-load skills list from embedded json under wwwroot/data/skills.json if present
            var skillsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data", "skills.json");
            _canonicalSkills = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            _aliasToCanonical = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            _tokenizer = new Regex("[A-Za-z0-9#\+\.\-]+", RegexOptions.Compiled);

            if (File.Exists(skillsPath))
            {
                try
                {
                    using var stream = File.OpenRead(skillsPath);
                    var doc = JsonSerializer.Deserialize<SkillsDocument>(stream);
                    if (doc != null)
                    {
                        foreach (var s in doc.Skills ?? Array.Empty<string>())
                        {
                            if (!string.IsNullOrWhiteSpace(s))
                            {
                                _canonicalSkills.Add(s.Trim());
                            }
                        }
                        foreach (var kv in doc.Aliases ?? new Dictionary<string, string>())
                        {
                            if (!string.IsNullOrWhiteSpace(kv.Key) && !string.IsNullOrWhiteSpace(kv.Value))
                            {
                                _aliasToCanonical[kv.Key.Trim()] = kv.Value.Trim();
                            }
                        }
                    }
                }
                catch
                {
                    // Ignore load errors; extractor will operate with empty dictionaries
                }
            }
        }

        public Task<IReadOnlyList<string>> ExtractSkillsAsync(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return Task.FromResult<IReadOnlyList<string>>(Array.Empty<string>());
            }

            var found = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            // Token-based scan
            foreach (Match m in _tokenizer.Matches(text))
            {
                var token = m.Value.Trim();
                if (_aliasToCanonical.TryGetValue(token, out var canonical))
                {
                    found.Add(canonical);
                }
                else if (_canonicalSkills.Contains(token))
                {
                    found.Add(token);
                }
            }

            // Simple phrase scan for multi-word skills
            foreach (var candidate in _canonicalSkills.Where(s => s.Contains(' ')))
            {
                if (text.Contains(candidate, StringComparison.OrdinalIgnoreCase))
                {
                    found.Add(candidate);
                }
            }

            var result = found
                .Select(s => s.Trim())
                .Where(s => s.Length > 0)
                .OrderBy(s => s)
                .ToList();

            return Task.FromResult<IReadOnlyList<string>>(result);
        }

        private sealed class SkillsDocument
        {
            public string[]? Skills { get; set; }
            public Dictionary<string, string>? Aliases { get; set; }
        }
    }
}
