using Glue.Events;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Glue.Triggers
{
    public class TriggerManager
    {
        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // TODO unify trigger list and stop defeating polymorphism
        private Dictionary<Keys, List<TriggerKeyboard>> KeyboardTriggers { get; } = new Dictionary<Keys, List<TriggerKeyboard>>();
        private List<TriggerController> ControllerTriggers { get; } = new List<TriggerController>();

        public TriggerManager()
        {
            EventBus<EventController>.Instance.EventRecieved += OnEventController;
        }

        private void OnEventController(object sender, BusEventArgs<EventController> e)
        {
            foreach(TriggerController trigger in ControllerTriggers)
            {
                trigger.CheckAndFire(e.BusEvent);
            }
        }

        public void Clear()
        {
            KeyboardTriggers.Clear();
            ControllerTriggers.Clear();
        }

        public void Add(TriggerController trigger)
        {
            ControllerTriggers.Add(trigger);
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

        internal List<Trigger> GetTriggers()
        {
            List<Trigger> triggerList = new List<Trigger>(ControllerTriggers);

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
                if (trigger.GetType() == typeof(TriggerKeyboard))
                {
                    Add((TriggerKeyboard) trigger);
                }
                else 
                {
                    try
                    {
                        Add((TriggerController) trigger);
                    }
                    catch (InvalidCastException e)
                    {
                        LOGGER.Error("Unknown Trigger type: " + trigger.GetType(), e);
                    }
                }
            }
        }
    }
}
