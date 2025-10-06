using System.Collections.Generic;
using System.Threading.Tasks;
using DotNetCoreSqlDb.Models.CvAnalysis;

namespace DotNetCoreSqlDb.Services.CvAnalysis.Interfaces
{
    public interface IGroupingService
    {
        Task<IReadOnlyList<StudentGroup>> GroupBySkillsAsync(IReadOnlyList<StudentProfile> profiles, string strategy = "category-top");
    }
}
