using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace ItsMagic.Tests
{
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
            string[] slnFiles = { @"C:\source\Mercury\src\Mercury.Terminal.sln", @"C:\source\Mercury\src\Mercury.TerminalGateway.sln" };
            foreach (var slnFile in slnFiles)
            {
                var csProjs = SlnFile.GetCsProjs(slnFile).Distinct().OrderBy(i=>i).ToArray();
                Output.WriteLine(csProjs.Length.ToString());
                foreach (var csProj in csProjs)
                {
                    Output.WriteLine(csProj);
                }
                Assert.NotEqual(0, csProjs.Length);
            }
        }

        [Fact]
        private void CanGetCsFiles()
        {
            string[] slnFiles = { @"C:\source\Mercury\src\Mercury.Terminal.sln", @"C:\source\Mercury\src\Mercury.TerminalGateway.sln" };
            foreach (var slnFile in slnFiles)
            {
                var csProjs = SlnFile.GetCsProjs(slnFile);
                
                foreach (var csProj in csProjs)
                {
                    var csFiles = CsProj.GetCsFiles(csProj).Distinct().OrderBy(i => i).ToArray();
                    Output.WriteLine(csFiles.Length.ToString());
                    foreach (var csFile in csFiles)
                    {
                        Output.WriteLine(csFile);
                    }
                    Assert.NotEqual(0, csFiles.Length);
                }
            }
        }

        [Fact]
        private void CanFindEvidenceOfJExt()
        {
            var csFile = @"C:\source\Mercury\src\Common\Contracts.Tests\Scenarios\LoanScenarions.cs";
            Assert.Equal(true, CsFile.HasEvidenceOfJExt(csFile));
        }

        [Fact]
        private void WillNotFalselyFindEvidenceOfJExt()
        {
            var csFile = @"C:\source\Mercury\src\Plugins\AuthorisationReadModel\AuthFranchise.cs";
            Assert.Equal(false, CsFile.HasEvidenceOfJExt(csFile));
        }

        [Fact]
        private void CanFindJExtReferenceInCsProj()
        {
            Assert.Equal(true, CsProj.ContainsJExtProjectReference(@"C:\source\ItsMagic\ItsMagic.Tests\Approved\Contracts.Tests.csproj"));
        }

        [Fact]
        private void CanFindJExtReferenceInSln()
        {
            Assert.Equal(true, SlnFile.ContainsJExtProjectReference(@"C:\source\ItsMagic\ItsMagic.Tests\Approved\Mercury.Terminal.sln"));
        }
    }
}
