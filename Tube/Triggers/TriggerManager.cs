using Glue.Event;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Glue.Triggers
{
    public class TriggerManager
    {
        public Dictionary<Keys, List<TriggerKeyboard>> KeyboardTriggers { get; } = new Dictionary<Keys, List<TriggerKeyboard>>();

        public void Clear()
        {
            KeyboardTriggers.Clear();
        }

        public void Add(TriggerKeyboard trigger)
        {
            if (!KeyboardTriggers.TryGetValue(trigger.TriggerKey, out List<TriggerKeyboard> triggerList))
            {
                triggerList = new List<TriggerKeyboard>();
                KeyboardTriggers.Add(trigger.TriggerKey, triggerList);
            }
            triggerList.Add(trigger);
        }

        internal void AddTriggers(List<TriggerKeyboard> triggers)
        {
            foreach (TriggerKeyboard trigger in triggers)
            {
                Add(trigger);
            }
        }

        public bool CheckAndFireTriggers(int vkCode, ButtonStates movement)
        {
            bool eatInput = false;

            // Triggers fire macros 
            if (KeyboardTriggers!= null &&
                KeyboardTriggers.TryGetValue((Keys) vkCode, out List<TriggerKeyboard> triggers))
            {
                foreach (TriggerKeyboard trigger in triggers)
                {
                    switch (trigger.ButtonState)
                    {
                        case ButtonStates.Both:
                            eatInput |= trigger.CheckAndFire();
                        break;

                        case ButtonStates.Press:
                        if (ButtonStates.Press == movement)
                        {
                            eatInput |= trigger.CheckAndFire();
                        }
                        break;

                        case ButtonStates.Release:
                        if (ButtonStates.Release == movement)
                        {
                            eatInput |= trigger.CheckAndFire();
                        }
                        break;
                    }
                }

            }

            return eatInput;
        }
    }
}
