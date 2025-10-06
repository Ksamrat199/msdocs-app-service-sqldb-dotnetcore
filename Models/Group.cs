using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DotNetCoreSqlDb.Models
{
    public class Group
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        public ICollection<GroupMember> Members { get; set; } = new List<GroupMember>();

        // JSON for aggregated group skills computed from members
        public string? AggregatedSkillsJson { get; set; }
    }
}
