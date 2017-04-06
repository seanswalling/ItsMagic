using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Dumbledore
{
    class Program
    {
        static void Main()
        {
            //TODO transform this whole app into a class lib and make a cli then make a gui.
            var sw = Stopwatch.StartNew();
            Helper.FindInvalidCsprojReferences();
            Wand.FindMissingPackConfigEntries();
            
            Console.WriteLine($"\nApplication Complete: {sw.Elapsed.TotalSeconds}");
            Console.ReadLine();
        }
       
    }

    internal static class Helper
    {

        public static void FindInvalidCsprojReferences()
        {
            var dic = Directory.EnumerateFiles(@"c:\source\mercury", "*.csproj", SearchOption.AllDirectories)
                .ToDictionary(path => path, GetAllBogusReferences)
                .Where(kvp => kvp.Value.Any());

            foreach (var keyValuePair in dic)
            {
                Console.WriteLine(keyValuePair.Key);
                foreach (var refs in keyValuePair.Value)
                {
                    Console.WriteLine($"  {refs}");
                }
            }
        }

        private static string[] GetAllBogusReferences(string path)
        {
            return File.ReadLines(path)
                .Select(Match)
                .Where(m => m.Success)
                .Select(m => m.Value)
                .Select(relPath => Wand.ToAbsolutePath(relPath, Directory.GetParent(path).FullName))
                .Where(absPath => !File.Exists(absPath))
                .ToArray();
        }

        private static Group Match(string line)
        {
            var regex = new Regex(RegexStore.CsProjPathFromCsProjPattern);
            return regex.Match(line).Groups["capturegroup"];
        }


    }

    
}
