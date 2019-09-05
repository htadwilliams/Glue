using Glue;
using Glue.Actions;
using Glue.Native;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace Tests
{
    [TestClass]
    public class MacroTests
    {
        public class TestTimeProvider : TimeProvider
        {
            public long TimeNow { get; set; } = 0L;

            public override long Now()
            {
                return TimeNow;
            }
        }

        public class ActionTest : Action
        {
            public ActionTest(long timeDelayMS) : base(timeDelayMS)
            {
            }

            public override void Play()
            {
            }

            public override Action[] Schedule(long timeScheduled)
            {
                return new Action[] 
                {
                    new ActionTest(this.delayMS)
                    {
                        ScheduledTick = timeScheduled + DelayMS
                    }
                };
            }
        }

        public class TestScheduler : IActionScheduler
        {
            public TestScheduler()
            {
            }

            public List<Action> ScheduledList { get; set; } = new List<Action>();

            public void Cancel(string name)
            {
                throw new System.NotImplementedException();
            }

            public void Schedule(Action action)
            {
                ScheduledList.Add(action);
            }

            public void SubscribeQueueChange(OnQueueChange changeHandler)
            {
                throw new System.NotImplementedException();
            }

            public void UnsubscribeScheduleChange(OnQueueChange changeHandler)
            {
                throw new System.NotImplementedException();
            }
        }

        [TestMethod]
        public void TestScheduleActions()
        {
            /// TODO Use a mocking framework for test classes
            TestScheduler scheduler = new TestScheduler();

            TestTimeProvider timeProvider = new TestTimeProvider();

            // Inject test scheduler instead of default (which is probably ActionQueueThread)
            Macro.Scheduler = scheduler;
            Macro.Time = timeProvider;

            Macro macro = new Macro("testMacro", 1000) // Fire 1s 1ms after triggered
                .AddAction(new ActionTest(100))
                .AddAction(new ActionTest(100))
                ;

            macro.ScheduleActions();

            Assert.AreEqual(2, scheduler.ScheduledList.Count);
            Assert.AreEqual(1100, scheduler.ScheduledList[0].ScheduledTick);
            Assert.AreEqual(1200, scheduler.ScheduledList[1].ScheduledTick);
        }
    }
}
