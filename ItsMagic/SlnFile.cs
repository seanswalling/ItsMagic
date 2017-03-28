using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ItsMagic
{
    public class SlnFile : MagicFile
    {
        private CsProj[] CsProjsCache { get; set; }

        public SlnFile(string path)
        {
            Filepath = path;
        }

        public CsProj[] CsProjs()
        {
            if (CsProjsCache == null)
            {
                var dir = Path.GetDirectoryName(Filepath);
                CsProjsCache = RegexStore.Get(RegexStore.CsProjFromSlnPattern, Text)
                    .Select(csProjRelPath => Path.Combine(dir, csProjRelPath))
                    .Where(File.Exists)
                    .Select(file => new CsProj(file))
                    .ToArray();
            }
            return CsProjsCache;
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
            var regex = new Regex(pattern);
            Text = regex.Replace(Text, "");

            pattern = $".*{{{projectGuid}}}.*";
            regex = new Regex(pattern);
            Text = regex.Replace(Text, "");
            WriteFile();
        }

        public void AddProjectReference(CsProj projectToAdd, string solutionFolder = null)
        {
            var replacementText = Text;

            AddProjectText(projectToAdd);
            AddDebugAndReleaseInformation(projectToAdd);
            if (solutionFolder != null)
                AddToFolder(projectToAdd, solutionFolder);

            WriteFile();
        }

        private void AddProjectText(CsProj projectToAdd)
        {
            Uri mercurySourcePath = new Uri(Dumbledore.MercurySourceDir);
            Uri referencedProjectPath = new Uri(projectToAdd.Filepath);
            Uri relPath = mercurySourcePath.MakeRelativeUri(referencedProjectPath);

            Text = RegexStore.ReplaceLastOccurrence(Text,
                RegexStore.EndProject,
                RegexStore.EndProject + "\n" + "Project(\"{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}\") = " +
                $"\"{projectToAdd.Name}\"," +
                $" \"{relPath.ToString().Replace("src", "").Replace("/", "\\").TrimStart('\\')}\", " +
                $"\"{{{projectToAdd.Guid.ToUpper()}}}\"\r\n" +
                "EndProject");
        }

        private void AddDebugAndReleaseInformation(CsProj projectToAdd)
        {
            Text = RegexStore.ReplaceLastOccurrence(Text,
                RegexStore.ReleaseAnyCpu,
                RegexStore.ReleaseAnyCpu +
                Environment.NewLine + $"\t\t{{{projectToAdd.Guid.ToUpper()}}}.Debug|Any CPU.ActiveCfg = Debug|Any CPU" +
                Environment.NewLine + $"\t\t{{{projectToAdd.Guid.ToUpper()}}}.Debug|Any CPU.Build.0 = Debug|Any CPU" +
                Environment.NewLine + $"\t\t{{{projectToAdd.Guid.ToUpper()}}}.Release|Any CPU.ActiveCfg = Release|Any CPU" +
                Environment.NewLine + $"\t\t{{{projectToAdd.Guid.ToUpper()}}}.Release|Any CPU.Build.0 = Release|Any CPU");
        }

        private void AddToFolder(CsProj projectToAdd, string solutionFolder)
        {
            var solutionFolderGuid =
                RegexStore.Get($"Project.* = \\\"{solutionFolder}\\\", \\\"{solutionFolder}\\\", \\\"" +
                               "\\{(?<capturegroup>(.*))\\}\\\"", Text).ToArray();
            if (solutionFolderGuid.Length == 0)
                return;

            string projectToAddEqualsFolderToAdd = $"{{{projectToAdd.Guid.ToUpper()}}} = {{{solutionFolderGuid.Single()}}}";

            var reg = new Regex(RegexStore.EndGlobalSection);
            Text = reg.Replace(Text,
                "}" + Environment.NewLine + "\t\t" + projectToAddEqualsFolderToAdd + Environment.NewLine + "\tEndGlobalSection", 1);
        }                
    }
}