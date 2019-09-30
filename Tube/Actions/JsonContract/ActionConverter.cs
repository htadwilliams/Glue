using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

/// <summary>
/// 
/// Allows list in Json file to contain any action type. 
/// 
/// The ReadJson() method is a factory
/// that creates and deserializes the correct Action type given the "type" attribute 
/// of the Json node.
/// 
/// </summary>
namespace Glue.Actions.JsonContract
{
    internal class ActionConverter : JsonConverter
    {
        private static readonly log4net.ILog LOGGER = 
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static readonly JsonSerializerSettings SpecifiedSubclassConversion = new JsonSerializerSettings() 
        { 
            ContractResolver = new ActionContractResolver() 
        };

        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(Action));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            string typeString = jo["type"].Value<string>();

            ActionType type = (ActionType) Enum.Parse(typeof(ActionType), typeString, true);

            switch (type)
            {
                // TODO Type constants should be defined in one place, and possibly use an enum if the contract will support it
                case ActionType.KEY:
                    return JsonConvert.DeserializeObject<ActionKey>(jo.ToString(), SpecifiedSubclassConversion);
                case ActionType.TYPING:
                    return JsonConvert.DeserializeObject<ActionTyping>(jo.ToString(), SpecifiedSubclassConversion);
                case ActionType.SOUND:
                    return JsonConvert.DeserializeObject<ActionSound>(jo.ToString(), SpecifiedSubclassConversion);
                case ActionType.MOUSE:
                    return JsonConvert.DeserializeObject<ActionMouse>(jo.ToString(), SpecifiedSubclassConversion);
                case ActionType.REPEAT:
                    return JsonConvert.DeserializeObject<ActionRepeat>(jo.ToString(), SpecifiedSubclassConversion);
                case ActionType.CANCEL:
                    return JsonConvert.DeserializeObject<ActionCancel>(jo.ToString(), SpecifiedSubclassConversion);
                case ActionType.MOUSE_LOCK:
                    return JsonConvert.DeserializeObject<ActionMouseLock>(jo.ToString(), SpecifiedSubclassConversion);

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
}
