using System.IO;
using Xunit;

namespace ItsMagic.Tests
{
    public class ApprovedFiles
    {
        [Fact]
        public void AssessmentScenariosCsContainsJExtUsing()
        {
            Assert.True(File
                .ReadAllText(@"C:\source\Mercury\src\Common\Contracts.Tests\Scenarios\AssessmentScenarios.cs")
                .Contains("using Mercury.Core.JsonExtensions;"));
        }

        [Fact]
        public void AuthFranchiseCsContainsNHibExtUsing()
        {
            Assert.True(File
                .ReadAllText(@"C:\source\Mercury\src\Plugins\AuthorisationReadModel\AuthFranchise.cs")
                .Contains("using Mercury.Core.NHibernateExtensions;"));
        }

        [Fact]
        public void ContractsTestsCsProjContainsJExtReference()
        {
            Assert.True(
                new CsProj(@"C:\source\Mercury\src\Common\Contracts.Tests\Contracts.Tests.csproj")
                .ContainsJExtProjectReference());
        }

        [Fact]
        public void AuthorisationReadModelCsProjContainsNHibExt()
        {
            Assert.True(
                new CsProj(@"C:\source\Mercury\src\Plugins\AuthorisationReadModel\AuthorisationReadModel.csproj")
                .ContainsNHibExtProjectReference());
        }

        [Fact]
        public void TerminalSolutionContainsJExtReference()
        {
            Assert.True(
                new SlnFile(@"C:\source\Mercury\src\Mercury.Terminal.sln")
                .ContainsJExtProjectReference());
        }

        [Fact]
        public void TerminalGatewaySolutionContainsNHibExtReference()
        {
            Assert.True(new SlnFile(@"C:\source\Mercury\src\Mercury.TerminalGateway.sln")
                .ContainsNHibExtProjectReference());
        }
    }
}
