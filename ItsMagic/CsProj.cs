using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using NuGet;

namespace ItsMagic
{
    public class CsProj : MagicFile
    {
        public CsFile[] CsFilesCache { get; private set; }
        private string GuidCache { get; set; }

        public string Guid
            => GuidCache ?? (GuidCache = RegexStore.Get(RegexStore.CsProjGuidPattern, Text).First().ToLower());

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

        private CsProj[] ReferencesCache { get; set; }

        public CsProj[] References => ReferencesCache ?? (ReferencesCache = GetProjectDependencies());

        private CsProj[] GetProjectDependencies()
        {
            Cauldron.Add($"Getting Project references for {FilePath}");
            List<CsProj> dependencies = new List<CsProj>();
            foreach (var csProjRelPath in RegexStore.Get(RegexStore.CsProjPathFromCsProjPattern, Text))
            {
                // platform: x\x\x\x\x\Platform\y\y.proj
                //platform ref: ..\z\z.proj
                string commonFolder = Directory.GetParent(Directory.GetParent(FilePath).ToString()).ToString(); //what about multiple step backs?
                string csProjFullPath = csProjRelPath.Replace("..", commonFolder);
                dependencies.Add(new CsProj(csProjFullPath));
            }
            return dependencies.ToArray();
        }

        //private string[] NugetReferenceCache { get; set; }

        //public string[] NugetReferences => NugetReferenceCache ?? (NugetReferenceCache = GetNugetProjectDependencies());

        //private string[] GetNugetProjectDependencies()
        //{
        //    Cauldron.Add($"Getting Nuget references for {FilePath}");
        //    List<string> nugetReferences = new List<string>();
        //    foreach (var reference in RegexStore.Get(RegexStore.NugetReferenceFromCsProj, Text))
        //    {
        //        string nugetId = "";
        //        var srgsxegft = new PackageDependency(nugetId, );

        //        nugetReferences.Add(reference);
        //    }
        //    return nugetReferences.ToArray();
        //}

        public CsProj(string path)
        {
            FilePath = path;
        }              

        public CsFile[] CsFiles()
        {
            if (CsFilesCache != null) return CsFilesCache;
            var dir = System.IO.Path.GetDirectoryName(FilePath);
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

            if (ContainsProjectReference(referencedProject))
                return;

            Cauldron.Add($"Adding {referencedProject.FilePath} project reference to {FilePath}");
            Uri mercurySourcePath = new Uri(Dumbledore.MercurySourceDir);
            Uri referencedProjectPath = new Uri(referencedProject.FilePath);
            Uri relPath = mercurySourcePath.MakeRelativeUri(referencedProjectPath);

            var regex = new Regex(RegexStore.ItemGroupProjectReferencepattern);
            Text = regex.Replace(Text, RegexStore.ItemGroupProjectReference +
                                                "Include=\"" + relPath.ToString().Replace("src", "..\\..").Replace("/", "\\") + "\">" + Environment.NewLine +
                                                "<Project>{" + referencedProject.Guid + "}</Project>" + Environment.NewLine +
                                                "<Name>" + referencedProject.Name + "</Name>" + Environment.NewLine +
                                                "</ProjectReference>" + Environment.NewLine +
                                                "<ProjectReference ", 1);
            WriteFile();
            ReformatXml(FilePath);
        }
        
        public void RemoveProjectReference(string projectGuid)
        {
            if (ContainsProjectReference(projectGuid))
            {
                Cauldron.Add($"Removing project reference with guid {projectGuid} from {Name}");
                var pattern = $".*(?:<ProjectReference.+(\\n*\\r*))(?:.*{projectGuid}.*(\\n*\\r*))(?:.+(\\n*\\r*))+?(?:.*<\\/ProjectReference>(\\n*\\r*))";
                Regex regex = new Regex(pattern);
                Text = regex.Replace(Text, "");
                WriteFile();
                ReformatXml(FilePath);
            }
            else
            {
                Cauldron.Add($"No project of GUID: {projectGuid} found");
            }
        }

        //public void AddNugetPackage(string packageId)
        //{
        //    IPackageRepository repo = PackageRepositoryFactory.Default.CreateRepository("https://packages.nuget.org/api/v2");
        //    List<IPackage> packages = repo.FindPackagesById(packageId).ToList();


        //    string path = "";
        //    PackageManager packageManager = new PackageManager(repo, path);
        //}

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
            var csProjText = File.ReadAllText(FilePath);

            Text = regex.Replace(csProjText, RegexStore.ItemGroupTag +
                                                   "<Reference Include=\"NewRelic.Api.Agent, Version=5.19.47.0, Culture=neutral, PublicKeyToken=06552fced0b33d87, processorArchitecture=MSIL\">" +
                                                   "<HintPath>..\\..\\packages\\NewRelic.Agent.Api.5.19.47.0\\lib\\NewRelic.Api.Agent.dll</HintPath>" +
                                                   "<Private>True</Private>" +
                                                   "</Reference>", 1);
            WriteFile();
            ReformatXml(FilePath);
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
