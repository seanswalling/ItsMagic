using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Dumbledore
{
    public class SlnFile : MagicFile
    {
        private CsProj[] CsProjsCache { get; set; }

        public SlnFile(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException();
            FilePath = path;
        }

        public CsProj[] CsProjs()
        {
            if (CsProjsCache == null)
            {
                var dir = Path.GetDirectoryName(FilePath);
                CsProjsCache = RegexStore.Get(RegexStore.CsProjFromSlnPattern, Text)
                    .Select(csProjRelPath => Path.Combine(dir, csProjRelPath))
                    .Where(File.Exists)
                    .Select(file => CsProj.GetCsProj(file))
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
            if (ContainsProjectReference(projectGuid))
            {
                Cauldron.Add($"Removing project reference with guid {projectGuid} from {Name}");
                var pattern = $"(?:Project.+{projectGuid}.*(\\n*\\r*))(?:EndProject(\\n*\\r*))";
                var regex = new Regex(pattern);
                Text = regex.Replace(Text, "");

                pattern = $".*{{{projectGuid}}}.*";
                regex = new Regex(pattern);
                Text = regex.Replace(Text, "");
                WriteFile();
                RemoveWhiteSpace();
            }
            else
            {
                Cauldron.Add($"No project of GUID: {projectGuid} found");
            }
        }

        public void AddProjectReference(CsProj projectToAdd, string solutionFolder = null)
        {
            AddProjectText(projectToAdd);
            AddDebugAndReleaseInformation(projectToAdd);
            if(solutionFolder != null)
                AddToFolder(projectToAdd, solutionFolder);

            WriteFile();
        }

        private void AddProjectText(CsProj projectToAdd)
        {
            Uri mercurySourcePath = new Uri(Wand.MercurySourceDir);
            Uri referencedProjectPath = new Uri(projectToAdd.FilePath);
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
            if (!SolutionFolderExists(solutionFolder))
            {
                CreateSolutionFolder(solutionFolder);
            }

            var solutionFolderGuid =
                RegexStore.Get($"Project.* = \\\"{solutionFolder}\\\", \\\"{solutionFolder}\\\", \\\"" +
                               "\\{(?<capturegroup>(.*))\\}\\\"", Text).ToArray();

            string projectToAddEqualsFolderToAdd = $"{{{projectToAdd.Guid.ToUpper()}}} = {{{solutionFolderGuid.Single()}}}";

            var reg = new Regex(RegexStore.EndGlobalSection);
            Text = reg.Replace(Text,
                "}" + Environment.NewLine + "\t\t" + projectToAddEqualsFolderToAdd + Environment.NewLine + "\tEndGlobalSection", 1);
        }

        private bool SolutionFolderExists(string solutionFolder)
        {
            return RegexStore.Get(RegexStore.SolutionFolderNamePattern, Text).Contains(solutionFolder);
        }

        private void CreateSolutionFolder(string solutionFolder)
        {
            Text = RegexStore.ReplaceLastOccurrence(Text,
                RegexStore.EndProject,
                RegexStore.EndProject + Environment.NewLine + 
                "Project(\"{2150E333-8FDC-42A3-9474-1A3956D46DE8}\") = " +
                $"\"{solutionFolder}\", " +
                $"\"{solutionFolder}\", " +
                $"\"{{{Guid.NewGuid().ToString().ToUpper()}}}\"" +
                Environment.NewLine +
                RegexStore.EndProject);
            WriteFile();
        }

        public void RemoveWhiteSpace()
        {
            Regex reg = new Regex("(\\r\\n)+");
            Text = reg.Replace(Text, Environment.NewLine);
            WriteFile();
        }

        public static IEnumerable<SlnFile> SolutionsThatReference(CsProj csProj)
        {
            var slns = Wand.GetSolutionFiles(Wand.MercurySourceDir);
            foreach (var slnFile in slns)
            {
                if (slnFile.ContainsProjectReference(csProj))
                {
                    yield return slnFile;
                }
            }
        }
    }
}