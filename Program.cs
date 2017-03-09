using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace ItsMagic
{
    class Program
    {
        static void Main()
        {
            Dumbledore.AddMissingReferencesTo(CsProj.GetCsFiles(@"C:\source\Mercury\src\Terminal\Terminal\Terminal.csproj"));
        }

        private void testingStuff()
        {
            var getStuff = Dumbledore.GetFiles(@"C:\source\Mercury\src", "sln")//.SelectMany(Dumbledore.GetCsProjs);
            //var UsingStatements = Dumbledore.GetFiles(@"E:\Users\illus\Leisure\C#", "sln")
                .SelectMany(SlnFile.GetCsProjs)
                .SelectMany(CsProj.GetCsFiles)
                .SelectMany(CsFile.Usings)
                .Distinct()
                .OrderBy(i => i);

            var solutions = Dumbledore.GetFiles(@"C:\source\Mercury\src", "sln").ToArray().Distinct();
            var csProjs = solutions.SelectMany(SlnFile.GetCsProjs).ToArray().Distinct();
            var csFiles = csProjs.SelectMany(CsProj.GetCsFiles).ToArray().Distinct();
            var usings = csFiles.SelectMany(CsFile.Usings).ToArray().Distinct();
            
            foreach (string stuff in usings)
            {
                Console.WriteLine(stuff);
            }
            Console.WriteLine("\nComplete");
            Console.ReadLine();
        }
    }
}
