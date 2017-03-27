using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace ItsMagic.Tests
{
    public class CsFileTests
    {
        private ITestOutputHelper Output { get; }

        public CsFileTests(ITestOutputHelper output)
        {
            Output = output;
        }

        [Fact]
        private void CanGetClasses()
        {
            Assert.Equal(true,true);
        }
    }
}
