using Glue.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlueTests
{
    [TestClass]
    public class FormatDurationStringFromMillisTests
    {
        [TestMethod]
        public void TestZero()
        {
            Assert.AreEqual("0ms", FormatDuration.StringFromMillis(0));
        }

        [TestMethod]
        public void TestOneMS()
        {
            Assert.AreEqual("1ms", FormatDuration.StringFromMillis(1));
        }

        [TestMethod]
        public void TestOneSecond()
        {
            Assert.AreEqual("1s", FormatDuration.StringFromMillis(1000));
        }

        [TestMethod]
        public void TestOneOfEverything()
        {
            Assert.AreEqual("1h 1m 1s 1ms", FormatDuration.StringFromMillis(3661001));
        }

        [TestMethod]
        public void TestOneOfEverythingButMinutes()
        {
            Assert.AreEqual("1h 1s 1ms", FormatDuration.StringFromMillis(3601001));
        }
    }
}
