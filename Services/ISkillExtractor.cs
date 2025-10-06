using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotNetCoreSqlDb.Services
{
    public interface ISkillExtractor
    {
        Task<IReadOnlyList<string>> ExtractSkillsAsync(string text);
    }
}
