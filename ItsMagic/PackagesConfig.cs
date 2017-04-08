using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace Dumbledore
{
    public class PackagesConfig
    {
        private PackagesConfigEntry[] NugetpackageReferencesCache { get; set; }
        public PackagesConfigEntry[] NugetpackageReferences => NugetpackageReferencesCache ?? (NugetpackageReferencesCache = GetNugetPackageReferences());

        public string FilePath { get; set; }
        public string Text { get; set; }

        public PackagesConfig(string path)
        {
            if (!File.Exists(path))
                File.WriteAllText(path, _packagesConfigDefault);
            FilePath = path;
            Text = File.ReadAllText(FilePath);
        }

        private PackagesConfigEntry[] GetNugetPackageReferences()
        {

            List<PackagesConfigEntry> nugetPackageReferences = new List<PackagesConfigEntry>();
            foreach (var package in RegexStore.Get(_packagePattern, Text))
            {
                nugetPackageReferences.Add(new PackagesConfigEntry(
                    RegexStore.Get(_packageIdPattern, package).Single(),
                    RegexStore.Get(_packageVersionPattern, package).Single(),
                    RegexStore.Get(_packageTargetFrameworkPattern, package).Single()));
            }

            return nugetPackageReferences.ToArray();
        }

        internal void AddPackageEntry(NugetPackageReference referenceToAdd)
        {
            if (ContainsEntry(referenceToAdd))
                return;

            Cauldron.Add($"Adding Package Entry {referenceToAdd.Id} to {FilePath}");
            var regex = new Regex(_packagesTag);
            Text = regex.Replace(Text, _packagesTag + 
                                      Environment.NewLine +
                                      $"<package id=\"{referenceToAdd.Id}\" " +
                                      $"version=\"{referenceToAdd.Version}\" " +
                                      $"targetFramework=\"{referenceToAdd.TargetFramework}\" />");
             
            File.WriteAllText(FilePath, Text);
        
            ReformatXml();
        }

        private bool ContainsEntry(NugetPackageReference referenceToAdd)
        {
            return Text.Contains($"id=\"{referenceToAdd.Id}\"");
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

        private const string _packagesTag = "<packages>";
        private const string _packagesConfigDefault = "<?xml version =\"1.0\" encoding=\"utf-8\"?>\r\n<packages>\r\n</packages>";
        private const string _packagePattern = "(?<capturegroup>(<package.*\\/>))";
        private const string _packageIdPattern = "id=\"(?<capturegroup>[\\w\\.-]+)\"";
        private const string _packageVersionPattern = "version=\"(?<capturegroup>[\\w\\.-]*)\"";
        private const string _packageTargetFrameworkPattern = "targetFramework=\"(?<capturegroup>(.*))\"";
    }
}
