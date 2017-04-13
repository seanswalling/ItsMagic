using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Dumbledore
{
    public static class Wand
    {
        public static string MercurySourceDir
        {
            get
            {
                string dir1 = @"C:\source\Mercury\src";
                string dir2 = @"E:\github\cc\Mercury\src";
                string dir3 = @"C:\Mercury\src";
                if (Directory.Exists(dir1))
                    return dir1;
                if (Directory.Exists(dir2))
                    return dir2;
                if (Directory.Exists(dir3))
                    return dir3;
                throw new DirectoryNotFoundException();
            }
        }
        public static string MagicDir => new DirectoryInfo(Environment.CurrentDirectory).Parent.Parent.FullName;
        public static string PackagesDir => MercurySourceDir + "\\packages";

        public static IEnumerable<SlnFile> GetSolutionFiles(string dir)
        {
            return Directory.EnumerateFiles(dir, "*.sln", SearchOption.AllDirectories)
                .Where(File.Exists)
                .Select(file => SlnFile.Get(file));
        }

        public static IEnumerable<CsProj> GetCsProjFiles(string dir)
        {
            return Directory.EnumerateFiles(dir, "*.csproj", SearchOption.AllDirectories)
                .Select(file => CsProj.Get(file));
        }

        public static IEnumerable<CsFile> GetCsFiles(string dir)
        {
            return Directory.EnumerateFiles(dir, "*.cs", SearchOption.AllDirectories)
                .Select(file => CsFile.Get(file));
        }

        public static SlnFile[] GetSolutionsReferencing(CsProj csProj)
        {
            return GetSolutionFiles(MercurySourceDir).Where(sln => sln.ContainsProjectReference(csProj.Guid)).ToArray();
        }

        private static HashSet<string> RepairedProjects = new HashSet<string>();
        public static void RepairProjectReferences(CsProj csProj)
        {
            if (RepairedProjects.Contains(csProj.Filepath))
                return;
            
            foreach (var csProjReference in csProj.References)
            {
                RepairProjectReferences(csProjReference);
            }

            foreach (var csProjDependency in CsProj.GetAllProjectReferences(csProj))
            {
                csProj.AddProjectReference(csProjDependency);
            }
            
            foreach (var nugetDependency in CsProj.GetNugetReferences(csProj))
            {
                csProj.AddNugetReference(nugetDependency);
            }

            RepairedProjects.Add(csProj.Filepath);
        }

        public static string ToAbsolutePath(string relPath, string path)
        {
            var abspath = Path.Combine(path, relPath);
            var result = Path.GetFullPath(abspath);
            return result.Replace(@"c:\source\mercury\Templates\", @"c:\source\mercury\src\");
        }

        private const string NugetReferenceFromCsProjPattern = "(?<capturegroup>(<Reference Include=.*\\s+(\\s*<SpecificVersion>.*\\s*)*<HintPath>.+<\\/HintPath>\\s+.+\\s+<\\/Reference>))";
        private const string NugetIdFromNugetReference = "packages\\\\(?<capturegroup>[\\w\\.-]+?)\\.\\d";
        public static void FindMissingPackConfigEntries()
        {
            foreach (var csProj in Wand.GetCsProjFiles(Wand.MercurySourceDir).ToArray())
            {
                var missingCsProjNugetReferences = new Librarian(NugetReferenceFromCsProjPattern, csProj.Text)
                    .Get("capturegroup")
                    .Where(token => token.Contains("\\packages\\"))
                    .Where(csProjNugetReference =>
                    {
                        var nugetId = new Librarian(NugetIdFromNugetReference, csProjNugetReference)
                            .Get("capturegroup")
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

    }

    public static class HashEx
    {
        public static void AddRange<T>(this HashSet<T> target, IEnumerable<T> newItems)
        {
            foreach (var newItem in newItems)
            {
                target.Add(newItem);
            }
        }
    }
}
