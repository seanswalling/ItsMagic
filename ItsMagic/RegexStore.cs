using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ItsMagic
{
    static class RegexStore
    {
        public const string ItemGroupTag = "<ItemGroup>";
        public const string PackagesTag = "<packages>";
        public const string CsFilesFromCsProjPattern = "<Compile Include=\\\"(?<capturegroup>(.*.cs))\\\"( \\/)*>";
        public const string CsProjFromSlnPattern = "Project(.*) = .*, \"(?<capturegroup>.*\\.csproj)\", \".*\"";
        public const string UsingsFromCsFilePattern = "using (?<capturegroup>(\\w+\\.*)*);";
        public const string PackagesConfigDefault = "<? xml version =\"1.0\" encoding=\"utf-8\"?><packages></packages>";

        public static IEnumerable<string> Get(string pattern, string file)
        {
            Regex regex = new Regex(pattern);
            foreach (var match in Dumbledore.ReadLines(file)
                .Select(line => regex.Match(line))
                .Where(match => match.Success)
                .Select(match => match.Groups["capturegroup"].Value))
            {
                yield return match;
            }
        }
    }
}
