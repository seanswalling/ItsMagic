using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ItsMagic
{
    public static class Dumbledore
    {
        public static string MercurySourceDir
        {
            get
            {
                string dir1 = @"C:\source\Mercury\src";
                string dir2 = @"E:\github\cc\Mercury\src";
                if (Directory.Exists(dir1))
                    return dir1;
                if (Directory.Exists(dir2))
                    return dir1;
                throw new DirectoryNotFoundException();
            }
        }
        public static string MagicDir => new DirectoryInfo(Environment.CurrentDirectory).Parent.Parent.FullName;

        public static void UpdateAllReferencesToNugetReferences()
        {
            SlnFile[] solutionFiles = Directory.EnumerateFiles(MercurySourceDir, "*.sln", SearchOption.AllDirectories)
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

        public static void AddProjectReferences(CsProj projectToAdd)
        {
            foreach (var solutionFile in GetSolutionFiles(MercurySourceDir).ToArray())
            {
                foreach (var csProj in solutionFile.CsProjs().ToArray())
                {
                    foreach (var csFile in csProj.CsFiles())
                    {
                        if (csFile.HasEvidenceOf(projectToAdd))
                        {
                            Cauldron.Add($"Adding References too {csFile.Name}, {csProj.Name} and {solutionFile.Name}");
                            AddReferences(csFile, csProj, solutionFile, projectToAdd);
                        }
                    }
                }
            }
        }

        private static void AddReferences(CsFile csFile, CsProj csProj, SlnFile slnFile, CsProj projectToAdd)
        {
            csFile.AddUsing(projectToAdd.Name);
            if (!csProj.ContainsProjectReference(projectToAdd))
            {
                csProj.AddProjectReference(projectToAdd);
            }
            if (!slnFile.ContainsProjectReference(projectToAdd))
            {
                slnFile.AddProjectReference(projectToAdd, "Common");
            }
        }

        public static void AddTestCoreReplacementsProjectReferences(CsProj[] testCoreReplacements)
        {
            AddTestCoreReplacementsToSln(testCoreReplacements);
            AddTestCoreReplacementsToCsProj(testCoreReplacements);
            //AddTestCoreReplacementsToCsFile();
        }

        private static void AddTestCoreReplacementsToSln(CsProj[] testCoreReplacements)
        {
            var slnFiles = GetSolutionFiles(MercurySourceDir);
            foreach (var solutionFile in slnFiles)
            {
                if (solutionFile.ContainsProjectReference(RegexStore.TestsSharedGuid) ||
                solutionFile.ContainsProjectReference(RegexStore.TestsCoreGuid))
                {
                    Cauldron.Add($"Removing references from {solutionFile.Name}.sln");
                    solutionFile.RemoveProjectReference(RegexStore.TestsSharedGuid);
                    solutionFile.RemoveProjectReference(RegexStore.TestsCoreGuid);

                    foreach (var testCoreReplacement in testCoreReplacements)
                    {
                        Cauldron.Add($"Adding Project {testCoreReplacement.Name} Reference to {solutionFile.Name}.sln");
                        solutionFile.AddProjectReference(testCoreReplacement, "Tests");
                    }
                }
            }
        }

        private static void AddTestCoreReplacementsToCsProj(CsProj[] testCoreReplacements)
        {
            var csProjs = GetCsProjFiles(MercurySourceDir);
            foreach (var csProj in csProjs)
            {
                if (csProj.ContainsProjectReference(RegexStore.TestsSharedGuid) ||
                csProj.ContainsProjectReference(RegexStore.TestsCoreGuid))
                {
                    Cauldron.Add($"Removing references from {csProj.Name}.csproj");
                    csProj.RemoveProjectReference(RegexStore.TestsSharedGuid);
                    csProj.RemoveProjectReference(RegexStore.TestsCoreGuid);
                    foreach (var replacement in testCoreReplacements)
                    {
                        if (!csProj.ContainsProjectReference(replacement))
                        {
                            Cauldron.Add($"Adding Project {replacement.Name} Reference to {csProj.Name}.csproj");
                            csProj.AddProjectReference(replacement);
                        }
                    }
                }
            }
        }

        private static void AddTestCoreReplacementsToCsFile()
        {
            var csFiles = GetCsFiles(MercurySourceDir);
            foreach (var csFile in csFiles)
            {
                Cauldron.Add($"checking {csFile.Name}.cs for Using Statements");
                if (csFile.Usings.Contains("Mercury.Tests.Core") || csFile.Usings.Contains("Mercury.Tests.Shared"))
                {
                    Cauldron.Add($"Updating Using statements for {csFile.Name}.cs");
                    csFile.RemoveUsing("Mercury.Tests.Core");
                    csFile.RemoveUsing("Mercury.Tests.Shared");
                    csFile.AddUsing("Mercury.Testing");
                    csFile.AddUsing("Mercury.Testing.Factories");
                    csFile.AddUsing("Mercury.Testing.Integrated");
                    csFile.AlphabatiseUsings();
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
            var regex = new Regex(referenceToReplace.Pattern);
            var csProjText = File.ReadAllText(toUpdate.FilePath);
            toUpdate.Text = regex.Replace(csProjText, replacement);
            toUpdate.WriteFile();
        }

        public static void UpdateNugetPackageReference(CsProj toUpdate, NugetPackageReference referenceToReplace, string replacement)
        {
            var Regex = new Regex(referenceToReplace.Pattern);
            var csProjText = File.ReadAllText(toUpdate.FilePath);
            toUpdate.Text = Regex.Replace(csProjText, replacement);
            toUpdate.WriteFile();
        }

        #endregion

        //Deprecated Functions

        //public static void RemoveDuplicateXmlHeader()
        //{
        //    var csProjs = Directory.EnumerateFiles(MercurySourceDir, "*.csproj", SearchOption.AllDirectories)
        //        .Select(file => new CsProj(file));
        //    foreach (var csProj in csProjs)
        //    {
        //        Console.WriteLine("Checking: " + csProj);
        //        var csprojText = File.ReadAllText(csProj.FilePath);
        //        Regex reg =
        //            new Regex(
        //                "(\\s+)*<\\?xml version=\\\"1\\.0\\\" encoding=\\\"utf-8\\\"\\?>(\\s+)<\\?xml version=\\\"1\\.0\\\" encoding=\\\"utf-8\\\"\\?>");
        //        csprojText = reg.Replace(csprojText, "<?xml version=\"1.0\" encoding=\"utf-8\"?>", 1);
        //        csProj.WriteFile(csprojText);
        //        CsProj.ReformatXml(csProj.FilePath);
        //    }
        //}

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

        public static void UpdateReadModelConventionsTestReference()
        {
            var pluginProjs = GetCsProjFiles(MercurySourceDir + @"\Plugins");
            var pattern = "\\.\\.\\\\\\.\\.\\\\Platform\\\\Tests\\.Core\\\\ReadModelConventionsTest\\.cs\\\">(\\s+)<Link>ReadModelConventionsTest\\.cs<\\/Link>(\\s+)<\\/Compile>";
            var replacement = "ReadModelConventionsTest.cs\" />";

            foreach (var pluginProj in pluginProjs)
            {
                if (pluginProj.Text.Contains(@"Platform\Tests.Core\ReadModelConventionsTest.cs"))
                {
                    Regex reg = new Regex(pattern);
                    pluginProj.Text = reg.Replace(pluginProj.Text, replacement);
                    pluginProj.WriteFile();

                    var readModelConventionsTest = MagicDir + @"\ReadModelConventionsTest\ReadModelConventionsTest.cs";
                    var pluginProjDir = Path.GetDirectoryName(pluginProj.FilePath) + @"\ReadModelConventionsTest.cs";
                    File.Copy(readModelConventionsTest, pluginProjDir);
                }
            }
        }
    }
}
