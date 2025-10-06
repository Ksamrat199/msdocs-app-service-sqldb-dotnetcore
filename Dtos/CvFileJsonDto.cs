using System;
using System.ComponentModel.DataAnnotations;

namespace DotNetCoreSqlDb.Dtos
{
    public sealed class CvFileJsonDto
    {
        [Required]
        public string FileName { get; init; } = string.Empty;

        public string Name { get; init; } = string.Empty;

        [Required]
        public string Base64 { get; init; } = string.Empty;
    }
}
