using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace ItsMagic
{
    static class Dumbledore
    {

        public static void UpdateProjectReferenceWithNugetReference(CsProj toUpdate, ProjectReference reference,
            NugetPackageReference referenceToAdd)
        {
            RemoveReference(toUpdate, reference);
            AddNugetReference(toUpdate, reference);
            ReformatXml(toUpdate.Path);
        }

        public static void RemoveReference(CsProj csProj, ProjectReference reference)
        {
            var Regex = new Regex(reference.ProjectRefPattern);
            var csProjText = File.ReadAllText(csProj.Path);
            csProjText = Regex.Replace(csProjText, "", 1);
            File.WriteAllText(csProj.Path, csProjText);
        }

        public static void AddNugetReference(CsProj csProj, ProjectReference reference)
        {
            var Regex = new Regex(RegexStore.ItemGroupTag);
            var csProjText = File.ReadAllText(csProj.Path);
            csProjText = Regex.Replace(csProjText, RegexStore.ItemGroupTag + reference.NugetRef, 1);
            File.WriteAllText(csProj.Path, csProjText);
            UpdatePackagesConfig(Path.GetDirectoryName(csProj.Path) + "\\packages.config", reference);
        }

        private static void UpdatePackagesConfig(string packages, ProjectReference reference)
        {
            var Regex = new Regex(RegexStore.PackagesTag);
            if (!File.Exists(packages))
            {
                File.WriteAllText(packages, "<?xml version=\"1.0\" encoding=\"utf-8\"?><packages></packages>");
            }
            var packagesText = File.ReadAllText(packages);
            packagesText = Regex.Replace(packagesText, RegexStore.PackagesTag + reference.PackagesRef, 1);
            File.WriteAllText(packages, packagesText);
            ReformatXml(packages);
        }

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

        //public static IEnumerable<SlnFile> GetFiles(string projectDirectory)
        //{
        //    return Directory.EnumerateFiles(projectDirectory, "*.sln", SearchOption.AllDirectories)
        //                .Select(slnPath => new SlnFile(slnPath));
        //}      
        public static IEnumerable<string> GetFiles(string projectDirectory, string extension)
        {
            return Directory.EnumerateFiles(projectDirectory, "*."+extension, SearchOption.AllDirectories);
        }

        public static IEnumerable<string> ReadLines(string file)
        {
            using(var reader = new StreamReader(file))
            {
                while (!reader.EndOfStream)
                {
                    yield return reader.ReadLine();
                }
            }
        }
    }
}
