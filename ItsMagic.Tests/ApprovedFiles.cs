using System.IO;
using Xunit;

namespace ItsMagic.Tests
{
    public class ApprovedFiles
    {
        [Fact]
        public void AssessmentScenariosCsContainsJExtUsing()
        {
            var actual = File.ReadAllText(@"C:\source\Mercury\src\Common\Contracts.Tests\Scenarios\LoanScenarions.cs");
            var approved = File.ReadAllText(@"C:\source\ItsMagic\ItsMagic.Tests\Approved\AssessmentScenarios.cs");
            Assert.Equal(actual,approved);
        }

        [Fact]
        public void AuthFranchiseCsContainsNHibExtUsing()
        {
            var actual = File.ReadAllText(@"C:\source\Mercury\src\Plugins\AuthorisationReadModel\AuthFranchise.cs");
            var approved = File.ReadAllText(@"C:\source\ItsMagic\ItsMagic.Tests\Approved\AuthFranchise.cs");
            Assert.Equal(actual, approved);
        }

        [Fact]
        public void ContractsTestsCsProjContainsJExtReference()
        {
            var actual = File.ReadAllText(@"C:\source\Mercury\src\Common\Contracts.Tests\Contracts.Tests.csproj");
            var approved = File.ReadAllText(@"C:\source\ItsMagic\ItsMagic.Tests\Approved\Contracts.Tests.csproj");
            Assert.Equal(actual, approved);
        }

        [Fact]
        public void AuthorisationReadModelCsProjContainsNHibExt()
        {
            var actual = File.ReadAllText(@"C:\source\Mercury\src\Plugins\AuthorisationReadModel\AuthorisationReadModel.csproj");
            var approved = File.ReadAllText(@"C:\source\ItsMagic\ItsMagic.Tests\Approved\AuthorisationReadModel.csproj");
            Assert.Equal(actual, approved);
        }

        [Fact]
        public void TerminalSolutionContainsJExtReference()
        {
            var actual = File.ReadAllText(@"C:\source\Mercury\src\Mercury.Terminal.sln");
            var approved = File.ReadAllText(@"C:\source\ItsMagic\ItsMagic.Tests\Approved\Mercury.Terminal.sln");
            Assert.Equal(actual, approved);
        }

        [Fact]
        public void TerminalGatewaySolutionContainsNHibExtReference()
        {
            var actual = File.ReadAllText(@"C:\source\Mercury\src\Mercury.TerminalGateway.sln");
            var approved = File.ReadAllText(@"C:\source\ItsMagic\ItsMagic.Tests\Approved\Mercury.TerminalGateway.sln");
            Assert.Equal(actual, approved);
        }
    }
}
