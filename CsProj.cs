using System;
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
    }
}
