using System;
using System.Collections.Generic;
using System.IO;
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

        public static IEnumerable<string> Usings(string csFilePath)
        {
            Console.WriteLine("Get Using Statements for: " + csFilePath);
            return RegexStore.Get(RegexStore.UsingsFromCsFilePattern, csFilePath);
        }

        public static IEnumerable<string> GetLines(string csFile)
        {
            using (var reader = new StreamReader(csFile))
            {
                while (!reader.EndOfStream)
                {
                    yield return reader.ReadLine();
                }
            }
        }

        public static bool HasEvidenceOfJExt(string csFile)
        {
            var fileLines = CsFile.GetLines(csFile).ToArray();
            return fileLines.Contains(".JsonCopy()")
                   || fileLines.Contains(".ToBson()")
                   || fileLines.Contains("SettingsFactory.Build()")
                   || fileLines.Contains("DateSerializer")
                   || fileLines.Contains("RequiredPropertyContractResolver")
                   || fileLines.Contains(".FromBson()");
        }
    }
}