using System.Text;
using System.Threading.Tasks;
using DotNetCoreSqlDb.Services.CvAnalysis.Interfaces;

namespace DotNetCoreSqlDb.Services.CvAnalysis.Extractors
{
    public sealed class TxtTextExtractor : ICvTextExtractor
    {
        public Task<string> ExtractTextAsync(byte[] fileBytes, string fileNameOrMime)
        {
            return Task.FromResult(Encoding.UTF8.GetString(fileBytes));
        }
    }
}
