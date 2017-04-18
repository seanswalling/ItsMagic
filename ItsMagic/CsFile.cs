using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Dumbledore
{
    public class CsFile : MagicFile
    {
        private static readonly Dictionary<string, CsFile> CsFilePool = new Dictionary<string, CsFile>();
        public string[] Classes { get; set; }
        public List<string> Usings { get; set; }
        public string[] ExtensionMethods { get; set; }

        private CsFile(string path) : base(path)
        {
            if (Path.GetExtension(Filepath) != ".cs")
                throw new FileFormatException();
				
			Usings = GetUsings().ToList();
            Classes = GetClasses();
            ExtensionMethods = GetExtensionMethods();
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

        public void AddUsing(string reference)
        {
            Cauldron.Add($"Add Using: {reference} to {Name}.cs");
            if (!Usings.Contains(reference))
            {
                Usings.Add(reference);
            }
        }

        public void RemoveUsing(string reference)
        {
            Cauldron.Add($"Removing Using: {reference} from {Name}.cs");
            if (Usings.Contains(reference))
            {
                Usings.RemoveAt(Usings.IndexOf(reference));
            }
        }

        public void UpdateUsingsInText()
        {
            SortUsings();
            Text = new Librarian(UsingBlock).Replace(Text, string.Concat(Usings));
        }

        private void SortUsings()
        {
            Usings.Sort(new UsingComparer());
        }

        private class UsingComparer : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                var isXSystemUsing = IsSystemUsing(x);
                var isYSystemusing = IsSystemUsing(y);

                if (isXSystemUsing && !isYSystemusing)
                {
                    return -1;
                }

                if (!isXSystemUsing && isYSystemusing)
                {
                    return 1;
                }

                return string.CompareOrdinal(x, y);
            }
        }

        [Obsolete]
        public void AlphabatiseUsings()//TODO I don't think this has use anymore, if we want to retain this functionality i suggest trying to embed it into the Removal/Adding process.
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
            return new Librarian(ClassPattern, Text).Get("capturegroup").ToArray();
        }

        private string[] GetUsings()
        {
            return new Librarian(UsingsPattern, Text).Get("capturegroup").ToArray();
        }

        private string[] GetExtensionMethods()
        {
            return new Librarian(ExtensionPattern, Text).Get("capturegroup").ToArray();
        }

        private static bool IsSystemUsing(string reference)
        {
            return new Librarian("(?<!\\.)System\\.*", reference).HasMatch();
        }

        private const string UsingBlock = "(using.+\\n)+";
        private const string UsingsPattern = "using (?<capturegroup>(.*));";
        private const string ExtensionPattern = " (?<capturegroup>(\\w|\\d)*)\\(this";
        private const string ClassPattern = "class (?<capturegroup>(\\w*\\d*))";
    }
}