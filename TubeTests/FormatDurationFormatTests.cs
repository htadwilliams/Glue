using Glue.PropertyIO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlueTests
{
    [TestClass]
    public class FormatDurationFormatTests
    {
        [TestMethod]
        public void TestZero()
        {
            Assert.AreEqual("0ms", FormatDuration.Format(0));
        }

        [TestMethod]
        public void TestOneMS()
        {
            Assert.AreEqual("1ms", FormatDuration.Format(1));
        }

        [TestMethod]
        public void TestOneSecond()
        {
            Assert.AreEqual("1s", FormatDuration.Format(1000));
        }

        [TestMethod]
        public void TestOneOfEverything()
        {
            Assert.AreEqual("1h 1m 1s 1ms", FormatDuration.Format(3661001));
        }

        [TestMethod]
        public void TestOneOfEverythingButMinutes()
        {
            Assert.AreEqual("1h 1s 1ms", FormatDuration.Format(3601001));
        }
    }
}
