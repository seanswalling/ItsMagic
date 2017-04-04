using Xunit;
using Xunit.Abstractions;

namespace Dumbledore.Tests
{
    public class CsFileTests : TestsBase
    {
        public CsFileTests(ITestOutputHelper output)
        {
            Output = output;
        }

        [Fact]
        private void CanDetectEvidenceOfCsProj()
        {
            var csFile = GetActualCsFile();
            var csProj = GetActualCsProjFile();

            Assert.Equal(true, csFile.HasEvidenceOf(csProj));
        }

        [Fact]
        private void CanGetClasses()
        {
            var file = GetActualCsFile();
            string[] expected = { "a", "StepsTests" };

            Assert.Equal(expected, file.Classes);
        }

        [Fact]
        private void CanAlphabatiseUsings()
        {
            var file = GetActualCsFile();
            file.AlphabatiseUsings();
            string[] expected = { "System.Linq", "a", "b", "c", "Xunit", "Xunit.Abstractions" };

            Assert.Equal(expected, file.Usings);
        }
    }
}
