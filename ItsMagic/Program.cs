using System;
using System.Diagnostics;

namespace ItsMagic
{
    class Program
    {
        static void Main()
        {
            Stopwatch sw = Stopwatch.StartNew();
            string dir = "C:\\source\\Mercury\\src";
            //string dir = @"E:\github\cc\Mercury\src";
            
            CsProj[] testCoreReplacements =
            {
                new CsProj(@"C:\source\Mercury\src\Platform\Mercury.Testing\Mercury.Testing.csproj"),
                new CsProj(@"C:\source\Mercury\src\Platform\Mercury.Testing.Factories\Mercury.Testing.Factories.csproj"),
                new CsProj(@"C:\source\Mercury\src\Platform\Mercury.Testing.Integrated\Mercury.Testing.Integrated.csproj")
            };
            Dumbledore.AddTestCoreReplacementsProjectReferences(dir, testCoreReplacements);

            Console.WriteLine(sw.Elapsed.TotalSeconds);
            Console.WriteLine("Application Complete");
            Console.ReadLine();
        }
    }
}
