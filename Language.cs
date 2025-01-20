using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pack
{
    internal static class Language
    {
        public static Dictionary<string, string> ProgrammingLanguages = new Dictionary<string, string>
        {
            { ".cs", "C#" },
            { ".java", "Java" },
            { ".py", "Python" },
            { ".cpp", "C++" },
            { ".js", "JavaScript" },
            { ".ts", "TypeScript" },
            { ".html", "HTML" },
            { ".css", "CSS" },
            { ".sql", "SQL" }
        };
    }
}
