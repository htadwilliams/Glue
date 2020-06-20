using Glue.Native;
using Glue.Triggers.JsonContract;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace Glue.Triggers
{
    public enum TriggerType
    {
        Keyboard,
        MouseWheel,
        ControllerButton,
        ControllerPOV,
        ControllerAxis,
     }

    [JsonObject(MemberSerialization.OptIn)]
    [JsonConverter(typeof(TriggerConverter))]
    public abstract class Trigger : IDisposable
    {
        public bool EatInput => this.eatInput;
        public List<string> MacroNames => macroNames;
        protected TriggerType Type { get => type; set => type = value; }
        public string ProcessName { get => processName; set => processName = value; }
        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // Index into ripple fire macros
        protected int indexMacroCurrent = 0;

        //
        // Using privates for JSonProperty results in JSon files with lower case names
        //
        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        private TriggerType type;

        [JsonProperty]
        private readonly List<string> macroNames = new List<string>();
        
        [JsonProperty]
        private readonly bool eatInput;

        [JsonProperty]
        private string processName;

        public Trigger(List<string> macroNames, bool eatInput)
        {
            this.macroNames.AddRange(macroNames);
            this.eatInput = eatInput;

            SubscribeEvent();
        }

        public Trigger(string macroName, bool eatInput)
        {
            this.macroNames.Add(macroName);
            this.eatInput = eatInput;

            SubscribeEvent();
        }

        protected virtual void SubscribeEvent()
        {
            // Do nothing by default - subclasses must opt in
        }

        protected virtual void UnsubscribeEvent()
        {
        }

        private bool IsTargetProcessFocused()
        {
            // Process name is optional - if unset trigger is global
            if (ProcessName == null || ProcessName.Length == 0)
            {
                return true;
            }

            string inputFocusProcessName = ProcessInfo.GetProcessFileName(
                ProcessInfo.GetInputFocusProcessId());

            if (!inputFocusProcessName
                .ToLower()
                .Contains(ProcessName.ToLower())
                )
            {
                LOGGER.Debug(
                    "Trigger target process mismatch: " +
                    "focus process = [" + inputFocusProcessName + "] " +
                    "trigger process = [" + ProcessName + "]");

                return false;
            }

            return true;
        }

        protected virtual bool Fire(int macroIndex)
        {
            if (!IsTargetProcessFocused())
            {
                return false;
            }

            string macroName = MacroNames[macroIndex];

            if (null != macroName)
            {
                Tube.PlayMacro(macroName);
            }

            return EatInput;
        }

        protected virtual bool Fire()
        {
            if (!IsTargetProcessFocused())
            {
                return false;
            }

            if (this.indexMacroCurrent >= MacroNames.Count)
            {
                indexMacroCurrent = 0;
            }

            string macroName = MacroNames[this.indexMacroCurrent];

            // null macro names are allowed in list - useful for key toggles or other 
            // no-op entries in macro "ripple" effect 
            if (null != macroName)
            {
                Tube.PlayMacro(macroName);
            }
            this.indexMacroCurrent++;

            return EatInput;
        }

        public void Dispose()
        {
            UnsubscribeEvent();
        }
    }
}
