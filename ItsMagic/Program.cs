using System;
using System.Diagnostics;
using System.Linq;

namespace ItsMagic
{
    class Program
    {
        static void Main()
        {
            Stopwatch sw = Stopwatch.StartNew();

            //Dumbledore.UpdateReadModelConventionsTestReference();
            //Dumbledore.RemoveReferencesToContractsTests();
            //var refs = Dumbledore.ListSolutionsReferencing(new CsProj(Dumbledore.MercurySourceDir + @"\Reporting\IntegrationTests\IntegrationTests.csproj"));
            //foreach (var slnFile in refs)
            //{
            //    Console.WriteLine(slnFile.Name);
            //}

            //var slns =
            //    Dumbledore.GetSolutionFiles(Dumbledore.MercurySourceDir)
            //        .Where(sln => !sln.FilePath.Contains("Platform.sln"));

            //foreach (var slnFile in slns)
            //{
            //    slnFile.RemoveProjectReference("D969683C-AD3A-44E6-9BEA-9AAD7516AEFE");
            //}

            foreach (var slnFile in SlnFile.SolutionsThatReference(new CsProj(@"C:\source\Mercury\src\ApplicantCheckService\CommandProcessor.Tests\CommandProcessor.Tests.csproj")))
            {
                Console.WriteLine(slnFile.Name);
            }

            Console.WriteLine(sw.Elapsed.TotalSeconds);
            Console.WriteLine("Application Complete");
            Console.ReadLine();
        }

        public static void UpdateTestCoreRefs()
        {
            //CsProj[] testCoreReplacements =
            //{
            //    new CsProj(Dumbledore.MercurySourceDir + @"\Platform\Mercury.Testing\Mercury.Testing.csproj"),
            //    new CsProj(Dumbledore.MercurySourceDir + @"\Platform\Mercury.Testing.Factories\Mercury.Testing.Factories.csproj"),
            //    new CsProj(Dumbledore.MercurySourceDir + @"\Platform\Mercury.Testing.Integrated\Mercury.Testing.Integrated.csproj")
            //};
            //Dumbledore.AddTestCoreReplacementsProjectReferences(testCoreReplacements);
        }

        public static void AddProjectRefsToNewSlns()
        {
            //int count = 0;
            //var allProjects =
            //    Dumbledore.GetCsProjFiles(Dumbledore.MercurySourceDir);
            //var allMercurySolution =
            //    Dumbledore
            //        .GetSolutionFiles(Dumbledore.MercurySourceDir)
            //        .Single(sln => sln.FilePath.Contains("AllMercury.sln"));

            //foreach (var project in allProjects)
            //{
            //    count++;
            //    allMercurySolution.AddProjectReference(project, count.ToString());
            //}
            //count = 0;
            //var allTestProjects =
            //    Dumbledore.GetCsProjFiles(Dumbledore.MercurySourceDir).Where(proj => proj.FilePath.Contains(".Test"));
            //var allTestsSolution =
            //    Dumbledore
            //        .GetSolutionFiles(Dumbledore.MercurySourceDir)
            //        .Single(sln => sln.FilePath.Contains("AllTests.sln"));

            //foreach (var testProject in allTestProjects)
            //{
            //    count++;
            //    allTestsSolution.AddProjectReference(testProject, count.ToString());
            //}
        }
    }
}
