using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace Dumbledore
{
    class PackagesConfig : MagicFile
    {
        private PackagesConfigEntry[] NugetpackageReferencesCache { get; set; }
        public PackagesConfigEntry[] NugetpackageReferences => NugetpackageReferencesCache ?? (NugetpackageReferencesCache = GetNugetPackageReferences());

        public PackagesConfig(string path)
        {
            if (!File.Exists(path))
                File.WriteAllText(path, RegexStore.PackagesConfigDefault);
            FilePath = path;
        }

        private PackagesConfigEntry[] GetNugetPackageReferences()
        {
            List<PackagesConfigEntry> nugetPackageReferences = new List<PackagesConfigEntry>();
            foreach (var package in RegexStore.Get(RegexStore.PackageFromPackagesConfigPattern, Text))
            {
                nugetPackageReferences.Add(new PackagesConfigEntry(
                    RegexStore.Get(RegexStore.PackageIdFromPackagesPattern, package).Single(),
                    RegexStore.Get(RegexStore.PackageVersionFromPackagesPattern, package).Single(),
                    RegexStore.Get(RegexStore.PackageTargetFrameworkFromPackagesPattern, package).Single()));
            }

            return nugetPackageReferences.ToArray();
        }

        internal void AddPackageEntry(NugetPackageReference referenceToAdd)
        {
            if (ContainsEntry(referenceToAdd))
                return;

            Cauldron.Add($"Adding Package Entry {referenceToAdd.Id} to {FilePath}");
            var regex = new Regex(RegexStore.PackagesTag);
            Text = regex.Replace(Text, RegexStore.PackagesTag + 
                                      Environment.NewLine +
                                      $"<package id=\"{referenceToAdd.Id}\" " +
                                      $"version=\"{referenceToAdd.Version}\" " +
                                      $"targetFramework=\"{referenceToAdd.TargetFramework}\" />");
            WriteFile();
            ReformatXml();
        }

        private bool ContainsEntry(NugetPackageReference referenceToAdd)
        {
            return Text.Contains(referenceToAdd.Id);
        }

        private void ReformatXml()
        {
            var doc = XDocument.Load(FilePath);
            using (XmlTextWriter writer = new XmlTextWriter(FilePath, System.Text.Encoding.UTF8))
            {
                writer.Formatting = Formatting.Indented;
                doc.Save(writer);
            }
        }
    }
}
