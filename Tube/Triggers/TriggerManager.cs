using Glue.Events;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Glue.Triggers
{
    public class TriggerManager
    {
        // private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private Dictionary<Keys, List<TriggerKeyboard>> KeyboardTriggers { get; } = new Dictionary<Keys, List<TriggerKeyboard>>();
        private List<Trigger> Triggers { get; } = new List<Trigger>();

        public TriggerManager()
        {
        }

        public void Clear()
        {
            KeyboardTriggers.Clear();
            Triggers.Clear();
        }

        public void Add(Trigger trigger)
        {
            if (trigger.GetType() == typeof(TriggerKeyboard))
            {
                Add((TriggerKeyboard) trigger);
            }
            else 
            {
                Triggers.Add(trigger);
            }
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

        public bool OnKeyboard(int vkCode, ButtonStates movement)
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

        internal List<Trigger> GetTriggers()
        {
            List<Trigger> triggerList = new List<Trigger>(Triggers);

            foreach (List<TriggerKeyboard> keyboardTriggerList in KeyboardTriggers.Values)
            {
                foreach (Trigger keyboardTrigger in keyboardTriggerList)
                {
                    triggerList.Add(keyboardTrigger);
                }
            }
            
            return triggerList;
        }

        internal void AddTriggers(List<Trigger> triggers)
        {
            foreach (Trigger trigger in triggers)
            {
                Add(trigger);
            }
        }
    }
}
