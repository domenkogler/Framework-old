using Kogler.Framework.NLog;
using NLog;
using Shouldly;
using Xunit;

namespace Kogler.Framework.Test
{
    public class NLogTests
    {
        [Fact]
        public void TestProxyName()
        {
            var logger = NLogProxy.GetCaliburnMicroILogFromType(GetType());
            ((Logger)logger).Name.ShouldNotBeNull();
        }
    }
}