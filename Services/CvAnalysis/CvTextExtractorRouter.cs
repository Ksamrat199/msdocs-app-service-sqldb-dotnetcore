using System;
using System.Threading.Tasks;
using DotNetCoreSqlDb.Services.CvAnalysis.Interfaces;
using DotNetCoreSqlDb.Services.CvAnalysis.Extractors;

namespace DotNetCoreSqlDb.Services.CvAnalysis
{
    public sealed class CvTextExtractorRouter : ICvTextExtractor
    {
        private readonly TxtTextExtractor _txt = new();
        private readonly DocxTextExtractor _docx = new();
        private readonly PdfTextExtractor _pdf = new();

        public Task<string> ExtractTextAsync(byte[] fileBytes, string fileNameOrMime)
        {
            var lower = (fileNameOrMime ?? string.Empty).ToLowerInvariant();
            if (lower.EndsWith(".txt") || lower.Contains("text/plain"))
            {
                return _txt.ExtractTextAsync(fileBytes, fileNameOrMime);
            }
            if (lower.EndsWith(".docx") || lower.Contains("application/vnd.openxmlformats-officedocument.wordprocessingml.document"))
            {
                return _docx.ExtractTextAsync(fileBytes, fileNameOrMime);
            }
            if (lower.EndsWith(".pdf") || lower.Contains("application/pdf"))
            {
                return _pdf.ExtractTextAsync(fileBytes, fileNameOrMime);
            }
            // Fallback
            return _txt.ExtractTextAsync(fileBytes, fileNameOrMime);
        }
    }
}
