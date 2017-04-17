using System.Linq;
using a;
using b;
using c;
using Xunit;
using Xunit.Abstractions;

namespace ItsMagic.Tests
{
	private class a
	{
		
	}
    public class StepsTests
    {
        private ITestOutputHelper Output { get; }

        public StepsTests(ITestOutputHelper output)
        {
            Output = output;
        }

        [Fact]
        private void CanGetSolutionFiles()
        {
            string[] files = { @"C:\source\Mercury\src\Mercury.Terminal.sln", @"C:\source\Mercury\src\Mercury.TerminalGateway.sln" };
            Output.WriteLine(files.Length.ToString());
            foreach (var file in files)
            {
                Output.WriteLine(file);
            }
            Assert.NotEqual(0,files.Length);
        }

        [Fact]
        private void CanGetCsProjFiles()
        {
            SlnFile[] slnFiles = { new SlnFile(@"C:\source\Mercury\src\Mercury.Terminal.sln"), new SlnFile(@"C:\source\Mercury\src\Mercury.TerminalGateway.sln") };
            foreach (var slnFile in slnFiles)
            {
                var csProjs = slnFile.CsProjs().Distinct().OrderBy(i=>i.Path).ToArray();
                Output.WriteLine(csProjs.Length.ToString());
                foreach (var csProj in csProjs)
                {
                    Output.WriteLine(csProj.Path);
                }
                Assert.NotEqual(0, csProjs.Length);
            }
        }

        [Fact]
        private void CanGetCsFiles()
        {
            SlnFile[] slnFiles = { new SlnFile(@"C:\source\Mercury\src\Mercury.Terminal.sln"), new SlnFile(@"C:\source\Mercury\src\Mercury.TerminalGateway.sln")};
            foreach (var slnFile in slnFiles)
            {
                var csProjs = slnFile.CsProjs();
                
                foreach (var csProj in csProjs)
                {
                    var csFiles = csProj.CsFiles().Distinct().OrderBy(i => i.Path).ToArray();
                    Output.WriteLine(csFiles.Length.ToString());
                    foreach (var csFile in csFiles)
                    {
                        Output.WriteLine(csFile.Path);
                    }
                    Assert.NotEqual(0, csFiles.Length);
                }
            }
        }

        [Fact]
        private void CanFindEvidenceOfWeTc()
        {
            var csFile = @"C:\source\itsmagic\ItsMagic.Tests\Approved\AuthorisationReadModelPopulator.cs";
            Assert.Equal(true, new CsFile(csFile).HasEvidenceOf(new CsProj(@"C:\source\Mercury\src\Platform\WorkerEngine.TestCommon\WorkerEngine.TestCommon.csproj")));
        }

        [Fact]
        private void WontFalselyFindEvidenceOfWeTc()
        {
            var csFile = @"C:\source\ItsMagic\ItsMagic.Tests\Approved\CurrentUserProviderTests.cs";
            Assert.Equal(false, new CsFile(csFile).HasEvidenceOf(new CsProj(@"C:\source\Mercury\src\Platform\WorkerEngine.TestCommon\WorkerEngine.TestCommon.csproj")));
        }

        [Fact]
        private void CanFindWeTcReferenceInCsProj()
        {
            CsProj csProj = new CsProj(@"C:\source\ItsMagic\ItsMagic.Tests\Approved\AuthorisationReadModel.Tests.csproj");
            Assert.Equal(true, csProj.ContainsProjectReference(new CsProj(@"C:\source\Mercury\src\Platform\WorkerEngine.TestCommon\WorkerEngine.TestCommon.csproj")));
        }

        [Fact]
        private void CanGetProjectGuid()
        {
            var csProj = new CsProj(@"C:\source\ItsMagic\ItsMagic.Tests\Approved\Logging.Client.csproj");
            Assert.Equal("42A388A2-B797-4335-8A7D-8D748F58E7A3",csProj.Guid);
        }

        [Fact]
        private void CanGetUsingStatementsFromCsFile()
        {
            string[] expected = { "System", "Mercury.Core", "Mercury.Core.NHibernateExtensions", "NHibernate.Mapping.ByCode", "NHibernate.Mapping.ByCode.Conformist"};
            Assert.Equal(expected, new CsFile(@"E:\github\cc\ItsMagic\ItsMagic.Tests\Approved\AuthFranchise.cs").Usings);
        }
    }
}
