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
        private static readonly Dictionary<string, CsProj> CsProjPool = new Dictionary<string, CsProj>();
        private static readonly string[] HostPaths = GetHostPaths();

        private CsFile[] _csFilesCache;
        private CsProj[] _csProjCache;
        private NugetPackageReference[] _NuPkgCache;

        private CsProj(string path) : base(path)
        {
            if (Path.GetExtension(FilePath) != "csproj")
                throw new FileFormatException();

            if (File.Exists(FilePath))
            {
                Guid = new Librarian(_csProjGuidPattern, Text)
                    .Get("capturegroup")
                    .First()
                    .ToLower();
            }
        }

        public static CsProj Get(string path)
        {
            CsProj result;
            if (!CsProjPool.TryGetValue(path, out result))
            {
                result = new CsProj(path);
                CsProjPool.Add(path, result);
            }
            return result;
        }
        
        public string Guid { get; }
        public CsProj[] References => _csProjCache ?? (_csProjCache = GetProjectReferences());
        public NugetPackageReference[] NugetReferences => _NuPkgCache ?? (_NuPkgCache = GetNugetProjectDependencies());

        public CsFile[] CsFiles()
        {
            if (_csFilesCache != null) return _csFilesCache;
            var dir = Path.GetDirectoryName(FilePath);
            _csFilesCache = new Librarian(_csFilesPattern, Text)
                .Get("capturegroup")
                .Select(csFileRelPath => System.IO.Path.Combine(dir, csFileRelPath))
                .Where(File.Exists)
                .Select(file => CsFile.Get(file))
                .ToArray();
            return _csFilesCache;
        }

        public void AddNugetReference(NugetPackageReference referenceToAdd)
        {
            if (ContainsNugetProjectReference(referenceToAdd))
                return;

            Cauldron.Add($"Adding nuget Reference to {FilePath}");
            var isCopyLocal = IsHost();
            var regex = new Regex(_itemGroupTag);
            Text = regex.Replace(Text, _itemGroupTag + Environment.NewLine +
                                        $"<Reference Include=\"{referenceToAdd.Include}\">" + Environment.NewLine +
                                        $"<HintPath>{referenceToAdd.HintPath}</HintPath>" + Environment.NewLine +
                                        $"<Private>{isCopyLocal}</Private>" + Environment.NewLine +
                                        "</Reference>" + Environment.NewLine
                                        , 1);
            WriteFile();
            ReformatXml(FilePath);
            PackagesConfig().AddPackageEntry(referenceToAdd);
            _NuPkgCache = null;
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

            var regex = new Regex(_itemGroupProjectReferencePattern);
            Text = regex.Replace(Text, _itemGroupProjectReference +
                                "Include=\"" + projectRefPath + "\">" + Environment.NewLine +
                                "<Project>{" + referencedProject.Guid + "}</Project>" + Environment.NewLine +
                                "<Name>" + referencedProject.Name + "</Name>" + Environment.NewLine +
                                "</ProjectReference>" + Environment.NewLine +
                                "<ProjectReference ", 1);
            WriteFile();
            ReformatXml(FilePath);
            _csProjCache = null;
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
            if (new Librarian("<Project>{" + upperGuidRegex + "}<\\/Project>", Text).HasMatch() ||
                new Librarian("<Project>{" + lowerGuidRegex + "}<\\/Project>", Text).HasMatch())
                return true;
            return false;
        }
                
        public PackagesConfig PackagesConfig()
        {
            return new PackagesConfig(Directory.GetParent(FilePath) + @"\packages.config");
        }

        public void RemoveProjectReference(string projectGuid)
        {
            if (ContainsProjectReference(projectGuid))
            {
                Cauldron.Add($"Removing project reference with guid {projectGuid} from {Name}");
                var pattern = $".*(?:<ProjectReference.+(\\n*\\r*))(?:.*{projectGuid}.*(\\n*\\r*))(?:.+(\\n*\\r*))+?(?:.*<\\/ProjectReference>(\\n*\\r*))";
                Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
                Text = regex.Replace(Text, "");
                WriteFile();
                ReformatXml(FilePath);
                _csProjCache = null;
            }
            else
            {
                Cauldron.Add($"No project of GUID: {projectGuid} found");
            }
        }

        public bool IsHost()
        {
            //Does file name end with ".Tests.csproj"
            return FilePath.EndsWith(".Tests.csproj")
                   || Text.Contains("349c5851-65df-11da-9384-00065b846f21") //IsWebApp/Site/MVC/WebRole
                   || IsInHostPath(FilePath);

            // is in services.json
        }

        public string[] Classes()
        {
            return CsFiles().SelectMany(csFile => csFile.Classes)
                .Distinct()
                .ToArray();
        }
        public string[] ExtensionMethods()
        {
            return CsFiles().SelectMany(csFile => csFile.ExtensionMethods)
                .Distinct()
                .ToArray();
        }
        public string[] Usings()
        {
            return CsFiles().SelectMany(csFile => csFile.Usings)
                .Distinct()
                .ToArray();
        }
        
        public void RemoveCompileEntry(string csFileName)
        {
            var rstr = string.Format(_compileEntry, csFileName);
            var regex = new Regex(rstr);
            Text = regex.Replace(Text, "");
            WriteFile();
        }

        public static HashSet<CsProj> GetAllProjectReferences(CsProj csProj)
        {
            if (csProj.References.Length == 0)
                return new HashSet<CsProj>();

            HashSet<CsProj> references = new HashSet<CsProj>();
            references.AddRange(csProj.References);

            foreach (var csProjToTraverse in csProj.References)
            {
                references.AddRange(GetAllProjectReferences(csProjToTraverse));
            }
            return references;
        }

        public static IEnumerable<NugetPackageReference> GetNugetReferences(CsProj csproj)
        {
            return csproj.NugetReferences.Concat(
                    csproj.References.SelectMany(GetNugetReferences)
                )
                .Distinct();
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

        private CsProj[] GetProjectReferences(string[] removedProjects = null)
        {
            Cauldron.Add($"Getting Project references for {FilePath}");
            List<CsProj> dependencies = new List<CsProj>();
            foreach (var csProjRelPath in new Librarian(_csProjPathPattern, Text).Get("capturegroup"))
            {
                if (removedProjects != null && removedProjects.Contains(Path.GetFileName(csProjRelPath)))
                {
                    continue;
                }
                var path = Path.Combine(Directory.GetParent(FilePath).FullName, csProjRelPath);
                var csProjFullPath = Path.GetFullPath(path);
                dependencies.Add(Get(csProjFullPath));
            }
            return dependencies.ToArray();
        }

        private bool ContainsNugetProjectReference(NugetPackageReference nugetReference)
        {
            return Text.Contains($"<Reference Include=\"{nugetReference.DllName}");
        }

        private NugetPackageReference[] GetNugetProjectDependencies()
        {
            //This needs to be tidied up.
            //Should we get nuget references from packages.config followed by additional info from csproj, or the other way around?

            //Additional problem - How do I map csproj nuget references to packages.config entries. The Id's dont always align?

            Cauldron.Add($"Getting Nuget references for {FilePath}");
            List<NugetPackageReference> nugetReferences = new List<NugetPackageReference>();
            var references = new Librarian(_nugetReferencePattern, Text)
                .Get("capturegroup")
                .Where(token => token.Contains("\\packages\\"));  //i.e. ignore any "lib" references. Cant exclued by !contains(lib) as all packages have a lib folder
            foreach (var reference in references)
            {
                var dllName = new Librarian(_nugetDllNameFromNugetReferencePattern, reference)
                    .Get("capturegroup")
                    .Single();

                var nugetId = new Librarian(_nugetIdFromNugetReferencePattern, reference)
                    .Get("capturegroup")
                    .Single()
                    .TrimEnd('.');

                var hintPath = new Librarian(_nugetHintPathPattern, reference)
                    .Get("capturegroup")
                    .Single();

                var include = new Librarian(_nugetIncludeFromNugetReferencePattern, reference)
                    .Get("capturegroup")
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
                .Select(relPath => Wand.ToAbsolutePath(relPath, Wand.MercurySourceDir))
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

        private const string _itemGroupTag = "<ItemGroup>";
        private static string _itemGroupProjectReference = "<ItemGroup>" + Environment.NewLine + "<ProjectReference ";
        private const string _csFilesPattern = "<Compile Include=\\\"(?<capturegroup>(.*.cs))\\\"( \\/)*>";
        private const string _csProjGuidPattern = "<ProjectGuid>{(?<capturegroup>([\\d\\w-]*))}<\\/ProjectGuid>";
        private const string _itemGroupProjectReferencePattern = "<ItemGroup>\\s+<ProjectReference ";
        private const string _csProjPathPattern = "\"(?<capturegroup>(.*\\.csproj))\"";
        private const string _nugetReferencePattern = "(?<capturegroup>(<Reference Include=.*\\s+(\\s*<SpecificVersion>.*\\s*)*<HintPath>.+<\\/HintPath>\\s+.+\\s+<\\/Reference>))";
        private const string _nugetHintPathPattern = "<HintPath>(?<capturegroup>(.+))<\\/HintPath>";
        private const string _nugetDllNameFromNugetReferencePattern = "Include=\\\"(?<capturegroup>(\\w|\\.)+)";
        private const string _nugetIncludeFromNugetReferencePattern = "Include=\"(?<capturegroup>(.*))\">";
        private const string _nugetIdFromNugetReferencePattern = "packages\\\\(?<capturegroup>[\\w\\.-]+?)\\.\\d";
        private const string _compileEntry = "<Compile Include\\.+>\\n\\.+<Link>.+{0}</Link>\\n\\.+</Compile>";
    }
}
