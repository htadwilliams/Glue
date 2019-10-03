using Glue;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class EventBusTests
    {
        private const int BUS_MESSAGE_INT = 17;

        private int intReceived;
        private object objectReceived;

        [TestMethod]
        public void TestInstance()
        {
            EventBus<int> busInt1 = EventBus<int>.Instance;
            EventBus<int> busInt2 = EventBus<int>.Instance;
            EventBus<string> busString = EventBus<string>.Instance;

            Assert.IsNotNull(busInt1);
            Assert.IsNotNull(busInt2);
            Assert.IsNotNull(busString);

            Assert.AreSame(busInt1, busInt2);

            // Falls under category of "duh" but does make intent crystal clear
            Assert.AreNotSame(busInt2, busString);
        }

        [TestMethod]
        public void TestInt()
        {

            EventBus<int> bus = EventBus<int>.Instance;

            bus.Empty();
            bus.EventRecieved += Bus_EventRecievedInt;

            // Guaranteed not to be BUS_MESSAGE_INT
            this.intReceived = BUS_MESSAGE_INT + 1;

            int countListeners = bus.SendEvent(this, BUS_MESSAGE_INT);

            Assert.AreEqual(this.intReceived, BUS_MESSAGE_INT);
            Assert.AreEqual(countListeners, 1);
        }

        [TestMethod]
        public void TestObject()
        {
            EventBus<object> bus = EventBus<object>.Instance;

            bus.Empty();
            bus.EventRecieved += Bus_EventRecievedObject;

            this.objectReceived = null;
            object objectSent = new object();

            bus.SendEvent(this, objectSent);

            Assert.AreSame(objectSent, this.objectReceived);
        }

        [TestMethod]
        public void TestSendNoSubscribers()
        {
            EventBus<int> bus = EventBus<int>.Instance;

            bus.Empty();

            int countListeners = bus.SendEvent(this, 23);

            Assert.AreEqual(0, countListeners, "Nobody is listening");
        }

        [TestMethod]
        public void TestUnsubscribed()
        {
            EventBus<int> bus = EventBus<int>.Instance;

            // Should be same as bus.Empty();
            bus.Empty();
            bus.EventRecieved += Bus_EventRecievedInt;
            bus.EventRecieved -= Bus_EventRecievedInt;

            // Guaranteed not to be BUS_MESSAGE_INT
            this.intReceived = BUS_MESSAGE_INT + 1;

            bus.SendEvent(this, BUS_MESSAGE_INT);

            // Still shouldn't be BUS_MESSAGE_INT
            Assert.AreNotEqual(BUS_MESSAGE_INT, this.intReceived);
        }

        [TestMethod]
        public void TestCanSendNull()
        {
            EventBus<object> bus = EventBus<object>.Instance;

            bus.Empty();
            bus.EventRecieved += Bus_EventRecievedObject;

            bus.SendEvent(this, null);

            // Yes it can send and recieve a null
            Assert.AreSame(null, this.objectReceived);
        }

        [TestMethod]
        public void TestEmptyOne()
        {
            EventBus<int> bus = EventBus<int>.Instance;

            bus.Empty();
            bus.EventRecieved += Bus_EventRecievedInt;

            int subscribersDumped = bus.Empty();

            Assert.AreEqual(1, subscribersDumped);
        }

        [TestMethod]
        public void TestEmptyEmpty()
        {
            EventBus<int> bus = EventBus<int>.Instance;

            // Assumes there are no other threads subscribing
            bus.Empty();
            int subscribersDumped = bus.Empty();

            Assert.AreEqual(0, subscribersDumped);
        }

        private void Bus_EventRecievedInt(object sender, BusEventArgs<int> e)
        {
            this.intReceived = e.BusEvent;
        }

        private void Bus_EventRecievedObject(object sender, BusEventArgs<object> e)
        {
            this.objectReceived = e.BusEvent;
        }
    }
}
