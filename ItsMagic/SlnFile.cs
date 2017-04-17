using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Dumbledore
{
    public class SlnFile : MagicFile
    {
        private static readonly Dictionary<string, SlnFile> SlnFilePool = new Dictionary<string, SlnFile>();

        private CsProj[] _csProjsCache { get; set; }

        private SlnFile(string path) : base(path)
        {
        }

        public static SlnFile Get(string path)
        {
            SlnFile result;
            if (!SlnFilePool.TryGetValue(path, out result))
            {
                result = new SlnFile(path);
                SlnFilePool.Add(path, result);
            }
            return result;
        }

        public CsProj[] CsProjs => _csProjsCache ?? (_csProjsCache = GetCsProjs());
        
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

                RemoveProjectText(projectGuid);
                RemoveRemainingReferences(projectGuid);

                WriteFile();
                _csProjsCache = null;
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
            _csProjsCache = null;
        }

        public void RepairWhiteSpace()
        {
            Text = new Librarian("(\\t*\\r\\n){2,}").Replace(Text, Environment.NewLine);
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

        private CsProj[] GetCsProjs()
        {
            if (_csProjsCache == null)
            {
                var dir = Path.GetDirectoryName(FilePath);
                _csProjsCache = new Librarian(_csProjPattern, Text)
                    .Get("capturegroup")
                    .Select(csProjRelPath => Path.Combine(dir, csProjRelPath))
                    .Where(File.Exists)
                    .Select(file => CsProj.Get(file))
                    .ToArray();
            }
            return _csProjsCache;
        }

        private void RemoveProjectText(string projectGuid)
        {
            var pattern = $"(?:Project.+{projectGuid}.+\\n)(?:EndProject.+\\n)";
            Text = new Librarian(pattern, Text, RegexOptions.IgnoreCase)
                .Replace(Text, "");
        }

        private void RemoveRemainingReferences(string projectGuid)
        {
            var pattern = $".*{{{projectGuid}}}.*\\n";
            Text = new Librarian(pattern, Text, RegexOptions.IgnoreCase)
                .Replace(Text, "");
        }

        private void AddProjectText(CsProj projectToAdd)
        {
            Uri mercurySourcePath = new Uri(Wand.MercurySourceDir);
            Uri referencedProjectPath = new Uri(projectToAdd.FilePath);
            Uri relPath = mercurySourcePath.MakeRelativeUri(referencedProjectPath);

            Text = Text.ReplaceLastOccurrence(_endProject,
                _endProject + "\n" + "Project(\"{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}\") = " +
                $"\"{projectToAdd.Name}\"," +
                $" \"{relPath.ToString().Replace("src", "").Replace("/", "\\").TrimStart('\\')}\", " +
                $"\"{{{projectToAdd.Guid.ToUpper()}}}\"\r\n" +
                "EndProject");
        }

        private void AddDebugAndReleaseInformation(CsProj projectToAdd)
        {
            Text = Text.ReplaceLastOccurrence(_releaseAnyCpu,
                _releaseAnyCpu +
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
                new Librarian($"Project.* = \\\"{solutionFolder}\\\", \\\"{solutionFolder}\\\", \\\"" +
                               "\\{(?<capturegroup>(.*))\\}\\\"", Text)
                               .Get("capturegroup")
                               .ToArray();

            string projectToAddEqualsFolderToAdd = $"{{{projectToAdd.Guid.ToUpper()}}} = {{{solutionFolderGuid.Single()}}}";

            var reg = new Regex(_endGlobalSection);
            Text = reg.Replace(Text,
                "}" + Environment.NewLine + "\t\t" + projectToAddEqualsFolderToAdd + Environment.NewLine + "\tEndGlobalSection", 1);
        }

        private bool SolutionFolderExists(string solutionFolder)
        {
            return new Librarian(_solutionFolderNamePattern, Text)
                .Get("capturegroup")
                .Contains(solutionFolder);
        }

        private void CreateSolutionFolder(string solutionFolder)
        {
            Text = Text.ReplaceLastOccurrence(_endProject,
                _endProject + Environment.NewLine + 
                "Project(\"{2150E333-8FDC-42A3-9474-1A3956D46DE8}\") = " +
                $"\"{solutionFolder}\", " +
                $"\"{solutionFolder}\", " +
                $"\"{{{Guid.NewGuid().ToString().ToUpper()}}}\"" +
                Environment.NewLine +
                _endProject);
            WriteFile();
        }

        private const string _endProject = "EndProject";
        private const string _releaseAnyCpu = "Release|Any CPU";
        private const string _csProjPattern = "Project(.*) = .*, \\\"(?<capturegroup>.*\\.csproj)\\\", \\\".*\\\"";
        private const string _endGlobalSection = "\\}\\s+EndGlobalSection";
        private const string _solutionFolderNamePattern = "Project\\(\\\"\\{2150E333-8FDC-42A3-9474-1A3956D46DE8\\}\\\"\\) = \\\"(?<capturegroup>(\\w+))\\\"";
    }
}