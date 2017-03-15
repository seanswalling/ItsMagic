using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace ItsMagic
{
    public static class Dumbledore
    {
        public static void UpdateAllReferencesToNewgetReferences(string projectDirectory)
        {
            SlnFile[] solutionFiles = Directory.EnumerateFiles(projectDirectory, "*.sln", SearchOption.AllDirectories)
                .Select(file => new SlnFile(file))
                .ToArray();
            //{ @"C:\source\Mercury\src\Mercury.ReflectiveTests.sln" };
            foreach (var solutionFile in solutionFiles)
            {
                var csProjs = solutionFile.GetCsProjs().ToArray();
                foreach (var csProj in csProjs)
                {
                    var csFiles = csProj.GetCsFiles().ToArray();
                    foreach (var csFile in csFiles)
                    {

                    }
                }
            }
        }
        public static void AddJExtAndNHibExtReferences(string projectDirectory)
        {
            SlnFile[] solutionFiles = Directory.EnumerateFiles(projectDirectory, "*.sln", SearchOption.AllDirectories)
                .Select(file => new SlnFile(file))
                .ToArray();
            
                //{ @"C:\source\Mercury\src\Mercury.ReflectiveTests.sln" };
            foreach (var solutionFile in solutionFiles)
            {
                var csProjs = solutionFile.GetCsProjs().ToArray();
                foreach (var csProj in csProjs)
                {
                    var csFiles = csProj.GetCsFiles().ToArray();
                    foreach (var csFile in csFiles)
                    {
                        if (csFile.HasEvidenceOfJExt())
                        {
                            AddJExtReferences(csFile, csProj, solutionFile);
                        }
                        if (csFile.HasEvidenceOfNHibExt())
                        {
                            AddNHibExtReferences(csFile, csProj, solutionFile);
                        }
                    }
                }
            }
        }

        private static void AddJExtReferences(CsFile csFile, CsProj csProj, SlnFile slnFile)
        {
            csFile.AddUsingToCsFile("Mercury.Core.JsonExtensions");
            csFile.RemoveUsing("Mercury.Core.MessageSerialisation");
            if (!csProj.ContainsJExtProjectReference())
            {
                csProj.AddJExtProjectReference();
            }
            if (!slnFile.ContainsJExtProjectReference())
            {
                slnFile.AddJExtProjectReference();
            }
        }

        private static void AddNHibExtReferences(CsFile csFile, CsProj csProj, SlnFile slnFile)
        {
            csFile.AddUsingToCsFile("Mercury.Core.NHibernateExtensions");
            if (!csProj.ContainsNHibExtProjectReference())
            {
                csProj.AddNHibExtProjectReference();
            }

            if (!slnFile.ContainsNHibExtProjectReference())
            {
                slnFile.AddNHibExtProjectReference();
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

        //public static IEnumerable<SlnFile> GetFiles(string projectDirectory)
        //{
        //    return Directory.EnumerateFiles(projectDirectory, "*.sln", SearchOption.AllDirectories)
        //                .Select(slnPath => new SlnFile(slnPath));
        //}

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

        #region Abstract Later
        public static void UpdateProjectReference(CsProj toUpdate, ProjectReference referenceToReplace, string replacement)
        {
            var Regex = new Regex(referenceToReplace.Pattern);
            var csProjText = File.ReadAllText(toUpdate.Path);
            csProjText = Regex.Replace(csProjText, replacement);
            File.WriteAllText(toUpdate.Path, csProjText);
        }

        public static void UpdateNugetPackageReference(CsProj toUpdate, NugetPackageReference referenceToReplace, string replacement)
        {
            var Regex = new Regex(referenceToReplace.Pattern);
            var csProjText = File.ReadAllText(toUpdate.Path);
            csProjText = Regex.Replace(csProjText, replacement);
            File.WriteAllText(toUpdate.Path, csProjText);
        }
        #endregion

        //public static IEnumerable<string> GetFiles(string projectDirectory, string extension)
        //{
        //    return Directory.EnumerateFiles(projectDirectory, "*."+extension, SearchOption.AllDirectories);
        //}

        //public static void AddMissingReferencesTo(IEnumerable<string> csFiles)
        //{
        //    var collection = csFiles;
        //    var filteredCollection = collection.Where(CsFile.HasEvidenceOfJExt);
        //    foreach (string csFile in filteredCollection)
        //    {
        //        CsFile.AddUsingToCsFile(csFile, "Mercury.Core.JsonExtensions");
        //        CsProj.AddProjectReference(csFile, "");
        //        //SlnFile.AddCsProjToSolution(csFile, "");
        //    }
        //}
        public static void FixXml(string dir)
        {
            var csProjs = Directory.EnumerateFiles(dir, "*.csproj", SearchOption.AllDirectories)
                .Select(file => new CsProj(file));
            foreach (var csProj in csProjs)
            {
                Console.WriteLine("Checking: "+csProj);
                var csprojText = File.ReadAllText(csProj.Path);
                Regex reg = new Regex("(\\s+)*<\\?xml version=\\\"1\\.0\\\" encoding=\\\"utf-8\\\"\\?>(\\s+)<\\?xml version=\\\"1\\.0\\\" encoding=\\\"utf-8\\\"\\?>");
                csprojText = reg.Replace(csprojText, "<?xml version=\"1.0\" encoding=\"utf-8\"?>",1);
                File.WriteAllText(csProj.Path, csprojText);
                CsProj.ReformatXml(csProj.Path);
            }
        }

        public static void RemoveLogForNetReference(string[] filesToFix)
        {
            Regex reg = new Regex("(\\s)*<Reference Include=\\\"log4net(.*)\\\">(\\s)*(.)*(\\s)*(.)*(\\s)*<\\/Reference>");
            foreach (var file in filesToFix)
            {
                var csProjText = File.ReadAllText(file);
                csProjText = reg.Replace(csProjText, "");
                File.WriteAllText(file,csProjText);
            }
        }

        public static void FixNHibExtUsings(string[] files)
        {
            foreach (var file in files)
            {
                var csFile = new CsFile(file);
                csFile.RemoveUsing("Mercury.Core.NHibernateExtensions");
                if (csFile.HasEvidenceOfNHibExt())
                {
                    csFile.AddUsingToCsFile("Mercury.Core.NHibernateExtensions");
                }
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
    }
}
