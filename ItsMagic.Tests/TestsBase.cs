using System.Linq;
using Xunit;
using Xunit.Abstractions;
using System.IO;

namespace Dumbledore.Tests
{
    public class TestsBase
    {
        public ITestOutputHelper Output { get; set; }
        public static string sampleCsFile = Wand.MagicDir + @"\Samples\StepsTests.cs";
        public static string actualCsFile = Wand.MagicDir + @"\Samples\actual.cs";
        public static string sampleCsProjFile = Wand.MagicDir + @"\Samples\AuthorisationReadModel.csproj";
        public static string actualCsProjFile = Wand.MagicDir + @"\Samples\actual.csproj";
        public static string sampleSlnFile = Wand.MagicDir + @"\Samples\Mercury.Terminal.sln";
        public static string actualSlnFile = Wand.MagicDir + @"\Samples\actual.sln";

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

        public CsProj getActualSlnFile()
        {
            if (File.Exists(actualSlnFile))
                File.Delete(actualSlnFile);
            File.Copy(sampleSlnFile, actualSlnFile);
            return new CsProj(actualSlnFile);
        }
    }
}
