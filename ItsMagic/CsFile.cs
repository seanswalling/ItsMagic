using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Dumbledore
{
    public class CsFile : MagicFile
    {
        private static readonly Dictionary<string, CsFile> CsFilePool = new Dictionary<string, CsFile>();

        private string[] ClassesCache { get; set; }
        private string[] UsingsCache { get; set; }
        private string[] ExtensionMethodsCache { get; set; }

        private CsFile(string path) : base(path)
        {
                
        }

        public static CsFile Get(string path)
        {
            CsFile result;
            if (!CsFilePool.TryGetValue(path, out result))
            {
                result = new CsFile(path);
                CsFilePool.Add(path, result);
            }
            return result;
        }

        public string[] Classes => ClassesCache ?? (ClassesCache = GetClasses());
        public string[] Usings => UsingsCache ?? (UsingsCache = GetUsings());
        public string[] ExtensionMethods => ExtensionMethodsCache ?? (ExtensionMethodsCache = GetExtensionMethods());

        public string[] textAsLines { get; set; }

        public void ReadLines()
        {
            textAsLines = File.ReadAllLines(FilePath);
        }

        public void RemoveLines(int[] lineNumbersToRemove)
        {
            textAsLines = textAsLines.Where((line, index) => !lineNumbersToRemove.Contains(index + 1)).ToArray();
        }

        public void WriteLines()
        {
            File.WriteAllLines(FilePath, textAsLines);
        }

        public void AddUsing(string reference)
        {
            Cauldron.Add($"Add Using: {reference} to {Name}.cs");
            if (!Text.Contains("using " + reference + ";"))
            {
                Text = "using " + reference + ";" + Environment.NewLine + Text;
                WriteFile();
                UsingsCache = new Librarian(_usingsPattern, Text).Get("capturegroup").ToArray();
            }
        }

        public void RemoveUsing(string reference)
        {
            Cauldron.Add($"Removing Using: {reference} from {Name}.cs");
            if (Text.Contains("using " + reference + ";"))
            {
                Text = Text.Replace("using " + reference + ";" + Environment.NewLine, "");
                WriteFile();
                UsingsCache = new Librarian(_usingsPattern, Text).Get("capturegroup").ToArray();
            }
        }

        public void AlphabatiseUsings()
        {
            Cauldron.Add($"Alphabatise Usings for {Name}.cs");
            var systemUsings = Usings.Where(@using => new Librarian("(?<!\\.)System\\.*", @using).HasMatch());
            var otherUsings = Usings.Where(@using => !new Librarian("(?<!\\.)System\\.*", @using).HasMatch());
            foreach(var @using in Usings)
            {
                RemoveUsing(@using);
            }
            foreach(var @using in otherUsings.OrderByDescending(u => u))
            {
                AddUsing(@using);
            }
            foreach (var @using in systemUsings.OrderByDescending(u => u))
            {
                AddUsing(@using);
            }
        }
  
        public bool HasEvidenceOf(CsProj csProj)
        {
            throw new NotImplementedException(); //https://github.com/Jordan466/ItsMagic/issues/21

            foreach (var @class in csProj.Classes())
            {
                if (new Librarian("[\\s:]" + @class + "[\\s\\.(]", Text).HasMatch())
                {
                    Cauldron.Add($"Found Evidence of {csProj.Name}.{@class} in {Name}.cs");
                    return true;
                }
            }
            //foreach(var extensionMethod in csProj.ExtensionMethods)
            //{
            //    if (RegexStore.Contains("\\." + extensionMethod + "\\(", Text()))
            //    {
            //        return true;
            //    }
            //}
            return false;
        }

        private string[] GetClasses()
        {
            return new Librarian(_classPattern, Text).Get("capturegroup").ToArray();
        }

        private string[] GetUsings()
        {
            return new Librarian(_usingsPattern, Text).Get("capturegroup").ToArray();
        }

        private string[] GetExtensionMethods()
        {
            return new Librarian(_extensionPattern, Text).Get("capturegroup").ToArray();
        }

        private const string _usingsPattern = "using (?<capturegroup>(.*));";
        private const string _extensionPattern = " (?<capturegroup>(\\w|\\d)*)\\(this";
        private const string _classPattern = "class (?<capturegroup>(\\w*\\d*))";
    }
}