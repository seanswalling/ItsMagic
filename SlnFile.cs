using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItsMagic
{
    class SlnFile
    {
        public string Path {get; private set;}
        public IEnumerable<CsProj> CsProjs { get; set; }

        public SlnFile(string path)
        {
            Path = path;
            CsProjs = GetCsProjs(path);
        }

        public static IEnumerable<CsProj> GetCsProjs(string slnPath)
        {
            var dir = System.IO.Path.GetDirectoryName(slnPath);
            return RegexStore.Get(RegexStore.CsProjFromSlnPattern,slnPath)
                .Select(CsProjRelPath => System.IO.Path.Combine(dir, CsProjRelPath))
                .Select(CsProjFilePath => new CsProj(CsProjFilePath));
        }
    }
}
