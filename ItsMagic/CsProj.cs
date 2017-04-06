using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;

namespace Dumbledore
{
    public class CsProj : MagicFile
    {
        private static Dictionary<string, CsProj> CsProjPool = new Dictionary<string, CsProj>();
        private static readonly string[] HostPaths = GetHostPaths();

        private readonly bool _isHost;

        private CsProj[] _referencesCache;
        private string _guidCache;
        private NugetPackageReference[] _nugetReferenceCache;

        public CsFile[] CsFilesCache { get; private set; }

        public string Guid => _guidCache ?? (_guidCache = RegexStore.Get(RegexStore.CsProjGuidPattern, Text).First().ToLower());

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

        public CsProj[] References => _referencesCache ?? (_referencesCache = GetProjectDependencies());
        
        public NugetPackageReference[] NugetReferences => _nugetReferenceCache ?? (_nugetReferenceCache = GetNugetProjectDependencies());


        public void AddNugetReference(NugetPackageReference referenceToAdd)
        {
            if (ContainsNugetProjectReference(referenceToAdd))
                return;

            Cauldron.Add($"Adding nuget Reference to {FilePath}");
            var isCopyLocal = _isHost;
            var regex = new Regex(RegexStore.ItemGroupTag);
            Text = regex.Replace(Text,RegexStore.ItemGroupTag + Environment.NewLine +
                                        $"<Reference Include=\"{referenceToAdd.Include}\">" + Environment.NewLine +
                                        $"<HintPath>{referenceToAdd.HintPath}</HintPath>" + Environment.NewLine +
                                        $"<Private>{isCopyLocal}</Private>" + Environment.NewLine +
                                        "</Reference>" + Environment.NewLine
                                        , 1);
            WriteFile();
            ReformatXml(FilePath);
            PackagesConfig().AddPackageEntry(referenceToAdd);
        }

        private bool ContainsNugetProjectReference(NugetPackageReference nugetReference)
        {
            return Text.Contains($"<Reference Include=\"{nugetReference.DllName}");
            //return Text.Contains(nugetReference.DllName);
        }

        private CsProj(string path)
        {
            if(!File.Exists(path))
                throw new FileNotFoundException(path);
            FilePath = path;
            _isHost = IsHost();
        }

        public static CsProj GetCsProj(string path)
        {
            CsProj result;
            if (!CsProjPool.TryGetValue(path, out result))
            {
                result = new CsProj(path);
                CsProjPool.Add(path, result);
            }
            return result;
        }

        private CsProj[] GetProjectDependencies()
        {
            Cauldron.Add($"Getting Project references for {FilePath}");
            List<CsProj> dependencies = new List<CsProj>();
            foreach (var csProjRelPath in RegexStore.Get(RegexStore.CsProjPathFromCsProjPattern, Text))
            {
                var path = Path.Combine(Directory.GetParent(FilePath).FullName, csProjRelPath);
                var csProjFullPath = Path.GetFullPath(path);
                dependencies.Add(GetCsProj(csProjFullPath));
            }
            return dependencies.ToArray();
        }
        public PackagesConfig PackagesConfig()
        {
            return new PackagesConfig(Directory.GetParent(FilePath) + @"\packages.config");
        }

        private NugetPackageReference[] GetNugetProjectDependencies()
        {
            //This needs to be tidied up.
            //Should we get nuget references from packages.config followed by additional info from csproj, or the other way around?

            //Additional problem - How do I map csproj nuget references to packages.config entries. The Id's dont always align?

            Cauldron.Add($"Getting Nuget references for {FilePath}");
            List<NugetPackageReference> nugetReferences = new List<NugetPackageReference>();
            var references = RegexStore.Get(RegexStore.NugetReferenceFromCsProjPattern, Text)
                .Where(token=>token.Contains("\\packages\\"));  //i.e. ignore any "lib" references. Cant exclued by !contains(lib) as all packages have a lib folder
            foreach (var reference in references)
            {
                var dllName = RegexStore
                    .Get(RegexStore.NugetDllNameFromNugetReference, reference)
                    .Single();

                var nugetId = RegexStore
                    .Get(RegexStore.NugetIdFromNugetReference, reference)
                    .Single()
                    .TrimEnd('.');

                var hintPath = RegexStore
                    .Get(RegexStore.NugetHintPathFromNugetReferencePattern, reference)
                    .Single();

                var include = RegexStore
                    .Get(RegexStore.NugetIncludeFromNugetReferencePattern, reference)
                    .Single();

                var pc = PackagesConfig();
                var nr = pc
                    .NugetpackageReferences;
                var packagesConfigEntry = nr
                        .Single(entry => string.Equals(entry.Id, nugetId, StringComparison.InvariantCultureIgnoreCase));

                var nugetReference = new NugetPackageReference(nugetId, dllName, hintPath, include, packagesConfigEntry);
                nugetReferences.Add(nugetReference);
            }
            return nugetReferences.ToArray();
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
            Uri mercurySourcePath = new Uri(FilePath);
            Uri referencedProjectPath = new Uri(referencedProject.FilePath);
            Uri relPath = mercurySourcePath.MakeRelativeUri(referencedProjectPath);
            var projectRefPath = relPath.ToString().Replace("/", "\\");

            var regex = new Regex(RegexStore.ItemGroupProjectReferencepattern);
            Text = regex.Replace(Text, RegexStore.ItemGroupProjectReference +
                                "Include=\"" + projectRefPath + "\">" + Environment.NewLine +
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

        private bool IsHost()
        {
            //Does file name end with ".Tests.csproj"
            return FilePath.EndsWith(".Tests.csproj")
                   || Text.Contains("349c5851-65df-11da-9384-00065b846f21") //IsWebApp/Site/MVC/WebRole
                   || IsInHostPath(FilePath);

            // is in services.json
        }

        private static bool IsInHostPath(string path)
        {
            return HostPaths.Any(path.StartsWith);
        }
        private static string[] GetHostPaths()
        {
            var serviceJsonPath = Path.Combine(Wand.MercurySourceDir, @"..\Hosting\AllServices.json");
            var serviceJson = File.ReadAllText(serviceJsonPath);
            var roles = (JArray)JObject.Parse(serviceJson)["Roles"];
            return roles.Select(r => r["Assembly"].ToString())
                .Select(relPath => Helper.ToAbsolutePath(relPath, Wand.MercurySourceDir))
                //Strip everything after \bin\
                .Select(absPath => GetUntil(absPath, @"\bin\"))
                .ToArray();
            //
        }

        private static string GetUntil(string input, string delimiter)
        {
            var idx = input.IndexOf(delimiter);
            return input.Substring(0, idx);
        }

    }
}
