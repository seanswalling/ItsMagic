using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ItsMagic
{
    static class RegexStore
    {
        public const string ItemGroupTag = "<ItemGroup>";
        public const string PackagesTag = "<packages>";
        public const string CsFilesFromCsProjPattern = "(?:<Compile Include=\\\")(?<capturegroup>(?:(?:\\w+)\\\\*(?:\\w+))*\\.cs)(?:\\\" \\/>)";
        public const string CsProjFromSlnPattern = "Project(.*) = .*, \"(?<capturegroup>.*\\.csproj)\", \".*\"";

        public static IEnumerable<string> Get(string pattern, string file)
        {
            Regex regex = new Regex(pattern);
            return File.ReadAllLines(file)
                .Select(line => regex.Match(line))
                .Where(match => match.Success)
                .Select(match => match.Groups["capturegroup"].Value);
        }
    }
}
