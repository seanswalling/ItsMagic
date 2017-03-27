using System.Linq;
using Xunit;
using Xunit.Abstractions;
using System.IO;

namespace ItsMagic.Tests
{
    public class CsFileTests
    {
        private ITestOutputHelper Output { get; }
        private static string sampleCsFile = Dumbledore.MagicDir + @"\Samples\StepsTests.cs";
        private static string actualCsFile = Dumbledore.MagicDir + @"\Samples\actual.cs";
        private static string sampleCsProjFile = Dumbledore.MagicDir + @"\Samples\AuthorisationReadModel.csproj";
        private static string actualCsProjFile = Dumbledore.MagicDir + @"\Samples\actual.csproj";

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

        private CsFile getActualCsFile()
        {
            if (File.Exists(actualCsFile))
                File.Delete(actualCsFile);
            File.Copy(sampleCsFile, actualCsFile);
            return new CsFile(actualCsFile);
        }

        private CsProj getActualCsProjFile()
        {
            if(File.Exists(actualCsProjFile))
                File.Delete(actualCsProjFile);
            File.Copy(sampleCsProjFile, actualCsProjFile);
            return new CsProj(actualCsProjFile);
        }
    }
}
