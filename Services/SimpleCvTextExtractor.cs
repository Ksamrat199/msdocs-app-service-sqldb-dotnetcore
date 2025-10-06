using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using UglyToad.PdfPig;
using Xceed.Words.NET;

namespace DotNetCoreSqlDb.Services
{
    // Minimal extractor: reads plain text and PDFs via PdfPig if available later
    public class SimpleCvTextExtractor : ICvTextExtractor
    {
        public async Task<string> ExtractTextAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return string.Empty;
            }

            var contentType = file.ContentType?.ToLowerInvariant() ?? string.Empty;
            if (contentType.Contains("text/plain") || file.FileName.EndsWith(".txt", System.StringComparison.OrdinalIgnoreCase))
            {
                using var stream = file.OpenReadStream();
                using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
                return await reader.ReadToEndAsync();
            }

            if (contentType.Contains("pdf") || file.FileName.EndsWith(".pdf", System.StringComparison.OrdinalIgnoreCase))
            {
                using var stream = file.OpenReadStream();
                using var ms = new MemoryStream();
                await stream.CopyToAsync(ms);
                ms.Position = 0;
                var sb = new StringBuilder();
                using (var document = PdfDocument.Open(ms))
                {
                    foreach (var page in document.GetPages())
                    {
                        sb.AppendLine(page.Text);
                    }
                }
                return sb.ToString();
            }

            if (contentType.Contains("word") || contentType.Contains("officedocument") || file.FileName.EndsWith(".docx", System.StringComparison.OrdinalIgnoreCase))
            {
                using var stream = file.OpenReadStream();
                using var ms = new MemoryStream();
                await stream.CopyToAsync(ms);
                ms.Position = 0;
                using var doc = DocX.Load(ms);
                return doc.Text ?? string.Empty;
            }

            // Fallback: just store no text for unsupported types (we'll extend to PDF/DOCX later)
            return string.Empty;
        }
    }
}
