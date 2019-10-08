using Glue.Events;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Tests
{
    [TestClass]
    public class ReturningEventBusTests
    {
        private const int BUS_MESSAGE_INT = 17;
        private const int COUNT_LISTENERS = 23;

        private int countReceived;
        private int intTotalReceived;

        [TestMethod]
        public void TestReturnInts()
        {
            List<int> returns = new List<int>();
            this.countReceived = 0;
            this.intTotalReceived = 0;

            ReturningEventBus<int, int> bus = ReturningEventBus<int, int>.Instance;
            bus.Empty();

            // Event notification list contains several
            for (int listener = 0; listener < COUNT_LISTENERS; listener++)
            {
                bus.ReturningEventRecieved += Bus_ReturningEventRecieved;
            }

            returns = bus.SendEvent(this, BUS_MESSAGE_INT);

            Assert.IsNotNull(returns);
            Assert.AreEqual(COUNT_LISTENERS, returns.Count);
            Assert.AreEqual(COUNT_LISTENERS, this.countReceived);

            int retCount = 0;
            foreach (int retVal in returns)
            {
                Assert.AreEqual(retVal, (retCount + 1) * BUS_MESSAGE_INT);
                retCount++;
            }
        }

        private int Bus_ReturningEventRecieved(object sender, int eventArg)
        {
            this.intTotalReceived += eventArg;
            this.countReceived++;

            return intTotalReceived;
        }
    }
}
