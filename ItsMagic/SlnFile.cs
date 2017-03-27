using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ItsMagic
{
    public class SlnFile : MagicFile
    {
        public CsProj[] _csProjsCache { get; private set; }

        public SlnFile(string path)
        {
            Path = path;
        }

        public CsProj[] CsProjs()
        {
            if (_csProjsCache == null)
            {
                var dir = System.IO.Path.GetDirectoryName(Path);
                _csProjsCache = RegexStore.Get(RegexStore.CsProjFromSlnPattern, Text())
                    .Select(csProjRelPath => System.IO.Path.Combine(dir, csProjRelPath))
                    .Where(File.Exists)
                    .Select(file => new CsProj(file))
                    .ToArray();
            }
            return _csProjsCache;
        }

        public void RemoveProjectReference(string projectGuid)
        {
            var pattern = $"(?:Project.+{projectGuid}.*\n)(?:EndProject\n)";
            Regex regex = new Regex(pattern);
            var replacementText = regex.Replace(Text(), "");
            WriteText(replacementText);
        }

        public bool ContainsProjectReference(string projectGuid)
        {
            return Text().Contains(projectGuid);
        }

        public bool ContainsProjectReference(CsProj referencedProject)
        {
            string pattern = $"Project.*\\\"{referencedProject.Name.Replace(".", "\\.")}\\\"";
            Regex regex = new Regex(pattern);
            Match match = regex.Match(Text());
            return match.Success;
        }

        public void AddProjectReference(CsProj projectToAdd, string solutionFolder)
        {
            var replacementText = Text();

            replacementText = AddProjectText(replacementText, projectToAdd);
            replacementText = AddDebugAndReleaseInformation(replacementText, projectToAdd);
            replacementText = AddToFolder(replacementText, projectToAdd, solutionFolder);

            WriteText(replacementText);
        }

        private string AddProjectText(string textToReplace, CsProj projectToAdd)
        {
            //fix this later
            string dir = "C:\\source\\Mercury\\src";
            Uri mercurySourcePath = new Uri(dir);
            Uri referencedProjectPath = new Uri(projectToAdd.Path);
            Uri relPath = mercurySourcePath.MakeRelativeUri(referencedProjectPath);


            textToReplace = RegexStore.ReplaceLastOccurrence(textToReplace,
                RegexStore.EndProject,
                RegexStore.EndProject + "\n" + "Project(\"{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}\") = " +
                $"\"{projectToAdd.Name}\"," +
                $" \"{relPath.ToString().Replace("src", "").Replace("/", "\\")}\", " +
                $"\"{projectToAdd.Guid}\"\r\n" +
                "EndProject");
            return textToReplace;
        }

        private string AddDebugAndReleaseInformation(string textToReplace, CsProj projectToAdd)
        {
            textToReplace = RegexStore.ReplaceLastOccurrence(textToReplace,
                RegexStore.ReleaseAnyCpu,
                RegexStore.ReleaseAnyCpu +
                Environment.NewLine + $"\t\t{{{projectToAdd.Guid}}}.Debug|Any CPU.ActiveCfg = Debug|Any CPU" +
                Environment.NewLine + $"\t\t{{{projectToAdd.Guid}}}.Debug|Any CPU.Build.0 = Debug|Any CPU" +
                Environment.NewLine + $"\t\t{{{projectToAdd.Guid}}}.Release|Any CPU.ActiveCfg = Release|Any CPU" +
                Environment.NewLine + $"\t\t{{{projectToAdd.Guid}}}.Release|Any CPU.Build.0 = Release|Any CPU");
            return textToReplace;
        }

        private string AddToFolder(string textToReplace, CsProj projectToAdd, string solutionFolder)
        {
            string projectToAddEqualsFolderToAdd = $"{{{projectToAdd.Guid}}} = " +
                                                   "{" + RegexStore.Get($"Project.* = \\\"{solutionFolder}\\\", \\\"{solutionFolder}\\\", \\\"" +
                                                                        "\\{(?<capturegroup>(.*))\\}\\\"", Text()).First() + "}";
            textToReplace = textToReplace.Replace(RegexStore.NestedProjects, RegexStore.NestedProjects + "\n\t\t" + projectToAddEqualsFolderToAdd);
            return textToReplace;
        }
        //Functions to deprecate

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

            WriteText(solutionFileText);
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
            string jExtProjEqualsCommonFolder = "{D3DC56B0-8B95-47A5-A086-9E7A95552364} = {" + RegexStore.Get(RegexStore.CommonFolderPattern, Text()).First() + "}";
            solutionFileText = solutionFileText.Replace(RegexStore.NestedProjects,RegexStore.NestedProjects + "\n\t\t" + jExtProjEqualsCommonFolder);
            return solutionFileText;
        }

        public void AddNHibExtProjectReference()
        {
            var solutionFileText = File.ReadAllText(Path);

            solutionFileText = AddNHibExtProjectText(solutionFileText);
            solutionFileText = AddNHibExtDebugAndReleaseInformation(solutionFileText);
            solutionFileText = AddNHibExtToCommonFolder(solutionFileText);

            WriteText(solutionFileText);
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
            string nHibExtProjEqualsCommonFolder = "{F1575997-02D0-486F-AE36-69F6A3B37C39} = {" + RegexStore.Get(RegexStore.CommonFolderPattern, Text()).First() + "}";
            solutionFileText = solutionFileText.Replace(RegexStore.NestedProjects, RegexStore.NestedProjects + "\n\t\t" + nHibExtProjEqualsCommonFolder);
            return solutionFileText;
        }

        public void AddWeTcProjectReference()
        {
            var replacementText = Text();

            replacementText = AddWeTcProjectText(replacementText);
            replacementText = AddWeTcDebugAndReleaseInformation(replacementText);
            replacementText = AddWeTcToCommonFolder(replacementText);

            WriteText(replacementText);
        }

        public static string AddWeTcProjectText(string TextToReplace)
        {
            TextToReplace = RegexStore.ReplaceLastOccurrence(TextToReplace,
                RegexStore.EndProject,
                RegexStore.EndProject + "\n" + RegexStore.SolutionWeTcProjectReference);
            return TextToReplace;
        }

        public static string AddWeTcDebugAndReleaseInformation(string TextToReplace)
        {
            TextToReplace = RegexStore.ReplaceLastOccurrence(TextToReplace,
                RegexStore.ReleaseAnyCpu,
                RegexStore.ReleaseAnyCpu + "\n\t\t" + RegexStore.WeTcReleaseDebugInformation);
            return TextToReplace;
        }

        public string AddWeTcToCommonFolder(string TextToReplace)
        {
            string nHibExtProjEqualsCommonFolder = "{499EBA0D-DA7E-431B-AF62-74C492FD6E2A} = {" + RegexStore.Get(RegexStore.CommonFolderPattern, Text()).First() + "}";
            TextToReplace = TextToReplace.Replace(RegexStore.NestedProjects, RegexStore.NestedProjects + "\n\t\t" + nHibExtProjEqualsCommonFolder);
            return TextToReplace;
        }

        public void AddMercuryTestingProjectReference()
        {
            var replacementText = Text();

            replacementText = AddMercTestingProjectText(replacementText);
            replacementText = AddMercTestingDebugAndReleaseInformation(replacementText);
            replacementText = AddMercTestingToCommonFolder(replacementText);

            WriteText(replacementText);
        }
        

        public static string AddMercTestingProjectText(string textToReplace)
        {
            textToReplace = RegexStore.ReplaceLastOccurrence(textToReplace,
                RegexStore.EndProject,
                RegexStore.EndProject + "\n" + RegexStore.SolutionMercTestingProjectReference);
            return textToReplace;
        }

        public static string AddMercTestingDebugAndReleaseInformation(string textToReplace)
        {
            textToReplace = RegexStore.ReplaceLastOccurrence(textToReplace,
                RegexStore.ReleaseAnyCpu,
                RegexStore.ReleaseAnyCpu + "\n\t\t" + RegexStore.MercTestingReleaseDebugInformation);
            return textToReplace;
        }

        public string AddMercTestingToCommonFolder(string textToReplace)
        {
            string mercTestingProjEqualsCommonFolder = "{5C9BAB9F-1136-4760-9771-A8E543574BE4} = {" + RegexStore.Get(RegexStore.TestsFolderPattern, Text()).First() + "}";
            textToReplace = textToReplace.Replace(RegexStore.NestedProjects, RegexStore.NestedProjects + "\n\t\t" + mercTestingProjEqualsCommonFolder);
            return textToReplace;
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
            WriteText(slnFiletext);
        }
        
    }
}