using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Build.Construction;

namespace ItsMagic
{
    public class SlnFile
    {
        public string Path { get; private set; }
        public CsProj[] CsProjs { get; private set; }
        private string TextCache { get; set; }

        public SlnFile(string path)
        {
            Path = path;
            CsProjs = GetCsProjs();
        }

        private CsProj[] GetCsProjs()
        {
            Console.WriteLine("Get Csproj Files for: " + Path);
            var dir = System.IO.Path.GetDirectoryName(Path);
            return RegexStore.Get(RegexStore.CsProjFromSlnPattern, Path)
                .Select(csProjRelPath => System.IO.Path.Combine(dir, csProjRelPath))
                .Select(file => new CsProj(file))
                .ToArray();
        }

        public string Text()
        {
            if (TextCache != null)
                return TextCache;
            var text = File.ReadAllText(Path);
            return text;
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

        public bool ContainsWeTcProjectReference()
        {
            var solutionFileText = File.ReadAllText(Path);
            Regex regex = new Regex(RegexStore.SolutionWeTcProjectReferencePattern);
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

        public void AddWeTcProjectReference()
        {
            var solutionFileText = File.ReadAllText(Path);

            solutionFileText = AddWeTcProjectText(solutionFileText);
            solutionFileText = AddWeTcDebugAndReleaseInformation(solutionFileText);
            solutionFileText = AddWeTcToCommonFolder(solutionFileText);

            File.WriteAllText(Path, solutionFileText);
        }

        public static string AddWeTcProjectText(string solutionFileText)
        {
            solutionFileText = RegexStore.ReplaceLastOccurrence(solutionFileText,
                RegexStore.EndProject,
                RegexStore.EndProject + "\n" + RegexStore.SolutionWeTcProjectReference);
            return solutionFileText;
        }

        public static string AddWeTcDebugAndReleaseInformation(string solutionFileText)
        {
            solutionFileText = RegexStore.ReplaceLastOccurrence(solutionFileText,
                RegexStore.ReleaseAnyCpu,
                RegexStore.ReleaseAnyCpu + "\n\t\t" + RegexStore.WeTcReleaseDebugInformation);
            return solutionFileText;
        }

        public string AddWeTcToCommonFolder(string solutionFileText)
        {
            string nHibExtProjEqualsCommonFolder = "{499EBA0D-DA7E-431B-AF62-74C492FD6E2A} = {" + RegexStore.Get(RegexStore.CommonFolderPattern, Path).First() + "}";
            solutionFileText = solutionFileText.Replace(RegexStore.NestedProjects, RegexStore.NestedProjects + "\n\t\t" + nHibExtProjEqualsCommonFolder);
            return solutionFileText;
        }

        public bool HasLogRepoReference()
        {
            return RegexStore.Get(RegexStore.LogRepoReferencePattern, Path).Any();
        }

        public string[] LogRepoReferences()
        {
            return RegexStore.Get(RegexStore.LogRepoReferencePattern, Path).ToArray();
        }

        public void UpdateLogRepoReference(string reference)
        {
            var slnFiletext = File.ReadAllText(Path);
            slnFiletext = slnFiletext.Replace(reference, "Platform\\" + reference);
            File.WriteAllText(Path, slnFiletext);
        }

        //Functions to deprecate


    }
}