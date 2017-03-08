using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

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
        public IEnumerable<CsFile> CsFiles { get; set; }

        public CsProj(string path)
        {
            Path = path;
            CsFiles = GetCsFiles(path);
        }

        public static IEnumerable<CsFile> GetCsFiles(string CsProjPath)
        {
            var dir = System.IO.Path.GetDirectoryName(CsProjPath);
            return RegexStore.Get(RegexStore.CsFilesFromCsProjPattern, CsProjPath)
                    .Select(CsFileRelPath => System.IO.Path.Combine(dir, CsFileRelPath))
                    .Select(CsFilePath => new CsFile(CsProjPath));
        }
    }
}
