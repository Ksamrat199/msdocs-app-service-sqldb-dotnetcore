using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DotNetCoreSqlDb.Services.CvAnalysis.Interfaces;

namespace DotNetCoreSqlDb.Services.CvAnalysis.Extractors
{
    public sealed class DocxTextExtractor : ICvTextExtractor
    {
        public async Task<string> ExtractTextAsync(byte[] fileBytes, string fileNameOrMime)
        {
            // Simple DOCX text extraction without external libs by reading document.xml
            await using var ms = new MemoryStream(fileBytes);
            using var archive = new ZipArchive(ms, ZipArchiveMode.Read, leaveOpen: false);
            var entry = archive.GetEntry("word/document.xml");
            if (entry == null)
            {
                return string.Empty;
            }

            await using var stream = entry.Open();
            using var reader = new StreamReader(stream, Encoding.UTF8);
            var xml = await reader.ReadToEndAsync();

            // Strip XML tags and normalize whitespace
            var noTags = Regex.Replace(xml, "<[^>]+>", " ");
            var text = Regex.Replace(noTags, "\s+", " ").Trim();
            return text;
        }
    }
}
