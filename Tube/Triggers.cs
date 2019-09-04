using System.Collections.Generic;
using System.Windows.Forms;

namespace Glue 
{
    class TriggerMap : Dictionary<Keys, List<Trigger>>
    {
        public TriggerMap(int initialCount) : base(initialCount)
        {
        }

        public TriggerMap() : base()
        {
        }

        public void Add(Trigger trigger)
        {
            if (!TryGetValue(trigger.TriggerKey, out List<Trigger> triggerList))
            {
                triggerList = new List<Trigger>();
                Add(trigger.TriggerKey, triggerList);
            }
            triggerList.Add(trigger);
        }
    }
}
