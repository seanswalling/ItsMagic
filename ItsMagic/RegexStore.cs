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
        public const string TestsCoreGuid = "AF2AA63F-B129-4D88-9D1A-4BC19E443B00"; //deprecate later
        public const string TestsSharedGuid = "CA12242D-1F50-44BB-9972-D6C6609E4C37"; //deprecate later
        public static string ItemGroupProjectReference = "<ItemGroup>" + Environment.NewLine + "<ProjectReference ";

        public const string CsFilesFromCsProjPattern = "<Compile Include=\\\"(?<capturegroup>(.*.cs))\\\"( \\/)*>"; 
        public const string CsProjFromSlnPattern = "Project(.*) = .*, \\\"(?<capturegroup>.*\\.csproj)\\\", \\\".*\\\"";
        public const string UsingsFromCsFilePattern = "using (?<capturegroup>(.*));";
        public const string ExtensionMethodsFromCsFilePattern = " (?<capturegroup>(\\w|\\d)*)\\(this";
        public const string ClassFromCsFilePattern = "class (?<capturegroup>(\\w*\\d*))";
        public const string CsProjGuidPattern = "<ProjectGuid>{(?<capturegroup>([\\d\\w-]*))}<\\/ProjectGuid>";
        public const string ItemGroupProjectReferencepattern = "<ItemGroup>\\s+<ProjectReference ";
        public const string EndGlobalSection = "\\}\\s+EndGlobalSection";
        public const string SolutionFolderNamePattern = "Project\\(\\\"\\{2150E333-8FDC-42A3-9474-1A3956D46DE8\\}\\\"\\) = \\\"(?<capturegroup>(\\w+))\\\"";
        public const string CsProjPathFromCsProjPattern = "\"(?<capturegroup>(.*\\.csproj))\"";
        public const string PackageFromPackagesConfigPattern = "(?<capturegroup>(<package.*\\/>))";
        public const string PackageIdFromPackagesPattern = "id=\"(?<capturegroup>((\\w+|\\.*)*))\"";
        public const string PackageVersionFromPackagesPattern = "version=\"(?<capturegroup>((\\d+|\\.*)*))\"";
        public const string PackageTargetFrameworkFromPackagesPattern = "targetFramework=\"(?<capturegroup>(.*))\"";


        public static IEnumerable<string> Get2(string pattern, string file)
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

        public static IEnumerable<string> Get(string pattern, string text)
        {
            Regex regex = new Regex(pattern);
            var matchCollection = regex.Matches(text);
            return matchCollection.Cast<Match>().Select(m => m.Groups["capturegroup"].Value);
        }

        public static string Either(string pattern1, string pattern2)
        {
            return pattern1 + "|" + pattern2;
        }

        public static bool Contains(string pattern, string inputText)
        {
            Regex regex = new Regex(pattern);
            var matches = regex.Matches(inputText);
            if (matches.Count > 0)
            {
                return true;
            }
            return false;
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
