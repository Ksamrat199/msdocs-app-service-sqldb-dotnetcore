using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DotNetCoreSqlDb.Services
{
    public interface ICvTextExtractor
    {
        Task<string> ExtractTextAsync(IFormFile file);
    }
}
