using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ItsMagic
{
    static class RegexStore
    {
        public const string ItemGroupTag = "<ItemGroup>";
        public const string PackagesTag = "<packages>";
        public const string EndProject = "EndProject";
        public const string PackagesConfigDefault = "<? xml version =\"1.0\" encoding=\"utf-8\"?><packages></packages>";
        public const string ReleaseAnyCpu = "Release|Any CPU";
        public const string NestedProjects = "GlobalSection(NestedProjects) = preSolution";

        public const string CsFilesFromCsProjPattern = "<Compile Include=\\\"(?<capturegroup>(.*.cs))\\\"( \\/)*>";
        public const string CsProjFromSlnPattern = "Project(.*) = .*, \\\"(?<capturegroup>.*\\.csproj)\\\", \\\".*\\\"";
        public const string UsingsFromCsFilePattern = "using (?<capturegroup>(\\w+\\.*)*);";
        public const string SolutionJExtProjectReferencePattern = "Project.*\\\"Mercury\\.Core\\.JsonExtensions\\\"";
        public const string SolutionNHibExtProjectReferencePattern = "Project.*\\\"Mercury\\.Core\\.NHibernateExtensions\\\"";
        public const string CommonFolderPattern = "Project.* = \\\"Common\\\", \\\"Common\\\", \\\"\\{(?<capturegroup>(.*))\\}\\\"";


        public const string SolutionJExtProjectReference =
            "Project(\"{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}\") = \"Mercury.Core.JsonExtensions\", \"Platform\\Mercury.Core.JsonExtensions\\Mercury.Core.JsonExtensions.csproj\", \"{D3DC56B0-8B95-47A5-A086-9E7A95552364}\"\nEndProject";
        public const string SolutionNHibExtProjectReference = 
            "Project(\"{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}\") = \"Mercury.Core.NHibernateExtensions\", \"Platform\\Mercury.Core.NHibernateExtensions\\Mercury.Core.NHibernateExtensions.csproj\", \"{F1575997-02D0-486F-AE36-69F6A3B37C39}\"\nEndProject";
        public const string JExtReleaseDebugInformation = "{D3DC56B0-8B95-47A5-A086-9E7A95552364}.Debug|Any CPU.ActiveCfg = Debug|Any CPU\r\n\t\t" +
                                                          "{D3DC56B0-8B95-47A5-A086-9E7A95552364}.Debug|Any CPU.Build.0 = Debug|Any CPU\r\n\t\t" +
                                                          "{D3DC56B0-8B95-47A5-A086-9E7A95552364}.Release|Any CPU.ActiveCfg = Release|Any CPU\r\n\t\t" +
                                                          "{D3DC56B0-8B95-47A5-A086-9E7A95552364}.Release|Any CPU.Build.0 = Release|Any CPU";

        public static string NHibExtReleaseDebugInformation = "{F1575997-02D0-486F-AE36-69F6A3B37C39}.Debug|Any CPU.ActiveCfg = Debug|Any CPU\r\n\t\t" +
                                                          "{F1575997-02D0-486F-AE36-69F6A3B37C39}.Debug|Any CPU.Build.0 = Debug|Any CPU\r\n\t\t" +
                                                          "{F1575997-02D0-486F-AE36-69F6A3B37C39}.Release|Any CPU.ActiveCfg = Release|Any CPU\r\n\t\t" +
                                                          "{F1575997-02D0-486F-AE36-69F6A3B37C39}.Release|Any CPU.Build.0 = Release|Any CPU";

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

        public static string ReplaceLastOccurrence(string source, string find, string Replace)
        {
            int place = source.LastIndexOf(find, StringComparison.Ordinal);

            if (place == -1)
                return source;

            string result = source.Remove(place, find.Length).Insert(place, Replace);
            return result;
        }
    }
}
