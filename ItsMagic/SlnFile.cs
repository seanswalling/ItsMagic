using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ItsMagic
{
    public class SlnFile
    {
        public string Path { get; private set; }

        public SlnFile(string path)
        {
            Path = path;
        }

        //public static IEnumerable<CsProj> GetCsProjs(string slnPath)
        //{
        //    var dir = System.IO.Path.GetDirectoryName(slnPath);
        //    return RegexStore.Get(RegexStore.CsProjFromSlnPattern,slnPath)
        //        .Select(CsProjRelPath => System.IO.Path.Combine(dir, CsProjRelPath))
        //        .Select(CsProjFilePath => new CsProj(CsProjFilePath));
        //}

        public IEnumerable<CsProj> GetCsProjs()
        {
            Console.WriteLine("Get Csproj Files for: " + Path);
            var dir = System.IO.Path.GetDirectoryName(Path);
            return RegexStore.Get(RegexStore.CsProjFromSlnPattern, Path)
                .Select(csProjRelPath => System.IO.Path.Combine(dir, csProjRelPath))
                .Select(file => new CsProj(file));
        }

        public bool ContainsJExtProjectReference()
        {
            var solutionFileText = File.ReadAllText(Path);
            Regex regex = new Regex(RegexStore.SolutionJExtProjectReferencePattern);
            Match match = regex.Match(solutionFileText);
            return match.Success;
        }

        public bool ContainsNHibExtProjectReference()
        {
            var solutionFileText = File.ReadAllText(Path);
            Regex regex = new Regex(RegexStore.SolutionNHibExtProjectReferencePattern);
            Match match = regex.Match(solutionFileText);
            return match.Success;
        }

        public void AddJExtProjectReference()
        {
            var solutionFileText = File.ReadAllText(Path);

            solutionFileText = AddJExtProjectText(solutionFileText);
            solutionFileText = AddJExtDebugAndReleaseInformation(solutionFileText);
            solutionFileText = AddJExtToCommonFolder(solutionFileText);

            File.WriteAllText(Path, solutionFileText);
        }

        private static string AddJExtProjectText(string solutionFileText)
        {
            solutionFileText = RegexStore.ReplaceLastOccurrence(solutionFileText,
                RegexStore.EndProject,
                RegexStore.EndProject+ "\n" + RegexStore.SolutionJExtProjectReference);
            return solutionFileText;
        }

        private static string AddJExtDebugAndReleaseInformation(string solutionFileText)
        {
            solutionFileText = RegexStore.ReplaceLastOccurrence(solutionFileText, 
                RegexStore.ReleaseAnyCpu,
                RegexStore.ReleaseAnyCpu + "\n\t\t" + RegexStore.JExtReleaseDebugInformation);
            return solutionFileText;
        }

        private string AddJExtToCommonFolder(string solutionFileText)
        {
            string jExtProjEqualsCommonFolder = "{D3DC56B0-8B95-47A5-A086-9E7A95552364} = {" + RegexStore.Get(RegexStore.CommonFolderPattern, Path).First() + "}";
            solutionFileText = solutionFileText.Replace(RegexStore.NestedProjects,RegexStore.NestedProjects + "\n\t\t" + jExtProjEqualsCommonFolder);
            return solutionFileText;
        }

        public void AddNHibExtProjectReference()
        {
            var solutionFileText = File.ReadAllText(Path);

            solutionFileText = AddNHibExtProjectText(solutionFileText);
            solutionFileText = AddNHibExtDebugAndReleaseInformation(solutionFileText);
            solutionFileText = AddNHibExtToCommonFolder(solutionFileText);

            File.WriteAllText(Path, solutionFileText);
        }

        private static string AddNHibExtProjectText(string solutionFileText)
        {
            solutionFileText = RegexStore.ReplaceLastOccurrence(solutionFileText,
                RegexStore.EndProject,
                RegexStore.EndProject + "\n" + RegexStore.SolutionNHibExtProjectReference);
            return solutionFileText;
        }

        private static string AddNHibExtDebugAndReleaseInformation(string solutionFileText)
        {
            solutionFileText = RegexStore.ReplaceLastOccurrence(solutionFileText,
                RegexStore.ReleaseAnyCpu,
                RegexStore.ReleaseAnyCpu + "\n\t\t" + RegexStore.NHibExtReleaseDebugInformation);
            return solutionFileText;
        }

        private string AddNHibExtToCommonFolder(string solutionFileText)
        {
            string nHibExtProjEqualsCommonFolder = "{F1575997-02D0-486F-AE36-69F6A3B37C39} = {" + RegexStore.Get(RegexStore.CommonFolderPattern, Path).First() + "}";
            solutionFileText = solutionFileText.Replace(RegexStore.NestedProjects, RegexStore.NestedProjects + "\n\t\t" + nHibExtProjEqualsCommonFolder);
            return solutionFileText;
        }
    }
}