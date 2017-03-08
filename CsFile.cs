using System.Collections.Generic;
using System.Linq;

namespace ItsMagic
{
    public class CsFile
    {
        public string Name { get; set; }
        //public IEnumerable<string> Usings { get; set; }
        public string Path { get; private set; }

        public CsFile(string path)
        {
            Path = path;
            //Usings = GetUsings(path);
        }

        public static IEnumerable<string> Usings(string CsFilePath)
        {
            return RegexStore.Get(RegexStore.UsingsFromCsFilePattern, CsFilePath);
        }
    }
}