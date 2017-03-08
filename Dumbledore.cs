using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace ItsMagic
{
    static class Dumbledore
    {
        private static string itemGroup;
        private static string packagesTag;

        public static void UpdateProjectReferenceWithNugetReference(string csProjFileToUpdate, CsProj projectReferenceToRemove,
            NugetPackageReference referenceToAdd)
        {
            
        }

        public static void UpdateReference(string csProj, Reference reference)
        {
            //Find and Delete old Reference
            RemoveReference(csProj, reference);
            //Find <ItemGroup> - string replace <ItemGroup> with <ItemGroup>+Reference Info
            AddNugetReference(csProj, reference);
            reformatXML(csProj);
            UpdatePackagesConfig(Path.GetDirectoryName(csProj) + "\\packages.config", reference);
        }

        public static void RemoveReference(string csProj, Reference reference)
        {
            var Regex = new Regex(reference.ProjectRefPattern);
            var csProjText = File.ReadAllText(csProj);
            csProjText = Regex.Replace(csProjText, "", 1);
            File.WriteAllText(csProj, csProjText);
        }

        public static void AddNugetReference(string csProj, Reference reference)
        {
            var Regex = new Regex(itemGroup);
            var csProjText = File.ReadAllText(csProj);
            csProjText = Regex.Replace(csProjText, itemGroup + reference.NugetRef, 1);
            File.WriteAllText(csProj, csProjText);
        }

        private static void reformatXML(string file)
        {
            var doc = XDocument.Load(file);
            using (XmlTextWriter writer = new XmlTextWriter(file, System.Text.Encoding.UTF8))
            {
                writer.Formatting = Formatting.Indented;
                doc.Save(writer);
            }
        }

        private static void UpdatePackagesConfig(string packages, Reference reference)
        {
            var Regex = new Regex(packagesTag);
            CreateIfDoestExist(packages);
            var packagesText = File.ReadAllText(packages);
            packagesText = Regex.Replace(packagesText, packagesTag + reference.PackagesRef, 1);
            File.WriteAllText(packages, packagesText);
            reformatXML(packages);
        }

        private static void CreateIfDoestExist(string packages)
        {
            if (!File.Exists(packages))
            {
                File.WriteAllText(packages, "<?xml version=\"1.0\" encoding=\"utf-8\"?><packages></packages>");
            }
        }




        public static IEnumerable<string> GetSlnFiles(string projectDirectory)
        {
            return Directory.EnumerateFiles(projectDirectory, "*.sln", SearchOption.AllDirectories);
        }

        public static IEnumerable<string> GetCsProjs(string sln)
        {
            var dir = Path.GetDirectoryName(sln);
            var regex = new Regex("Project(.*) = .*, \"(?<proj>.*\\.csproj)\", \".*\"");
            return File.ReadAllLines(sln)
                .Select(f => regex.Match(f))
                .Where(m => m.Success)
                .Select(m => m.Groups["proj"].Value)
                .Select(relPath => Path.Combine(dir, relPath));
        }

        public static IEnumerable<string> GetCsFiles(string csproj)
        {
            var dir = Path.GetDirectoryName(csproj);
            var regex = new Regex("(?:<Compile Include=\\\")(?<cs>(?:(?:\\w+)\\\\*(?:\\w+))*\\.cs)(?:\\\" \\/>)");
            return File.ReadAllLines(csproj)
                .Select(f => regex.Match(f))
                .Where(m => m.Success)
                .Select(m => m.Groups["cs"].Value)
                .Select(relPath => Path.Combine(dir, relPath));
        }
    }
}
