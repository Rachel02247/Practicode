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
            {"C#", ".cs" },
            { "Java" , ".java"},
            { "Python" , ".py" },
            { "C++" , ".cpp" },
            { "JavaScript" , ".js" },
            { "TypeScript" , ".ts" },
            { "HTML" , ".html" },
            { "CSS" , ".css" },
            { "SQL" , ".sql" }
        };
    }
}
