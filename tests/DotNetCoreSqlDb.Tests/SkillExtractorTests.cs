using System.Linq;
using System.Threading.Tasks;
using DotNetCoreSqlDb.Services.CvAnalysis;
using Xunit;

namespace DotNetCoreSqlDb.Tests
{
    public class SkillExtractorTests
    {
        [Fact]
        public async Task Extracts_Expected_Skills_And_Categories()
        {
            var extractor = new SkillExtractor();
            var text = "Experienced in C#, .NET, Azure, SQL Server, and ASP.NET MVC.";

            var skills = await extractor.ExtractSkillsAsync(text);

            Assert.Contains(skills, s => s.Skill.ToLower() == "c#" && s.Category == "Programming Languages" && s.Count >= 1);
            Assert.Contains(skills, s => s.Skill.ToLower() == ".net" && s.Category == "Programming Languages");
            Assert.Contains(skills, s => s.Skill.ToLower() == "azure" && s.Category == "Cloud & DevOps");
            Assert.Contains(skills, s => s.Skill.ToLower() == "sql server" && s.Category == "Data & ML");
            Assert.Contains(skills, s => s.Skill.ToLower() == "asp.net" && s.Category == "Web");
            Assert.Contains(skills, s => s.Skill.ToLower() == "mvc" && s.Category == "Web");
        }

        [Fact]
        public async Task Does_Not_Match_C_When_Text_Has_Cpp()
        {
            var extractor = new SkillExtractor();
            var text = "Strong in C++ and Rust.";

            var skills = await extractor.ExtractSkillsAsync(text);

            Assert.DoesNotContain(skills, s => s.Skill.ToLower() == "c");
            Assert.Contains(skills, s => s.Skill.ToLower() == "c++");
        }
    }
}
