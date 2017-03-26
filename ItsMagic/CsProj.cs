using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Build.Evaluation;

namespace ItsMagic
{
    public class CsProj : MagicFile
    {
        public CsFile[] _csFilesCache { get; private set; }
        public string _nameCache { get; private set; }
        public string _guidCache { get; private set; }
        public string[] Classes
        {
            get
            {
                return CsFiles().SelectMany(csFile => csFile.Classes())
                    .Distinct()
                    .ToArray();
            }
        }
        public string[] ExtensionMethods
        {
            get
            {
                return CsFiles().SelectMany(csFile => csFile.ExtensionMethods())                    
                    .Distinct()
                    .ToArray();
            }
        }
        public string[] Usings
        {
            get
            {
                return CsFiles().SelectMany(csFile => csFile.Usings())
                    .Distinct()
                    .ToArray();
            }
        }

        public CsProj(string path)
        {
            Path = path;
        }              

        public string Name()
        {
            if (_nameCache == null)
                _nameCache = System.IO.Path.GetFileNameWithoutExtension(Path);
            return _nameCache;
        }

        public string Guid()
        {
            if (_guidCache == null)
                _guidCache = RegexStore.Get(RegexStore.CsProjGuidPattern, Text()).First();
            return _guidCache;
        }

        public CsFile[] CsFiles()
        {
            if (_csFilesCache == null)
            {
                var dir = System.IO.Path.GetDirectoryName(Path);
                _csFilesCache = RegexStore.Get(RegexStore.CsFilesFromCsProjPattern, Text())
                        .Select(csFileRelPath => System.IO.Path.Combine(dir, csFileRelPath))
                        .Select(file => new CsFile(file))
                        .ToArray();
            }
            return _csFilesCache;
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

        public bool ContainsWeTcProjectReference()
        {
            throw new NotImplementedException();
        }

        public bool ContainsProjectReferenceOf(CsProj project)
        {
            var guid = project.Guid();
            var upperGuidRegex = guid.ToUpper().Replace("-", "\\-");
            var lowerGuidRegex = guid.ToLower().Replace("-", "\\-");
            if (RegexStore.Contains("<Project>{" + upperGuidRegex + "}<\\/Project>", Text()) ||
                RegexStore.Contains("<Project>{" + lowerGuidRegex + "}<\\/Project>", Text()))
                return true;
            return false;
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
            WriteText(csProjText);
            ReformatXml(Path);
        }

        internal void AddProjectReference(CsProj referencedProject, string projectDirectory)
        {
            if (Path.Contains(referencedProject.Name() + ".csproj"))
                return;

            var regex = new Regex(RegexStore.ItemGroupTag);

            Uri mercurySourcePath = new Uri(projectDirectory);
            Uri referencedProjectPath = new Uri(referencedProject.Path);
            Uri relPath = mercurySourcePath.MakeRelativeUri(referencedProjectPath);

            var newText = regex.Replace(Text(), RegexStore.ItemGroupTag +
                                                   "<ProjectReference Include=\"" + relPath.ToString().Replace("/","\\") + "\">" +
                                                   "<Project>{" + referencedProject.Guid() + "}</Project>" +
                                                   "<Name>" + referencedProject.Name() + "</Name>" +
                                                   "</ProjectReference>", 1);
            WriteText(newText);
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
            WriteText(csProjText);
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
            WriteText(csProjText);
            ReformatXml(Path);
        }

        public bool HasLogRepoReference()
        {
            return RegexStore.Get(RegexStore.LogRepoReferencePattern, Text()).Any();
        }

        public void UpdateLogRepoReference(string reference)
        {
            var csProjtext = File.ReadAllText(Path);
            csProjtext = csProjtext.Replace(reference, "\\Platform" + reference);
            WriteText(csProjtext);
        }

        //Functions to be deprecated
        public string[] LogRepoReferences()
        {
            return RegexStore.Get(RegexStore.LogRepoReferencePattern, Text()).ToArray();
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
