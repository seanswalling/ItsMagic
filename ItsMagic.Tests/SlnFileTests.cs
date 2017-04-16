using Xunit;
using Xunit.Abstractions;
using System.IO;
using System.Linq;

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

        [Fact]
        public void CanRemoveProjectReferences()
        {
            var slnFile = GetActualSlnFile();
            slnFile.RemoveProjectReference("57FA7CF2-9479-4C8B-83B4-0C2262A5E6FC");
            Assert.Equal(false, slnFile.Text.Contains("57FA7CF2-9479-4C8B-83B4-0C2262A5E6FC"));
        }

        [Fact]
        public void CanAddProjectReference()
        {
            var slnFile = GetActualSlnFile();
            var csProj = CsProj.Get(SampleCsProjFile);
            slnFile.AddProjectReference(csProj);
            Assert.Equal(true, slnFile.ContainsProjectReference(csProj));
        }

        [Fact]
        public void CanRepairWhiteSpace()
        {
            var slnFile = GetActualSlnFile();
            slnFile.RepairWhiteSpace();

            Assert.Equal(false, new Librarian("(\\t*\\r\\n){2,}", slnFile.Text).HasMatch());
        }
    }
}
