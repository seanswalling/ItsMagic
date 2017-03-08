using System;
using System.Linq;

namespace ItsMagic
{
    class Program
    {
        static void Main()
        {
            //var csFiles = Dumbledore.GetSlnFiles(@"C:\source\Mercury\src").SelectMany(Dumbledore.GetCsProjs);
            var UsingStatements = Dumbledore.GetSlnFiles(@"E:\Users\illus\Leisure\C#")
                .SelectMany(SlnFile.GetCsProjs)
                .SelectMany(CsProj.GetCsFiles)
                .SelectMany(CsFile.Usings)
                .Distinct()
                .OrderBy(i => i);
            foreach(string statement in UsingStatements)
            {
                Console.WriteLine(statement);
            }
            Console.ReadLine();
        }
    }
}
