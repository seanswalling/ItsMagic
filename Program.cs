using System;
using System.Linq;

namespace ItsMagic
{
    class Program
    {
        static void Main()
        {
            var getStuff = Dumbledore.GetFiles(@"C:\source\Mercury\src","sln")//.SelectMany(Dumbledore.GetCsProjs);
            //var UsingStatements = Dumbledore.GetFiles(@"E:\Users\illus\Leisure\C#", "sln")
                .SelectMany(SlnFile.GetCsProjs)
                .SelectMany(CsProj.GetCsFiles)
                .SelectMany(CsFile.Usings)
                .Distinct()
                .OrderBy(i => i);
            foreach(string stuff in getStuff)
            {
                Console.WriteLine(stuff);
            }
            Console.WriteLine("\nComplete");
            Console.ReadLine();
        }
    }
}
