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
            return Text.Contains(projectGuid.ToUpper());
        }

        public bool ContainsProjectReference(CsProj referencedProject)
        {
            return Text.Contains(referencedProject.Guid.ToUpper());
        }

        public void RemoveProjectReference(string projectGuid)
        {
            var pattern = $"(?:Project.+{projectGuid}.*(\\n*\\r*))(?:EndProject(\\n*\\r*))";
            //var pattern = "Project.+CA12242D-1F50-44BB-9972-D6C6609E4C37.*(\\n*\\r*)EndProject(\\n*\\r*)";
            Regex regex = new Regex(pattern);
            var replacementText = regex.Replace(Text, "");
            WriteText(replacementText);

            pattern = "\\s*\\{CA12242D-1F50-44BB-9972-D6C6609E4C37.*";
            regex = new Regex(pattern);
            replacementText = regex.Replace(Text, "");
            WriteText(replacementText);
        }

        public void AddProjectReference(CsProj projectToAdd, string solutionFolder = null)
        {
            var replacementText = Text;

            replacementText = AddProjectText(replacementText, projectToAdd);
            replacementText = AddDebugAndReleaseInformation(replacementText, projectToAdd);
            if(solutionFolder != null)
                replacementText = AddToFolder(replacementText, projectToAdd, solutionFolder);

            WriteText(replacementText);
        }

        private string AddProjectText(string textToReplace, CsProj projectToAdd)
        {
            Uri mercurySourcePath = new Uri(Dumbledore.MercurySourceDir);
            Uri referencedProjectPath = new Uri(projectToAdd.Path);
            Uri relPath = mercurySourcePath.MakeRelativeUri(referencedProjectPath);

            textToReplace = RegexStore.ReplaceLastOccurrence(textToReplace,
                RegexStore.EndProject,
                RegexStore.EndProject + "\n" + "Project(\"{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}\") = " +
                $"\"{projectToAdd.Name}\"," +
                $" \"{relPath.ToString().Replace("src", "").Replace("/", "\\").TrimStart('\\')}\", " +
                $"\"{{{projectToAdd.Guid.ToUpper()}}}\"\r\n" +
                "EndProject");
            return textToReplace;
        }

        private string AddDebugAndReleaseInformation(string textToReplace, CsProj projectToAdd)
        {
            textToReplace = RegexStore.ReplaceLastOccurrence(textToReplace,
                RegexStore.ReleaseAnyCpu,
                RegexStore.ReleaseAnyCpu +
                Environment.NewLine + $"\t\t{{{projectToAdd.Guid.ToUpper()}}}.Debug|Any CPU.ActiveCfg = Debug|Any CPU" +
                Environment.NewLine + $"\t\t{{{projectToAdd.Guid.ToUpper()}}}.Debug|Any CPU.Build.0 = Debug|Any CPU" +
                Environment.NewLine + $"\t\t{{{projectToAdd.Guid.ToUpper()}}}.Release|Any CPU.ActiveCfg = Release|Any CPU" +
                Environment.NewLine + $"\t\t{{{projectToAdd.Guid.ToUpper()}}}.Release|Any CPU.Build.0 = Release|Any CPU");
            return textToReplace;
        }

        private string AddToFolder(string textToReplace, CsProj projectToAdd, string solutionFolder)
        {
            var solutionFolderGuid =
                RegexStore.Get($"Project.* = \\\"{solutionFolder}\\\", \\\"{solutionFolder}\\\", \\\"" +
                               "\\{(?<capturegroup>(.*))\\}\\\"", Text).ToArray();
            if (solutionFolderGuid.Length == 0)
                return textToReplace;

            string projectToAddEqualsFolderToAdd = $"{{{projectToAdd.Guid.ToUpper()}}} = " +
                                                   "{" + solutionFolderGuid.First() + "}";

            Regex reg = new Regex(RegexStore.EndGlobalSection);
            textToReplace = reg.Replace(textToReplace,
                "}" + Environment.NewLine + "\t\t" + projectToAddEqualsFolderToAdd + Environment.NewLine + "\tEndGlobalSection", 1);
            return textToReplace;
        }                
    }
}