using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetCoreSqlDb.Models.CvAnalysis;
using DotNetCoreSqlDb.Services.CvAnalysis;
using Xunit;

namespace DotNetCoreSqlDb.Tests
{
    public class GroupingServiceTests
    {
        [Fact]
        public async Task Groups_By_Top_Category()
        {
            var service = new GroupingService();

            var profileA = new StudentProfile
            {
                Name = "Alice",
                Summary = AnalysisSummary.FromCategoryScores(new Dictionary<string, int>
                {
                    ["Programming Languages"] = 3,
                    ["Web"] = 1
                })
            };

            var profileB = new StudentProfile
            {
                Name = "Bob",
                Summary = AnalysisSummary.FromCategoryScores(new Dictionary<string, int>
                {
                    ["Web"] = 2,
                    ["Programming Languages"] = 1
                })
            };

            var groups = await service.GroupBySkillsAsync(new List<StudentProfile> { profileA, profileB });

            var langGroup = groups.FirstOrDefault(g => g.Label == "Programming Languages");
            var webGroup = groups.FirstOrDefault(g => g.Label == "Web");

            Assert.NotNull(langGroup);
            Assert.NotNull(webGroup);
            Assert.Contains(langGroup!.Members, p => p.Name == "Alice");
            Assert.Contains(webGroup!.Members, p => p.Name == "Bob");
        }
    }
}
