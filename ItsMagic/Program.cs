using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace ItsMagic
{
    class Program
    {
        static void Main()
        {
            string[] solutionFiles = { @"C:\source\Mercury\src\Mercury.Terminal.sln", @"C:\source\Mercury\src\Mercury.TerminalGateway.sln" };
            foreach (var solutionFile in solutionFiles)
            {
                var csProjs = SlnFile.GetCsProjs(solutionFile).ToArray();
                foreach (var csProj in csProjs)
                {
                    var csFiles = CsProj.GetCsFiles(csProj).ToArray();
                    foreach (var csFile in csFiles)
                    {
                        if (CsFile.HasEvidenceOfJExt(csFile))
                        {
                            CsFile.AddUsingToCsFile(csFile, "Mercury.Core.JsonExtensions");
                            if (!CsProj.ContainsJExtProjectReference(csProj))
                            {
                                CsProj.AddJExtProjectReference(csProj);
                            }

                            //check to see if solution has JExt project reference
                            //if it doesn't
                            if (!SlnFile.ContainsJExtProjectReference(solutionFile))
                            {
                                //Add Project Reference
                                SlnFile.AddJExtProjectReference(solutionFile);
                            }
                        }
                        //if Cs File contains Evidence of NHibExt
                        //...
                    }
                }
            }
        }













        //private void awdada()
        //{
        //    Dumbledore.AddMissingReferencesTo(Dumbledore.GetFiles(@"C:\source\Mercury\src\Common\Contracts.Tests\Scenarios", "cs"));
        //}

        //private void testingStuff()
        //{
        //    var getStuff = Dumbledore.GetFiles(@"C:\source\Mercury\src", "sln")//.SelectMany(Dumbledore.GetCsProjs);
        //    //var UsingStatements = Dumbledore.GetFiles(@"E:\Users\illus\Leisure\C#", "sln")
        //        .SelectMany(SlnFile.GetCsProjs)
        //        .SelectMany(CsProj.GetCsFiles)
        //        .SelectMany(CsFile.Usings)
        //        .Distinct()
        //        .OrderBy(i => i);

        //    var solutions = Dumbledore.GetFiles(@"C:\source\Mercury\src", "sln").ToArray().Distinct();
        //    var csProjs = solutions.SelectMany(SlnFile.GetCsProjs).ToArray().Distinct();
        //    var csFiles = csProjs.SelectMany(CsProj.GetCsFiles).ToArray().Distinct();
        //    var usings = csFiles.SelectMany(CsFile.Usings).ToArray().Distinct();
            
        //    foreach (string stuff in usings)
        //    {
        //        Console.WriteLine(stuff);
        //    }
        //    Console.WriteLine("\nComplete");
        //    Console.ReadLine();
        //}
    }
}
