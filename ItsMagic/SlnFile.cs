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
        //public IEnumerable<CsProj> CsProjs { get; set; }

        public SlnFile(string path)
        {
            Path = path;
            //CsProjs = GetCsProjs(path);
        }

        //public static IEnumerable<CsProj> GetCsProjs(string slnPath)
        //{
        //    var dir = System.IO.Path.GetDirectoryName(slnPath);
        //    return RegexStore.Get(RegexStore.CsProjFromSlnPattern,slnPath)
        //        .Select(CsProjRelPath => System.IO.Path.Combine(dir, CsProjRelPath))
        //        .Select(CsProjFilePath => new CsProj(CsProjFilePath));
        //}

        public static IEnumerable<string> GetCsProjs(string slnPath)
        {
            Console.WriteLine("Get Csproj Files for: " + slnPath);
            var dir = System.IO.Path.GetDirectoryName(slnPath);
            return RegexStore.Get(RegexStore.CsProjFromSlnPattern, slnPath)
                .Select(csProjRelPath => System.IO.Path.Combine(dir, csProjRelPath));
        }

        public static bool ContainsJExtProjectReference(string solutionFile)
        {
            var solutionFileText = File.ReadAllText(solutionFile);
            Regex regex = new Regex(RegexStore.SolutionJExtProjectReferencePattern);
            Match match = regex.Match(solutionFileText);
            return match.Success;
        }

        public static void AddJExtProjectReference(string solutionFile)
        {
            var solutionFileText = File.ReadAllText(solutionFile);

            solutionFileText = AddProjectText(solutionFileText);
            solutionFileText = AddDebugAndReleaseInformation(solutionFileText);
            solutionFileText = AddToCommonFolder(solutionFileText, solutionFile);

            File.WriteAllText(solutionFile, solutionFileText);
        }

        private static string AddProjectText(string solutionFileText)
        {
            solutionFileText = solutionFileText.Replace(RegexStore.EndProjectGlobal,
                RegexStore.SolutionJExtProjectReference + RegexStore.EndProjectGlobal);
            return solutionFileText;
        }

        private static string AddDebugAndReleaseInformation(string solutionFileText)
        {
            solutionFileText = RegexStore.ReplaceLastOccurrence(solutionFileText, RegexStore.ReleaseAnyCpu,
                RegexStore.JExtReleaseDebugInformation);
            return solutionFileText;
        }

        private static string AddToCommonFolder(string solutionFileText, string solutionFile)
        {
            string jExtProjEqualsCommonFolder = "{D3DC56B0-8B95-47A5-A086-9E7A95552364} = {" + RegexStore.Get(RegexStore.CommonFolderPattern, solutionFile) + "}";
            solutionFileText = solutionFileText.Replace(RegexStore.NestedProjects,RegexStore.NestedProjects+ jExtProjEqualsCommonFolder);
            return solutionFileText;
        }
    }
}
