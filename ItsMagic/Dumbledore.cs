﻿using System;
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
            string[] solutionFiles = Directory.EnumerateFiles(projectDirectory, "*.sln", SearchOption.AllDirectories).ToArray();
            //{ @"C:\source\Mercury\src\Mercury.ReflectiveTests.sln" };
            foreach (var solutionFile in solutionFiles)
            {
                var csProjs = SlnFile.GetCsProjs(solutionFile).ToArray();
                foreach (var csProj in csProjs)
                {
                    var csFiles = CsProj.GetCsFiles(csProj).ToArray();
                    foreach (var csFile in csFiles)
                    {

                    }
                }
            }
        }
        public static void AddJExtAndNHibExtReferences(string projectDirectory)
        {
            string[] solutionFiles = Directory.EnumerateFiles(projectDirectory, "*.sln", SearchOption.AllDirectories).ToArray();
                //{ @"C:\source\Mercury\src\Mercury.ReflectiveTests.sln" };
            foreach (var solutionFile in solutionFiles)
            {
                var csProjs = SlnFile.GetCsProjs(solutionFile).ToArray();
                foreach (var csProj in csProjs)
                {
                    var csFiles = CsProj.GetCsFiles(csProj).ToArray();
                    foreach (var csFile in csFiles)
                    {
                        if (CsFile.HasEvidenceOfJExt(csFile))
                        {
                            AddJExtReferences(csFile, csProj, solutionFile);
                        }
                        if (CsFile.HasEvidenceOfNHibExt(csFile))
                        {
                            AddNHibExtReferences(csFile, csProj, solutionFile);
                        }
                    }
                }
            }
        }

        private static void AddJExtReferences(string csFile, string csProj, string solutionFile)
        {
            CsFile.AddUsingToCsFile(csFile, "Mercury.Core.JsonExtensions");
            CsFile.RemoveUsing(csFile, "Mercury.Core.MessageSerialisation");
            if (!CsProj.ContainsJExtProjectReference(csProj))
            {
                CsProj.AddJExtProjectReference(csProj);
            }
            if (!SlnFile.ContainsJExtProjectReference(solutionFile))
            {
                SlnFile.AddJExtProjectReference(solutionFile);
            }
        }

        private static void AddNHibExtReferences(string csFile, string csProj, string solutionFile)
        {
            CsFile.AddUsingToCsFile(csFile, "Mercury.Core.NHibernateExtensions");
            if (!CsProj.ContainsNHibExtProjectReference(csProj))
            {
                CsProj.AddNHibExtProjectReference(csProj);
            }

            if (!SlnFile.ContainsNHibExtProjectReference(solutionFile))
            {
                SlnFile.AddNHibExtProjectReference(solutionFile);
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

        private static void ReformatXml(string file)
        {
            var doc = XDocument.Load(file);
            using (XmlTextWriter writer = new XmlTextWriter(file, System.Text.Encoding.UTF8))
            {
                writer.Formatting = Formatting.Indented;
                doc.Save(writer);
            }
        }

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
        public static void FixXML(string dir)
        {
            var csProjs = Directory.EnumerateFiles(dir, "*.csproj", SearchOption.AllDirectories);
            foreach (var csProj in csProjs)
            {
                Console.WriteLine("Checking: "+csProj);
                var csprojText = File.ReadAllText(csProj);
                Regex reg = new Regex("(\\s+)*<\\?xml version=\\\"1\\.0\\\" encoding=\\\"utf-8\\\"\\?>(\\s+)<\\?xml version=\\\"1\\.0\\\" encoding=\\\"utf-8\\\"\\?>");
                csprojText = reg.Replace(csprojText, "<?xml version=\"1.0\" encoding=\"utf-8\"?>",1);
                File.WriteAllText(csProj, csprojText);
                ReformatXml(csProj);
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
                CsFile.RemoveUsing(file, "Mercury.Core.NHibernateExtensions");
                if (CsFile.HasEvidenceOfNHibExt(file))
                {
                    CsFile.AddUsingToCsFile(file, "Mercury.Core.NHibernateExtensions");
                }
            }
        }
    }
}
