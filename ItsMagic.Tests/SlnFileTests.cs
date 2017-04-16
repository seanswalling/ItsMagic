using Xunit;
using Xunit.Abstractions;

namespace Dumbledore.Tests
{
    public class SlnFileTests : TestsBase
    {
        public SlnFileTests(ITestOutputHelper output)
        {
            Output = output;
        }

        [Fact]
        public void CanDetectEvidenceOfCsProj()
        {
            var slnFile = new SlnFile(SampleSlnFile);
            Assert.Equal(true, slnFile.ContainsProjectReference("57FA7CF2-9479-4C8B-83B4-0C2262A5E6FC"));
            Assert.Equal(true, slnFile.ContainsProjectReference("57FA7CF2-9479-4C8B-83B4-0C2262A5E6FC".ToLower()));
        }
    }
}
