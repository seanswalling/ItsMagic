using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ItsMagic
{
    public class CsFile : MagicFile
    {
        private string[] ClassesCache { get; set; }
        public string[] Classes => ClassesCache ?? (ClassesCache = RegexStore.Get(RegexStore.ClassFromCsFilePattern, Text).ToArray());
        private string[] UsingsCache { get; set; }
        public string[] Usings => UsingsCache ?? (UsingsCache = RegexStore.Get(RegexStore.UsingsFromCsFilePattern, Text).ToArray());
        private string[] ExtensionMethodsCache { get; set; }
        public string[] ExtensionMethods => ExtensionMethodsCache ?? (ExtensionMethodsCache =
                                                RegexStore.Get(RegexStore.ExtensionMethodsFromCsFilePattern, Text).ToArray());

        public CsFile(string path)
        {
            FilePath = path;
        }

        public void AddUsing(string reference)
        {
            Cauldron.Add($"Add Using: {reference} to {Name}.cs");
            if (!Text.Contains("using " + reference + ";"))
            {
                var text = "using " + reference + ";" + Environment.NewLine + Text;
                WriteFile();
                UsingsCache = RegexStore.Get(RegexStore.UsingsFromCsFilePattern, Text).ToArray();
            }
        }

        public void RemoveUsing(string reference)
        {
            Cauldron.Add($"Removing Using: {reference} from {Name}.cs");
            if (Text.Contains("using " + reference + ";"))
            {
                var text = Text.Replace("using " + reference + ";" + Environment.NewLine, "");
                WriteFile();
                UsingsCache = RegexStore.Get(RegexStore.UsingsFromCsFilePattern, Text).ToArray();
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
            foreach (var @class in csProj.Classes)
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
    }
}