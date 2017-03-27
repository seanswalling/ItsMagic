using System.Linq;
using Xunit;
using Xunit.Abstractions;
using System.IO;

namespace ItsMagic.Tests
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
            var csFile = getActualCsFile();
            var csProj = getActualCsProjFile();

            Assert.Equal(true, csFile.HasEvidenceOf(csProj));
        }

        [Fact]
        private void CanGetClasses()
        {
            var file = getActualCsFile();
            string[] expected = { "a", "StepsTests" };

            Assert.Equal(expected, file.Classes);
        }

        [Fact]
        private void CanAlphabatiseUsings()
        {
            var file = getActualCsFile();
            file.AlphabatiseUsings();
            string[] expected = { "System.Linq", "a", "b", "c", "Xunit", "Xunit.Abstractions" };

            Assert.Equal(expected, file.Usings);
        }
    }
}
