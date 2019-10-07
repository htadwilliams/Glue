using Glue.Triggers;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Glue 
{
    internal class TriggerMap : Dictionary<Keys, List<TriggerKeyboard>>
    {
        public TriggerMap(int initialCount) : base(initialCount)
        {
        }

        public TriggerMap() : base()
        {
        }

        public void Add(TriggerKeyboard trigger)
        {
            if (!TryGetValue(trigger.TriggerKey, out List<TriggerKeyboard> triggerList))
            {
                triggerList = new List<TriggerKeyboard>();
                Add(trigger.TriggerKey, triggerList);
            }
            triggerList.Add(trigger);
        }
    }
}
