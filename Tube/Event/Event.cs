using Glue.Native;
using System.Threading;

namespace Glue.Event
{
    public enum ButtonStates
    {
        Release = 0,
        Press = 1,
        Both = 3,
    }

    public abstract class Event
    {
        private static int s_nextEventId;

        public long TimeCreated { get; }
        public int EventId { get; }

        public Event()
        {
            EventId = CreateEventId();
            TimeCreated = TimeProvider.GetTickCount();
        }
        
        // 
        // Assuming 100 events a second, a UINT for id will give 2.7 years 
        // of operation before repeating.
        // 
        // Switching to ULONG would make that 5.8 million years if need be.
        // 
        // Might be nicer to make this un-signed, but Interlocked.Increment
        // doesn't have an overload for that.
        // 
        private static int CreateEventId()
        {
            return Interlocked.Increment(ref s_nextEventId);
        }
    }
}
