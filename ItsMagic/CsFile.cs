using System;
using System.Collections.Generic;
using System.IO;

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
            var csFileText = File.ReadAllText(csFile);
            return (csFileText.Contains(".JsonCopy()")
                   || csFileText.Contains(".ToBson()")
                   || csFileText.Contains("SettingsFactory.Build()")
                   || csFileText.Contains("DateSerializer")
                   || csFileText.Contains("RequiredPropertyContractResolver")
                   || csFileText.Contains(".FromBson()"))
                   && !csFileText.Contains("using Mercury.Core.JsonExtensions");
        }

        public static void AddUsingToCsFile(string csFile, string reference)
        {
            var csFileText = File.ReadAllText(csFile);
            csFileText = "using " + reference + "\n" + csFileText;
            File.WriteAllText(csFile,csFileText);
        }
    }
}