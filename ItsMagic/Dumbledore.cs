using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace ItsMagic
{
    public static class Dumbledore
    {
        public const string MercurySourceDir = "";

        public static void UpdateAllReferencesToNewgetReferences(string projectDirectory)
        {
            SlnFile[] solutionFiles = Directory.EnumerateFiles(projectDirectory, "*.sln", SearchOption.AllDirectories)
                .Select(file => new SlnFile(file))
                .ToArray();
            //{ @"C:\source\Mercury\src\Mercury.ReflectiveTests.sln" };
            foreach (var solutionFile in solutionFiles)
            {
                foreach (var csProj in solutionFile.CsProjs())
                {
                    foreach (var csFile in csProj.CsFiles())
                    {

                    }
                }
            }
        }

        public static void AddProjectReferences(string projectDirectory, CsProj projectToAdd)
        {
            foreach (var solutionFile in GetSolutionFiles(projectDirectory).ToArray())
            {
                foreach (var csProj in solutionFile.CsProjs().ToArray())
                {
                    foreach (var csFile in csProj.CsFiles())
                    {
                        if (csFile.HasEvidenceOf(projectToAdd))
                        {
                            var str = $"Adding References too {csFile.Name}, {csProj.Name} and {solutionFile.Name}";
                            File.AppendAllLines(@"C:\Users\jordan.warren\Desktop\Log.txt", new List<string> {str});
                            AddReferences(csFile, csProj, solutionFile, projectToAdd, projectDirectory);
                        }
                    }
                }
            }
        }

        private static void AddReferences(CsFile csFile, CsProj csProj, SlnFile slnFile, CsProj projectToAdd, string projectDirectory)
        {
            csFile.AddUsing(projectToAdd.Name);
            if (!csProj.ContainsProjectReference(projectToAdd))
            {
                csProj.AddProjectReference(projectToAdd, projectDirectory);
            }
            if (!slnFile.ContainsProjectReference(projectToAdd))
            {
                slnFile.AddProjectReference(projectToAdd, "Common");
            }
        }

        public static void AddTestCoreReplacementsProjectReferences(string projectDirectory, CsProj[] testCoreReplacements)
        {
            foreach (var solutionFile in GetSolutionFiles(projectDirectory).ToArray())
            {
                foreach (var csProj in solutionFile.CsProjs().ToArray())
                {
                    foreach (var csFile in csProj.CsFiles())
                    {
                        if (csFile.Usings.Contains("Mercury.Tests.Core") || csFile.Usings.Contains("Mercury.Tests.Shared"))
                        {
                            var str = $"Updating Using statements for {csFile.Name}";
                            File.AppendAllLines(@"C:\Users\jordan.warren\Desktop\Log.txt", new List<string> { str });
                            csFile.RemoveUsing("Mercury.Tests.Core");
                            csFile.RemoveUsing("Mercury.Tests.Shared");
                            csFile.AddUsing("Mercury.Testing");
                            csFile.AddUsing("Mercury.Testing.Factories");
                            csFile.AddUsing("Mercury.Testing.Integrated");
                            csFile.AlphabatiseUsings();
                        }
                    }
                    foreach (var testCoreReplacement in testCoreReplacements)
                    {
                        if (!csProj.ContainsProjectReference(testCoreReplacement))
                        {
                            var str = $"Adding Project {testCoreReplacement.Name} Reference to {csProj.Name}";
                            File.AppendAllLines(@"C:\Users\jordan.warren\Desktop\Log.txt", new List<string> { str });
                            csProj.AddProjectReference(testCoreReplacement, projectDirectory);
                        }
                        if (!solutionFile.ContainsProjectReference(testCoreReplacement))
                        {
                            var str = $"Adding Project {testCoreReplacement.Name} Reference to {solutionFile.Name}";
                            File.AppendAllLines(@"C:\Users\jordan.warren\Desktop\Log.txt", new List<string> { str });
                            solutionFile.AddProjectReference(testCoreReplacement, "Tests");
                        }
                    }
                    if (csProj.ContainsProjectReference(RegexStore.TestsSharedGuid) ||
                        csProj.ContainsProjectReference(RegexStore.TestsCoreGuid))
                    {
                        var str = $"Removing references from {csProj.Name}";
                        File.AppendAllLines(@"C:\Users\jordan.warren\Desktop\Log.txt", new List<string> { str });
                        csProj.RemoveProjectReference(RegexStore.TestsSharedGuid);
                        csProj.RemoveProjectReference(RegexStore.TestsCoreGuid);
                    }
                    if (solutionFile.ContainsProjectReference(RegexStore.TestsSharedGuid) ||
                        solutionFile.ContainsProjectReference(RegexStore.TestsCoreGuid))
                    {
                        var str = $"Removing references from {solutionFile.Name}";
                        File.AppendAllLines(@"C:\Users\jordan.warren\Desktop\Log.txt", new List<string> { str });
                        solutionFile.RemoveProjectReference(RegexStore.TestsSharedGuid);
                        solutionFile.RemoveProjectReference(RegexStore.TestsCoreGuid);
                    }
                }
            }
        }

        public static IEnumerable<string> ReadLines(string file)
        {
            if (File.Exists(file))
            {
                using (var reader = new StreamReader(file))
                {
                    while (!reader.EndOfStream)
                    {
                        yield return reader.ReadLine();
                    }
                }
            }
        }

        public static IEnumerable<SlnFile> GetSolutionFiles(string dir)
        {
            return Directory.EnumerateFiles(dir, "*.sln", SearchOption.AllDirectories)
                .Where(File.Exists)
                .Select(file => new SlnFile(file));
        }

        public static IEnumerable<CsProj> GetCsProjFiles(string dir)
        {
            return Directory.EnumerateFiles(dir, "*.csproj", SearchOption.AllDirectories)
                .Select(file => new CsProj(file));
        }

        public static IEnumerable<CsFile> GetCsFiles(string dir)
        {
            return Directory.EnumerateFiles(dir, "*.cs", SearchOption.AllDirectories)
                .Select(file => new CsFile(file));
        }




        #region Abstract Later

        public static void UpdateProjectReference(CsProj toUpdate, ProjectReference referenceToReplace, string replacement)
        {
            var Regex = new Regex(referenceToReplace.Pattern);
            var csProjText = File.ReadAllText(toUpdate.Path);
            csProjText = Regex.Replace(csProjText, replacement);
            toUpdate.WriteText(csProjText);
        }

        public static void UpdateNugetPackageReference(CsProj toUpdate, NugetPackageReference referenceToReplace, string replacement)
        {
            var Regex = new Regex(referenceToReplace.Pattern);
            var csProjText = File.ReadAllText(toUpdate.Path);
            csProjText = Regex.Replace(csProjText, replacement);
            toUpdate.WriteText(csProjText);
        }

        #endregion

        //Deprecated Functions

        public static void RemoveDuplicateXmlHeader(string dir)
        {
            var csProjs = Directory.EnumerateFiles(dir, "*.csproj", SearchOption.AllDirectories)
                .Select(file => new CsProj(file));
            foreach (var csProj in csProjs)
            {
                Console.WriteLine("Checking: " + csProj);
                var csprojText = File.ReadAllText(csProj.Path);
                Regex reg =
                    new Regex(
                        "(\\s+)*<\\?xml version=\\\"1\\.0\\\" encoding=\\\"utf-8\\\"\\?>(\\s+)<\\?xml version=\\\"1\\.0\\\" encoding=\\\"utf-8\\\"\\?>");
                csprojText = reg.Replace(csprojText, "<?xml version=\"1.0\" encoding=\"utf-8\"?>", 1);
                csProj.WriteText(csprojText);
                CsProj.ReformatXml(csProj.Path);
            }
        }

        public static void RemoveLogForNetReference(string[] filesToFix)
        {
            Regex reg =
                new Regex("(\\s)*<Reference Include=\\\"log4net(.*)\\\">(\\s)*(.)*(\\s)*(.)*(\\s)*<\\/Reference>");
            foreach (var file in filesToFix)
            {
                var csProjText = File.ReadAllText(file);
                csProjText = reg.Replace(csProjText, "");
                File.WriteAllText(file, csProjText);
            }
        }

        public static void AddNewRelicRefsTo(string[] filesThatRequireNewRelic)
        {
            foreach (var file in filesThatRequireNewRelic)
            {
                var csproj = new CsProj(file);
                csproj.AddNewRelicProjectReference();
            }
        }

        //public static void UpdateProjectReferenceWithNugetReference(CsProj toUpdate, ProjectReference reference,
        //    NugetPackageReference referenceToAdd)
        //{
        //    RemoveReference(toUpdate, reference);
        //    AddNugetReference(toUpdate, reference);
        //    ReformatXml(toUpdate.Path);
        //}

        //public static void RemoveReference(CsProj csProj, ProjectReference reference)
        //{
        //    var Regex = new Regex(reference.ProjectRefPattern);
        //    var csProjText = File.ReadAllText(csProj.Path);
        //    csProjText = Regex.Replace(csProjText, "", 1);
        //    File.WriteAllText(csProj.Path, csProjText);
        //}

        //public static void AddNugetReference(CsProj csProj, ProjectReference reference)
        //{
        //    var Regex = new Regex(RegexStore.ItemGroupTag);
        //    var csProjText = File.ReadAllText(csProj.Path);
        //    csProjText = Regex.Replace(csProjText, RegexStore.ItemGroupTag + reference.NugetRef, 1);
        //    File.WriteAllText(csProj.Path, csProjText);
        //    UpdatePackagesConfig(Path.GetDirectoryName(csProj.Path) + "\\packages.config", reference);
        //}

        //private static void UpdatePackagesConfig(string packages, ProjectReference reference)
        //{
        //    var Regex = new Regex(RegexStore.PackagesTag);
        //    if (!File.Exists(packages))
        //    {
        //        File.WriteAllText(packages, "<?xml version=\"1.0\" encoding=\"utf-8\"?><packages></packages>");
        //    }
        //    var packagesText = File.ReadAllText(packages);
        //    packagesText = Regex.Replace(packagesText, RegexStore.PackagesTag + reference.PackagesRef, 1);
        //    File.WriteAllText(packages, packagesText);
        //    ReformatXml(packages);
        //}

        //public static IEnumerable<string> GetFiles(string projectDirectory, string extension)
        //{
        //    return Directory.EnumerateFiles(projectDirectory, "*."+extension, SearchOption.AllDirectories);
        //}

    }
}
