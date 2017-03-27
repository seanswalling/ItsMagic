using System;
using System.Diagnostics;

namespace ItsMagic
{
    class Program
    {
        static void Main()
        {
            Stopwatch sw = Stopwatch.StartNew();

            //CsProj[] testCoreReplacements =
            //{
            //    new CsProj(Dumbledore.MercurySourceDir + @"\Platform\Mercury.Testing\Mercury.Testing.csproj"),
            //    new CsProj(Dumbledore.MercurySourceDir + @"\Platform\Mercury.Testing.Factories\Mercury.Testing.Factories.csproj"),
            //    new CsProj(Dumbledore.MercurySourceDir + @"\Platform\Mercury.Testing.Integrated\Mercury.Testing.Integrated.csproj")
            //};
            //Dumbledore.AddTestCoreReplacementsProjectReferences(testCoreReplacements);


            Console.WriteLine(sw.Elapsed.TotalSeconds);
            Console.WriteLine("Application Complete");
            Console.ReadLine();
        }
    }
}
