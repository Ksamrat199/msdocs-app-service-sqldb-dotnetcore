using System.Collections.Generic;

namespace DotNetCoreSqlDb.Services.CvAnalysis
{
    public static class SkillTaxonomy
    {
        // Simple built-in taxonomy; in real usage, source from config/db
        public static readonly IReadOnlyDictionary<string, string[]> CategoryToSkills = new Dictionary<string, string[]>
        {
            ["Programming Languages"] = new[] { "c#", ".net", "java", "python", "javascript", "typescript", "go", "rust", "c++", "c" },
            ["Web"] = new[] { "asp.net", "mvc", "entity framework", "sql", "rest", "graphql", "html", "css", "react", "angular", "vue" },
            ["Data & ML"] = new[] { "pandas", "numpy", "scikit-learn", "tensorflow", "pytorch", "nlp", "data analysis", "sql server" },
            ["Cloud & DevOps"] = new[] { "azure", "aws", "gcp", "docker", "kubernetes", "ci/cd", "github actions" },
        };
    }
}
