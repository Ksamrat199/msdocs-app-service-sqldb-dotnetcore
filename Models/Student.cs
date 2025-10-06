using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DotNetCoreSqlDb.Models
{
    public class Student
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string FullName { get; set; } = string.Empty;

        [EmailAddress]
        [StringLength(200)]
        public string? Email { get; set; }

        [StringLength(500)]
        public string? UploadedFileName { get; set; }

        [StringLength(200)]
        public string? UploadedContentType { get; set; }

        public DateTime UploadedAtUtc { get; set; } = DateTime.UtcNow;

        // Raw text extracted from CV; can be null if not processed yet
        public string? ExtractedText { get; set; }

        // JSON array of canonical skills, e.g. ["c#","asp.net core"]
        public string? ExtractedSkillsJson { get; set; }

        [NotMapped]
        public IReadOnlyList<string> ExtractedSkills
        {
            get
            {
                if (string.IsNullOrWhiteSpace(ExtractedSkillsJson))
                {
                    return Array.Empty<string>();
                }

                try
                {
                    return System.Text.Json.JsonSerializer.Deserialize<List<string>>(ExtractedSkillsJson) ?? new List<string>();
                }
                catch
                {
                    return Array.Empty<string>();
                }
            }
        }

        public ICollection<GroupMember> GroupMemberships { get; set; } = new List<GroupMember>();
    }
}
