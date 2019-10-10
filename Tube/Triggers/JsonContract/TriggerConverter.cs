using Glue.Triggers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

/// <summary>
/// 
/// Allows list in Json file to contain any Trigger type. 
/// 
/// The ReadJson() method is a factory that creates and deserializes the 
/// correct Trigger type given the "type" attribute of the Json node.
/// 
/// </summary>
namespace Glue.Triggers.JsonContract
{
    internal class TriggerConverter : JsonConverter
    {
        private static readonly log4net.ILog LOGGER = 
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static readonly JsonSerializerSettings SpecifiedSubclassConversion = new JsonSerializerSettings() 
        { 
            ContractResolver = new TriggerContractResolver() 
        };

        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(Trigger));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            string typeString = jo["type"].Value<string>();

            try
            {
                TriggerType type = (TriggerType) Enum.Parse(typeof(TriggerType), typeString, true);

                switch (type)
                {
                    case TriggerType.Keyboard:
                        return JsonConvert.DeserializeObject<TriggerKeyboard>(jo.ToString(), SpecifiedSubclassConversion);
                    case TriggerType.ControllerButton:
                        return JsonConvert.DeserializeObject<TriggerControllerButton>(jo.ToString(), SpecifiedSubclassConversion);
                    case TriggerType.ControllerPOV:
                        return JsonConvert.DeserializeObject<TriggerControllerPOV>(jo.ToString(), SpecifiedSubclassConversion);

                    default:
                        string message = "Unknown type [" + type + "] encountered during deserialization";
                        LOGGER.Warn(message);
                        return null;
                }
            }
            catch (Exception e)
            {
                LOGGER.Error(e);
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
