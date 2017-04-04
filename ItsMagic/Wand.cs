﻿using System;
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
                if (Directory.Exists(dir1))
                    return dir1;
                if (Directory.Exists(dir2))
                    return dir1;
                throw new DirectoryNotFoundException();
            }
        }
        public static string MagicDir => new DirectoryInfo(Environment.CurrentDirectory).Parent.Parent.FullName;
        public static string PackagesDir => MercurySourceDir + "\\packages";

        public static void UpdateAllReferencesToNugetReferences()
        {
            SlnFile[] solutionFiles = Directory.EnumerateFiles(MercurySourceDir, "*.sln", SearchOption.AllDirectories)
                .Select(file => new SlnFile(file))
                .ToArray();
            //{ @"C:\source\Mercury\src\Mercury.ReflectiveTests.sln" };
            foreach (var solutionFile in solutionFiles)
            {
                foreach (var csProj in solutionFile.CsProjs())
                {
                    foreach (var csFile in csProj.CsFiles())
                    {

                    }
                }
            }
        }

        public static void AddProjectReferences(CsProj projectToAdd)
        {
            foreach (var solutionFile in GetSolutionFiles(MercurySourceDir).ToArray())
            {
                foreach (var csProj in solutionFile.CsProjs().ToArray())
                {
                    foreach (var csFile in csProj.CsFiles())
                    {
                        if (csFile.HasEvidenceOf(projectToAdd))
                        {
                            Cauldron.Add($"Adding References too {csFile.Name}, {csProj.Name} and {solutionFile.Name}");
                            AddReferences(csFile, csProj, solutionFile, projectToAdd);
                        }
                    }
                }
            }
        }

        private static void AddReferences(CsFile csFile, CsProj csProj, SlnFile slnFile, CsProj projectToAdd)
        {
            csFile.AddUsing(projectToAdd.Name);
            if (!csProj.ContainsProjectReference(projectToAdd))
            {
                csProj.AddProjectReference(projectToAdd);
            }
            if (!slnFile.ContainsProjectReference(projectToAdd))
            {
                slnFile.AddProjectReference(projectToAdd, "Common");
            }
        }

        public static IEnumerable<string> ReadLines(string file)
        {
            if (File.Exists(file))
            {
                using (var reader = new StreamReader(file))
                {
                    while (!reader.EndOfStream)
                    {
                        yield return reader.ReadLine();
                    }
                }
            }
        }

        public static IEnumerable<SlnFile> GetSolutionFiles(string dir)
        {
            return Directory.EnumerateFiles(dir, "*.sln", SearchOption.AllDirectories)
                .Where(File.Exists)
                .Select(file => new SlnFile(file));
        }

        public static IEnumerable<CsProj> GetCsProjFiles(string dir)
        {
            return Directory.EnumerateFiles(dir, "*.csproj", SearchOption.AllDirectories)
                .Select(file => new CsProj(file));
        }

        public static IEnumerable<CsFile> GetCsFiles(string dir)
        {
            return Directory.EnumerateFiles(dir, "*.cs", SearchOption.AllDirectories)
                .Select(file => new CsFile(file));
        }

        public static void UpdateReadModelConventionsTestReference()
        {
            var pluginProjs = GetCsProjFiles(MercurySourceDir + @"\Plugins");
            var pattern = "\\.\\.\\\\\\.\\.\\\\Platform\\\\Tests\\.Core\\\\ReadModelConventionsTest\\.cs\\\">(\\s+)<Link>ReadModelConventionsTest\\.cs<\\/Link>(\\s+)<\\/Compile>";
            var replacement = "ReadModelConventionsTest.cs\" />";

            foreach (var pluginProj in pluginProjs)
            {
                if (pluginProj.Text.Contains(@"Platform\Tests.Core\ReadModelConventionsTest.cs"))
                {
                    Regex reg = new Regex(pattern);
                    pluginProj.Text = reg.Replace(pluginProj.Text, replacement);
                    pluginProj.WriteFile();

                    var readModelConventionsTest = MagicDir + @"\ReadModelConventionsTest\ReadModelConventionsTest.cs";
                    var pluginProjDir = Path.GetDirectoryName(pluginProj.FilePath) + @"\ReadModelConventionsTest.cs";
                    File.Copy(readModelConventionsTest, pluginProjDir);
                }
            }
        }

        public static void RemoveReferencesToContractsTests()
        {
            var csProjs = GetCsProjFiles(MercurySourceDir);
            foreach (var csProj in csProjs)
            {
                csProj.RemoveProjectReference(new CsProj(@"C:\source\Mercury\src\Platform\Contracts.Tests\Contracts.Tests.csproj").Guid);
            }
        }

        public static SlnFile[] ListSolutionsReferencing(CsProj csProj)
        {
            return GetSolutionFiles(MercurySourceDir).Where(sln => sln.ContainsProjectReference(csProj.Guid)).ToArray();
        }
    }
}