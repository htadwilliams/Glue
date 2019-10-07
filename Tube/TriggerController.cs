using Glue.Event;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Glue
{
    [JsonObject(MemberSerialization.OptIn)]
    public class TriggerController : Trigger
    {
        [JsonProperty]
        private readonly string namePattern;
        [JsonProperty]
        private readonly int button;

        public string NamePattern => namePattern;
        public int Button => button;

        [JsonConstructor]
        public TriggerController(
            ButtonStates buttonState, 
            List<string> macroNames, 
            bool eatInput, 
            int button, 
            string namePattern) 
                
            : base(buttonState, macroNames, eatInput)
        {
            this.button = button;
            this.namePattern = namePattern;
        }
    }
}
