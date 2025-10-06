using System;

namespace DotNetCoreSqlDb.Models.CvAnalysis
{
    public sealed class CvInput
    {
        public string Name { get; init; } = string.Empty;
        public string FileName { get; init; } = string.Empty;
        public byte[] Bytes { get; init; } = Array.Empty<byte>();
    }
}
