using System;
using System.Text;
using System.Threading.Tasks;
using DotNetCoreSqlDb.Services.CvAnalysis.Interfaces;

namespace DotNetCoreSqlDb.Services.CvAnalysis.Extractors
{
    public sealed class PdfTextExtractor : ICvTextExtractor
    {
        public Task<string> ExtractTextAsync(byte[] fileBytes, string fileNameOrMime)
        {
            // Minimal placeholder PDF text extraction: attempts naive UTF8 decode.
            // In production, use a library like UglyToad.PdfPig.
            try
            {
                var text = Encoding.UTF8.GetString(fileBytes);
                return Task.FromResult(text);
            }
            catch (Exception)
            {
                return Task.FromResult(string.Empty);
            }
        }
    }
}
