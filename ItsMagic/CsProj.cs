using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace ItsMagic
{
    class CsProj
    {
        public string Path { get; private set; }
        public string Guid { get; set; }
        public string OutputType { get; set; }
        public string RootNamespace { get; set; }
        public string TargetFrameworkVersion { get; set; }
        public string [] NugetPackageReferences { get; set; }
        public CsProj [] ProjectReferences { get; set; }
        //public IEnumerable<CsFile> CsFiles { get; set; }

        public CsProj(string path)
        {
            Path = path;
            //CsFiles = GetCsFiles(path);
        }

        //private static IEnumerable<CsFile> GetCsFiles(string CsProjPath)
        //{
        //    var dir = System.IO.Path.GetDirectoryName(CsProjPath);
        //    return RegexStore.Get(RegexStore.CsFilesFromCsProjPattern, CsProjPath)
        //            .Select(CsFileRelPath => System.IO.Path.Combine(dir, CsFileRelPath))
        //            .Select(CsFilePath => new CsFile(CsFilePath));
        //}

        public static IEnumerable<string> GetCsFiles(string csProjPath)
        {
            Console.WriteLine("Get Cs Files for: "+csProjPath);
            var dir = System.IO.Path.GetDirectoryName(csProjPath);
            return RegexStore.Get(RegexStore.CsFilesFromCsProjPattern, csProjPath)
                    .Select(csFileRelPath => System.IO.Path.Combine(dir, csFileRelPath));
        }

        public static void AddProjectReference(string csProj, string reference)
        {
            var regex = new Regex("Some Pattern Here");
            var csProjText = File.ReadAllText(csProj);

            csProjText = regex.Replace(csProjText, "Something here", 1);
            File.WriteAllText(csProj, csProjText);
            UpdatePackagesConfig(System.IO.Path.GetDirectoryName(csProj) + "\\packages.config", reference);
        }

        private static void UpdatePackagesConfig(string packages, string reference)
        {
            var regex = new Regex(RegexStore.PackagesTag);
            if (!File.Exists(packages))
            {
                File.WriteAllText(packages, RegexStore.PackagesConfigDefault);
            }
            var packagesText = File.ReadAllText(packages);
            packagesText = regex.Replace(packagesText, RegexStore.PackagesTag + reference, 1);
            File.WriteAllText(packages, packagesText);
            ReformatXml(packages);
        }

        private static void ReformatXml(string file)
        {
            var doc = XDocument.Load(file);
            using (XmlTextWriter writer = new XmlTextWriter(file, System.Text.Encoding.UTF8))
            {
                writer.Formatting = Formatting.Indented;
                doc.Save(writer);
            }
        }
    }
}
