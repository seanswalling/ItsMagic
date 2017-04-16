using Xunit.Abstractions;
using System.IO;

namespace Dumbledore.Tests
{
    public class TestsBase
    {
        protected ITestOutputHelper Output { get; set; }

        protected static string SampleCsFile = Wand.MagicDir + @"\Samples\StepsTests.cs";
        protected static string ActualCsFile = Wand.MagicDir + @"\Samples\actual.cs";

        protected static string SampleCsProjFile = Wand.MagicDir + @"\Samples\StepsTests.csproj";
        protected static string ActualCsProjFile = Wand.MagicDir + @"\Samples\actual.csproj";

        protected static string SampleSlnFile = Wand.MagicDir + @"\Samples\StepsTests.sln";
        protected static string ActualSlnFile = Wand.MagicDir + @"\Samples\actual.sln";

        public CsFile GetActualCsFile()
        {
            if (File.Exists(ActualCsFile))
                File.Delete(ActualCsFile);
            File.Copy(SampleCsFile, ActualCsFile);
            return new CsFile(ActualCsFile);
        }

        public CsProj GetActualCsProjFile()
        {
            if (File.Exists(ActualCsProjFile))
                File.Delete(ActualCsProjFile);
            File.Copy(SampleCsProjFile, ActualCsProjFile);
            return CsProj.Get(ActualCsProjFile);
        }

        public SlnFile GetActualSlnFile()
        {
            if (File.Exists(ActualSlnFile))
                File.Delete(ActualSlnFile);
            File.Copy(SampleSlnFile, ActualSlnFile);
            return new SlnFile(ActualSlnFile);
        }
    }
}
