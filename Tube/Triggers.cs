using System.Collections.Generic;
using System.Windows.Forms;

namespace Glue 
{
    internal class TriggerMap : Dictionary<Keys, List<KeyboardTrigger>>
    {
        public TriggerMap(int initialCount) : base(initialCount)
        {
        }

        public TriggerMap() : base()
        {
        }

        public void Add(KeyboardTrigger trigger)
        {
            if (!TryGetValue(trigger.TriggerKey, out List<KeyboardTrigger> triggerList))
            {
                triggerList = new List<KeyboardTrigger>();
                Add(trigger.TriggerKey, triggerList);
            }
            triggerList.Add(trigger);
        }
    }
}
