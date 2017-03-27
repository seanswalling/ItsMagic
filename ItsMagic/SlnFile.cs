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
                _csProjsCache = RegexStore.Get(RegexStore.CsProjFromSlnPattern, Text)
                    .Select(csProjRelPath => System.IO.Path.Combine(dir, csProjRelPath))
                    .Where(File.Exists)
                    .Select(file => new CsProj(file))
                    .ToArray();
            }
            return _csProjsCache;
        }

        public bool ContainsProjectReference(string projectGuid)
        {
            return Text.Contains(projectGuid);
        }

        public bool ContainsProjectReference(CsProj referencedProject)
        {
            return Text.Contains(referencedProject.Guid);
        }

        public void RemoveProjectReference(string projectGuid)
        {
            var pattern = $"(?:Project.+{projectGuid}.*\n)(?:EndProject\n)";
            Regex regex = new Regex(pattern);
            var replacementText = regex.Replace(Text, "");
            WriteText(replacementText);
        }

        public void AddProjectReference(CsProj projectToAdd, string solutionFolder)
        {
            var replacementText = Text;

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
                                                                        "\\{(?<capturegroup>(.*))\\}\\\"", Text).First() + "}";
            textToReplace = textToReplace.Replace(RegexStore.NestedProjects, RegexStore.NestedProjects + "\n\t\t" + projectToAddEqualsFolderToAdd);
            return textToReplace;
        }
                
    }
}