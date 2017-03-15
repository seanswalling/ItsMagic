using System;
using System.Collections.Generic;
using System.IO;

namespace ItsMagic
{
    public class CsFile
    {
        public string Path { get; private set; }

        public CsFile(string path)
        {
            Path = path;
        }

        public IEnumerable<string> Usings()
        {
            Console.WriteLine("Get Using Statements for: " + Path);
            return RegexStore.Get(RegexStore.UsingsFromCsFilePattern, Path);
        }

        public IEnumerable<string> GetLines()
        {
            using (var reader = new StreamReader(Path))
            {
                while (!reader.EndOfStream)
                {
                    yield return reader.ReadLine();
                }
            }
        }

        public bool HasEvidenceOfJExt()
        {
            var csFileText = File.ReadAllText(Path);
            return csFileText.Contains(".JsonCopy()")
                   || csFileText.Contains(".ToBson()")
                   || csFileText.Contains("SettingsFactory.Build()")
                   || csFileText.Contains("DateSerializer")
                   || csFileText.Contains("RequiredPropertyContractResolver")
                   || csFileText.Contains(".FromBson()");
        }

        public bool HasEvidenceOfNHibExt()
        {
            var csFileText = File.ReadAllText(Path);
            return csFileText.Contains(".Nullable()")
                   || csFileText.Contains(".NotNullable()")
                   && !Path.Contains("Mercury.Core.NHibernateExtensions.cs");
        }

        public void AddUsingToCsFile(string reference)
        {
            if (!File.ReadAllText(Path).Contains("using " + reference))
            {
                var csFileText = File.ReadAllText(Path);
                csFileText = "using " + reference + ";\r" + csFileText;
                File.WriteAllText(Path, csFileText);
            }
        }

        public void RemoveUsing(string reference)
        {
            var csFileText = File.ReadAllText(Path);
            if (csFileText.Contains("using " + reference))
            {
                var replace = csFileText.Replace("using " + reference + ";\r", "");
                File.WriteAllText(Path, replace);
            }
        }
    }
}