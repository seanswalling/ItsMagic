using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Dumbledore
{
    public class CsFile : MagicFile
    {
        private string[] ClassesCache { get; set; }
        public string[] Classes => ClassesCache ?? (ClassesCache = RegexStore.Get(_classPattern, Text).ToArray());
        private string[] UsingsCache { get; set; }
        public string[] Usings => UsingsCache ?? (UsingsCache = RegexStore.Get(_usingsPattern, Text).ToArray());
        private string[] ExtensionMethodsCache { get; set; }
        public string[] ExtensionMethods => ExtensionMethodsCache ?? (ExtensionMethodsCache =
                                                RegexStore.Get(_extensionPattern, Text).ToArray());

        public string[] textAsLines { get; set; }
        public CsFile(string path) : base(path)
        {
                
        }

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
                UsingsCache = RegexStore.Get(_usingsPattern, Text).ToArray();
            }
        }

        public void RemoveUsing(string reference)
        {
            Cauldron.Add($"Removing Using: {reference} from {Name}.cs");
            if (Text.Contains("using " + reference + ";"))
            {
                Text = Text.Replace("using " + reference + ";" + Environment.NewLine, "");
                WriteFile();
                UsingsCache = RegexStore.Get(_usingsPattern, Text).ToArray();
            }
        }

        public void AlphabatiseUsings()
        {
            Cauldron.Add($"Alphabatise Usings for {Name}.cs");
            var systemUsings = Usings.Where(@using => RegexStore.Contains("(?<!\\.)System\\.*", @using));
            var otherUsings = Usings.Where(@using => !RegexStore.Contains("(?<!\\.)System\\.*", @using));
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
            foreach (var @class in csProj.Classes())
            {
                if (RegexStore.Contains("[\\s:]" + @class + "[\\s\\.(]", Text))
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

        private const string _usingsPattern = "using (?<capturegroup>(.*));";
        private const string _extensionPattern = " (?<capturegroup>(\\w|\\d)*)\\(this";
        private const string _classPattern = "class (?<capturegroup>(\\w*\\d*))";
    }
}