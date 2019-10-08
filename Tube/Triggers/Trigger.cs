using Glue.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using static Glue.Events.Event;

namespace Glue.Triggers
{

    [JsonObject(MemberSerialization.OptIn)]
    public abstract class Trigger
    {
        public bool EatInput => this.eatInput;
        public List<string> MacroNames => macroNames;
        public ButtonStates ButtonState => this.buttonState;

        //
        // Using privates for JSonProperty results in JSon files with lower case names
        //

        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        private readonly ButtonStates buttonState;

        [JsonProperty]
        private readonly List<string> macroNames = new List<string>();
        
        [JsonProperty]
        private readonly bool eatInput;

        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        protected int indexMacroCurrent = 0;

        [JsonConstructor]
        public Trigger(ButtonStates buttonState, List<string> macroNames, bool eatInput)
        {
            this.buttonState = buttonState;
            this.macroNames.AddRange(macroNames);
            this.eatInput = eatInput;
        }

        public Trigger(ButtonStates buttonState, string macroName, bool eatInput)
        {
            this.buttonState = buttonState;
            this.macroNames.Add(macroName);
            this.eatInput = eatInput;
        }

        protected virtual void Fire()
        {
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
        }
    }
}
