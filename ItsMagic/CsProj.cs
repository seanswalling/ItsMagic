using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace ItsMagic
{
    public class CsProj : MagicFile
    {
        public CsFile[] CsFilesCache { get; private set; }
        private string GuidCache { get; set; }
        public string Guid => GuidCache ?? (GuidCache = RegexStore.Get(RegexStore.CsProjGuidPattern, Text).First().ToLower());
        public string[] Classes
        {
            get
            {
                return CsFiles().SelectMany(csFile => csFile.Classes)
                    .Distinct()
                    .ToArray();
            }
        }
        public string[] ExtensionMethods
        {
            get
            {
                return CsFiles().SelectMany(csFile => csFile.ExtensionMethods)                    
                    .Distinct()
                    .ToArray();
            }
        }
        public string[] Usings
        {
            get
            {
                return CsFiles().SelectMany(csFile => csFile.Usings)
                    .Distinct()
                    .ToArray();
            }
        }

        public CsProj(string path)
        {
            Filepath = path;
        }              

        public CsFile[] CsFiles()
        {
            if (CsFilesCache != null) return CsFilesCache;
            var dir = System.IO.Path.GetDirectoryName(Filepath);
            CsFilesCache = RegexStore.Get(RegexStore.CsFilesFromCsProjPattern, Text)
                .Select(csFileRelPath => System.IO.Path.Combine(dir, csFileRelPath))
                .Where(File.Exists)
                .Select(file => new CsFile(file))
                .ToArray();
            return CsFilesCache;
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
        
        public bool ContainsProjectReference(string projectGuid)
        {
            return Text.Contains($"<Project>{{{projectGuid.ToLower()}}}</Project>") || Text.Contains($"<Project>{{{projectGuid.ToUpper()}}}</Project>");
        }
        
        public bool ContainsProjectReference(CsProj project)
        {
            var guid = project.Guid;
            var upperGuidRegex = guid.ToUpper().Replace("-", "\\-");
            var lowerGuidRegex = guid.ToLower().Replace("-", "\\-");
            if (RegexStore.Contains("<Project>{" + upperGuidRegex + "}<\\/Project>", Text) ||
                RegexStore.Contains("<Project>{" + lowerGuidRegex + "}<\\/Project>", Text))
                return true;
            return false;
        }

        public void AddProjectReference(CsProj referencedProject)
        {
            if (this == referencedProject)
                return;

            Uri mercurySourcePath = new Uri(Dumbledore.MercurySourceDir);
            Uri referencedProjectPath = new Uri(referencedProject.Filepath);
            Uri relPath = mercurySourcePath.MakeRelativeUri(referencedProjectPath);

            var regex = new Regex(RegexStore.ItemGroupProjectReferencepattern);
            var newText = regex.Replace(Text, RegexStore.ItemGroupProjectReference +
                                                "Include=\"" + relPath.ToString().Replace("src", "..\\..").Replace("/", "\\") + "\">" + Environment.NewLine +
                                                "<Project>{" + referencedProject.Guid + "}</Project>" + Environment.NewLine +
                                                "<Name>" + referencedProject.Name + "</Name>" + Environment.NewLine +
                                                "</ProjectReference>" + Environment.NewLine +
                                                "<ProjectReference ", 1);
            WriteFile(newText);
            ReformatXml(Filepath);
        }
        
        public void RemoveProjectReference(string projectGuid)
        {
            var pattern = $".*(?:<ProjectReference.+(\\n*\\r*))(?:.*{projectGuid}.*(\\n*\\r*))(?:.+(\\n*\\r*))+?(?:.*<\\/ProjectReference>(\\n*\\r*))";
            Regex regex = new Regex(pattern);
            var replacementText = regex.Replace(Text, "");
            WriteFile(replacementText);
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

        //Functions to be deprecated

        public void AddNewRelicProjectReference()
        {
            var regex = new Regex(RegexStore.ItemGroupTag);
            var csProjText = File.ReadAllText(Filepath);

            csProjText = regex.Replace(csProjText, RegexStore.ItemGroupTag +
                                                   "<Reference Include=\"NewRelic.Api.Agent, Version=5.19.47.0, Culture=neutral, PublicKeyToken=06552fced0b33d87, processorArchitecture=MSIL\">" +
                                                   "<HintPath>..\\..\\packages\\NewRelic.Agent.Api.5.19.47.0\\lib\\NewRelic.Api.Agent.dll</HintPath>" +
                                                   "<Private>True</Private>" +
                                                   "</Reference>", 1);
            WriteFile(csProjText);
            ReformatXml(Filepath);
        }

        //public void AddProjectReference(string reference) //Nuget Version?
        //{
        //    var regex = new Regex("Some Pattern Here");
        //    var csProjText = File.ReadAllText(Path);

        //    csProjText = regex.Replace(csProjText, "Something here", 1);
        //    WriteText(csProjText);
        //    UpdatePackagesConfig(System.IO.Path.GetDirectoryName(Path) + "\\packages.config", reference);
        //} 
    }
}
