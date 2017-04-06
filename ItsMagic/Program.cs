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


            //var allCsProjs = Wand.GetCsProjFiles(Wand.MercurySourceDir);
            //foreach (var csProj in allCsProjs)
            //{
            //    Wand.RepairProjectReferences(csProj);
            //}
            var projs = Wand.GetCsProjFiles(Wand.MercurySourceDir).ToArray();
            foreach (var csProj in projs)
            {
                if (csProj.Text.Contains("<OutputType>Exe</OutputType>"))
                {
                    csProj.Text = csProj.Text.Replace("<Private>False</Private>", "<Private>True</Private>");
                    csProj.WriteFile();
                }
            }
            Console.WriteLine("Finished doing actual work");
            Helper.FindInvalidCsprojReferences();
            Helper.FindMissingPackConfigEntries();
            


            //var projs = Wand.GetCsProjFiles(Wand.MercurySourceDir);
            //Console.WriteLine("count = " + projs.Count());
            //IEnumerable<CsProj[]> deplists = projs.Select(p => p.References);
            //foreach (var deplist in deplists)
            //{

            //}
            //.Select(rl => rl.Count()).Average();
            //Console.WriteLine(avdepcount);

            //var messageHubWebRole = new CsProj(@"C:\source\Mercury\src\MessageHub\MessageHub.MessageHubWebRole\WebRole.csproj");
            //var allMessageHubWebRoleReferencesIncludingChildren = Wand.GetAllProjectReferences(messageHubWebRole);
            //Console.WriteLine(Environment.NewLine);
            //foreach (var reference in allMessageHubWebRoleReferencesIncludingChildren)
            //{
            //    Console.WriteLine(reference.Name);
            //}

            Console.WriteLine(sw.Elapsed.TotalSeconds);
            Console.WriteLine("Application Complete");
            Console.ReadLine();
        }
       
    }

    internal static class Helper
    {
        public static void FindMissingPackConfigEntries()
        {
            CsProj[] csProjs = Wand.GetCsProjFiles(Wand.MercurySourceDir).ToArray();
            foreach (var csProj in csProjs)
            {
                var missingCsProjNugetReferences = RegexStore.Get(RegexStore.NugetReferenceFromCsProjPattern, csProj.Text)
                    .Where(token => token.Contains("\\packages\\"))
                    .Where(csProjNugetReference =>
                    {
                        var nugetId = RegexStore
                            .Get(RegexStore.NugetIdFromNugetReference, csProjNugetReference)
                            .Single()
                            .TrimEnd('.');


                        var pc = csProj.PackagesConfig();
                        var nr = pc
                            .NugetpackageReferences;
                        return !nr
                            .Any(entry => string.Equals(entry.Id, nugetId, StringComparison.InvariantCultureIgnoreCase));
                    }).ToArray();
                if (missingCsProjNugetReferences.Length > 0)
                {
                    Console.WriteLine($"{csProj} is missing package.config entires for ");
                    foreach (var reference in missingCsProjNugetReferences)
                    {
                        Console.WriteLine($"    {reference}");
                    }
                }
            }
        }

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
                .Select(relPath => ToAbsolutePath(relPath, Directory.GetParent(path).FullName))
                .Where(absPath => !File.Exists(absPath))
                .ToArray();
        }

        private static Group Match(string line)
        {
            var regex = new Regex(RegexStore.CsProjPathFromCsProjPattern);
            return regex.Match(line).Groups["capturegroup"];
        }

        public static string ToAbsolutePath(string relPath, string path)
        {
            var abspath = Path.Combine(path, relPath);
            var result = Path.GetFullPath(abspath);
            return result.Replace(@"c:\source\mercury\Templates\", @"c:\source\mercury\src\");
        }
    }

    
}
