using System;
using System.IO;
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
            SlnFile[] slnFiles = { new SlnFile(@"C:\source\Mercury\src\Mercury.Terminal.sln"), new SlnFile(@"C:\source\Mercury\src\Mercury.TerminalGateway.sln") };
            foreach (var slnFile in slnFiles)
            {
                var csProjs = slnFile.GetCsProjs().Distinct().OrderBy(i=>i.Path).ToArray();
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
                var csProjs = slnFile.GetCsProjs();
                
                foreach (var csProj in csProjs)
                {
                    var csFiles = csProj.GetCsFiles().Distinct().OrderBy(i => i.Path).ToArray();
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
        private void CanFindEvidenceOfJExt()
        {
            var csFile = @"C:\source\Mercury\src\Common\Contracts.Tests\Scenarios\LoanScenarions.cs";
            Assert.Equal(true, new CsFile(csFile).HasEvidenceOfJExt());
        }

        [Fact]
        private void WillNotFalselyFindEvidenceOfJExt()
        {
            var csFile = @"C:\source\Mercury\src\Plugins\AuthorisationReadModel\AuthFranchise.cs";
            Assert.Equal(false, new CsFile(csFile).HasEvidenceOfJExt());
        }

        [Fact]
        private void CanFindJExtReferenceInCsProj()
        {
            Assert.Equal(true, new CsProj(@"C:\source\ItsMagic\ItsMagic.Tests\Approved\Contracts.Tests.csproj").ContainsJExtProjectReference());
        }

        [Fact]
        private void CanFindJExtReferenceInSln()
        {
            Assert.Equal(true, new SlnFile(@"C:\source\ItsMagic\ItsMagic.Tests\Approved\Mercury.Terminal.sln").ContainsJExtProjectReference());
        }

        [Fact]
        private void CanDetectLogRepoReferences()
        {
            Assert.Equal(true, new CsProj(@"C:\source\Mercury\src\Platform\Logging.Client\Logging.Client.csproj").HasLogRepoReference());
        }

        [Fact]
        private void CanUpdateLogRepoReference()
        {
            var csProj = new CsProj(@"C:\source\Mercury\src\Platform\Logging.Client\Logging.Client.csproj");
            foreach (var reference in csProj.LogRepoReferences())
            {
                csProj.UpdateLogRepoReference(reference);
            }
            
            Assert.Equal(
                File.ReadAllText(@"C:\source\ItsMagic\ItsMagic.Tests\Approved\Logging.Client.csproj"),
                File.ReadAllText(@"C:\source\Mercury\src\Platform\Logging.Client\Logging.Client.csproj"));
        }
    }
}
