using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace ItsMagic
{
    public class CsProj
    {
        public string Path { get; private set; }

        public CsProj(string path)
        {
            Path = path;
            //CsFiles = GetCsFiles(path);
        }
        
        public IEnumerable<CsFile> GetCsFiles()
        {
            Console.WriteLine("Get Cs Files for: "+Path);
            var dir = System.IO.Path.GetDirectoryName(Path);
            return RegexStore.Get(RegexStore.CsFilesFromCsProjPattern, Path)
                    .Select(csFileRelPath => System.IO.Path.Combine(dir, csFileRelPath))
                    .Select(file => new CsFile(file));
        }

        public void AddProjectReference(string reference)
        {
            var regex = new Regex("Some Pattern Here");
            var csProjText = File.ReadAllText(Path);

            csProjText = regex.Replace(csProjText, "Something here", 1);
            File.WriteAllText(Path, csProjText);
            UpdatePackagesConfig(System.IO.Path.GetDirectoryName(Path) + "\\packages.config", reference);
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

        public static string ReformatXml(string file)
        {
            var doc = XDocument.Load(file);
            using (XmlTextWriter writer = new XmlTextWriter(file, System.Text.Encoding.UTF8))
            {
                writer.Formatting = Formatting.Indented;
                doc.Save(writer);
            }
            return file;
        }

        public bool ContainsJExtProjectReference()
        {
            return File.ReadAllText(Path).Contains("<Project>{d3dc56b0-8b95-47a5-a086-9e7a95552364}</Project>");
        }

        public bool ContainsNHibExtProjectReference()
        {
            return File.ReadAllText(Path).Contains("<Project>{f1575997-02d0-486f-ae36-69f6a3b37c39}</Project>");
        }

        public void AddJExtProjectReference()
        {
            if (Path.Contains("Mercury.Core.JsonExtensions.csproj"))
                return;
            
            var regex = new Regex(RegexStore.ItemGroupTag);
            var csProjText = File.ReadAllText(Path);

            csProjText = regex.Replace(csProjText, RegexStore.ItemGroupTag +
                                                   "<ProjectReference Include=\"..\\..\\Platform\\Mercury.Core.JsonExtensions\\Mercury.Core.JsonExtensions.csproj\">" +
                                                   "<Project>{d3dc56b0-8b95-47a5-a086-9e7a95552364}</Project>" +
                                                   "<Name>Mercury.Core.JsonExtensions</Name>" +
                                                   "</ProjectReference>", 1);
            File.WriteAllText(Path, csProjText);
            ReformatXml(Path);
        }

        public void AddNHibExtProjectReference()
        {
            if (Path.Contains("Mercury.Core.NHibernateExtensions.csproj"))
                return;

            var regex = new Regex(RegexStore.ItemGroupTag);
            var csProjText = File.ReadAllText(Path);

            csProjText = regex.Replace(csProjText, RegexStore.ItemGroupTag +
                                                   "<ProjectReference Include=\"..\\..\\Platform\\Mercury.Core.NHibernateExtensions\\Mercury.Core.NHibernateExtensions.csproj\">" +
                                                   "<Project>{f1575997-02d0-486f-ae36-69f6a3b37c39}</Project>" +
                                                   "<Name>Mercury.Core.NHibernateExtensions</Name>" +
                                                   "</ProjectReference>", 1);
            File.WriteAllText(Path, csProjText);
            ReformatXml(Path);
        }

        public void AddNewRelicProjectReference()
        {
            var regex = new Regex(RegexStore.ItemGroupTag);
            var csProjText = File.ReadAllText(Path);

            csProjText = regex.Replace(csProjText, RegexStore.ItemGroupTag +
                                                   "<Reference Include=\"NewRelic.Api.Agent, Version=5.19.47.0, Culture=neutral, PublicKeyToken=06552fced0b33d87, processorArchitecture=MSIL\">" +
                                                   "<HintPath>..\\..\\packages\\NewRelic.Agent.Api.5.19.47.0\\lib\\NewRelic.Api.Agent.dll</HintPath>" +
                                                   "<Private>True</Private>" +
                                                   "</Reference>", 1);
            File.WriteAllText(Path, csProjText);
            ReformatXml(Path);
        }
    }
}
