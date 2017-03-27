using System.Linq;
using Xunit;
using Xunit.Abstractions;
using System.IO;

namespace ItsMagic.Tests
{
    public class TestsBase
    {
        public ITestOutputHelper Output { get; set; }
        public static string sampleCsFile = Dumbledore.MagicDir + @"\Samples\StepsTests.cs";
        public static string actualCsFile = Dumbledore.MagicDir + @"\Samples\actual.cs";
        public static string sampleCsProjFile = Dumbledore.MagicDir + @"\Samples\AuthorisationReadModel.csproj";
        public static string actualCsProjFile = Dumbledore.MagicDir + @"\Samples\actual.csproj";

        public CsFile getActualCsFile()
        {
            if (File.Exists(actualCsFile))
                File.Delete(actualCsFile);
            File.Copy(sampleCsFile, actualCsFile);
            return new CsFile(actualCsFile);
        }

        public CsProj getActualCsProjFile()
        {
            if (File.Exists(actualCsProjFile))
                File.Delete(actualCsProjFile);
            File.Copy(sampleCsProjFile, actualCsProjFile);
            return new CsProj(actualCsProjFile);
        }
    }
}
