﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Glue.Util;

namespace GlueTests
{
    [TestClass]
    public class FormatDurationMillisFromStringTests
    {
        [TestMethod]
        public void ParseGarbage()
        {
            Assert.AreEqual(0, FormatDuration.MillisFromString("Garbage in, zero out.  I just can't take it anymore!"));
        }

        [TestMethod]
        public void ParseEmpty()
        {
            Assert.AreEqual(0, FormatDuration.MillisFromString(""));
        }

        [TestMethod]
        public void ParseOneSecond()
        {
            Assert.AreEqual(1000, FormatDuration.MillisFromString("1s"));
        }

        [TestMethod]
        public void ParseSomeMillis()
        {
            Assert.AreEqual(23, FormatDuration.MillisFromString("23ms"));
        }

        [TestMethod]
        public void ParseWithoutSpaces()
        {
            Assert.AreEqual(1017, FormatDuration.MillisFromString("1s17ms"));
        }

        [TestMethod]
        public void ParseWithSpaces()
        {
            Assert.AreEqual(1017, FormatDuration.MillisFromString("1s 17ms"));
        }

        [TestMethod]
        public void ParseMixedCase()
        {
            Assert.AreEqual(1017, FormatDuration.MillisFromString("1s 17MS"));
        }

        [TestMethod]
        public void ParseHappyPathWithEverything()
        {
            Assert.AreEqual(83862666, FormatDuration.MillisFromString("23H 17m 42s666MS"));
        }

        [TestMethod]
        public void ParseWithEmptyMinutes()
        {
            Assert.AreEqual(82817666, FormatDuration.MillisFromString("23H 17S 666MS"));
        }

        [TestMethod]
        public void ParseOutOfOrder()
        {
            // Random order is not supported
            Assert.AreEqual(0, FormatDuration.MillisFromString("17S 23H 666MS"));
        }

        [TestMethod]
        public void ParseFraction()
        {
            // Fractions are NYI
            Assert.AreEqual(0, FormatDuration.MillisFromString("1.5s"));
        }
    }
}
