using System.Threading.Tasks;

namespace DotNetCoreSqlDb.Services.CvAnalysis.Interfaces
{
    public interface ICvTextExtractor
    {
        Task<string> ExtractTextAsync(byte[] fileBytes, string fileNameOrMime);
    }
}
