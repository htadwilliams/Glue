using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Glue.Actions
{
    // TODO Move Json adapter class BaseSpecifiedConcreteClassConverter into its own file
    internal class BaseSpecifiedConcreteClassConverter : DefaultContractResolver
    {
        protected override JsonConverter ResolveContractConverter(Type objectType)
        {
            if (typeof(Action).IsAssignableFrom(objectType) && !objectType.IsAbstract)
                return null; // pretend TableSortRuleConvert is not specified (thus avoiding a stack overflow)
            return base.ResolveContractConverter(objectType);
        }
    }

    // TODO Move Json adapter class ActionConverter into its own file
    internal class ActionConverter : JsonConverter
    {
        private static readonly log4net.ILog LOGGER = 
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static readonly JsonSerializerSettings SpecifiedSubclassConversion = new JsonSerializerSettings() 
        { 
            ContractResolver = new BaseSpecifiedConcreteClassConverter() 
        };

        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(Action));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            string type = jo["type"].Value<string>();
            switch (type)
            {
                case "KEY":
                    return JsonConvert.DeserializeObject<ActionKey>(jo.ToString(), SpecifiedSubclassConversion);
                case "TYPING":
                    return JsonConvert.DeserializeObject<ActionTyping>(jo.ToString(), SpecifiedSubclassConversion);
                case "SOUND":
                    return JsonConvert.DeserializeObject<ActionSound>(jo.ToString(), SpecifiedSubclassConversion);
                case "MOUSE":
                    return JsonConvert.DeserializeObject<ActionMouse>(jo.ToString(), SpecifiedSubclassConversion);
                case "REPEAT":
                    return JsonConvert.DeserializeObject<ActionRepeat>(jo.ToString(), SpecifiedSubclassConversion);
                case "CANCEL":
                    return JsonConvert.DeserializeObject<ActionCancel>(jo.ToString(), SpecifiedSubclassConversion);

                default:
                    string message = "Unknown type [" + type + "] encountered during deserialization";
                    LOGGER.Warn(message);
                    return null;
            }
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            // won't be called because CanWrite returns false
            throw new NotImplementedException(); 
        }
    }

    [JsonConverter(typeof(ActionConverter))]
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class Action
    {
        public long TimeTriggerMS
        {
            get => this.timeTriggerMS;
            set => this.timeTriggerMS = value;
        }

        public string Type
        {
            get => this.type;
            set => this.type=value;
        }

        public long TimeScheduledMS
        {
            get => this.timeScheduledMS;
            set => this.timeScheduledMS = value;
        }
        
        public string Name
        {
            get => this.name;
            set => this.name = value;
        }

        [JsonProperty]
        protected string type;

        [JsonProperty]
        // Used to schedule action relative to triggering event
        protected long timeTriggerMS;

        // Name of scheduled instance - used to cancel actions in queue
        protected string name;

        // Time scheduled for this action instance 
        protected long timeScheduledMS;

        public abstract void Play();

        // Actions may schedule multiple instances - see ActionTyping
        public abstract Action[] Schedule();

        public override string ToString()
        {
            return type;
        }
    }
}
