using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Glue
{
    class Trigger
    {
        private readonly Keys triggerKey;
        private List<Keys> modKeys = new List<Keys>();
        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public Keys TriggerKey => triggerKey;

        public Trigger(Keys triggerKey)
        {
            this.triggerKey = triggerKey;
        }

        public void AddModifier(Keys modKey)
        {
            modKeys.Add(modKey);
        }

        public bool AreModKeysActive()
        {
            bool modKeysAreActive = true;

            foreach(Keys key in modKeys)
            {
                if (!Keyboard.IsKeyDown(key))
                {
                    modKeysAreActive = false;
                    break;
                }
            }

            return modKeysAreActive;
        }

        internal void Fire()
        {
            String message = "TRIGGER FIRED: triggerKey=" + triggerKey;

            if (modKeys.Count > 0)
            {
                message += " modKeys={";

                int modCount = 0;
                foreach (Keys key in modKeys)
                {
                    message += key;
                    modCount++;
                    if (modCount < modKeys.Count)
                    {
                        message += ", ";
                    }
                }
                message += "}";
            }

            LOGGER.Info(message);

            ActionQueue.Enqueue(new Action(ActionTypes.KEYBOARD_PRESS, 0, Keys.Z));
        }
    }
}
