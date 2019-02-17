using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace GlueTests
{
    public enum AnimalType
    {
        CAT,
        SALMON
    }

    [JsonConverter(typeof(AnimalConverter))]
    public abstract class Animal
    {
        // TODO should be readonly
        [JsonProperty]
        protected String type;
        private String move;

        public string Move
        {
            get => this.move;
            set => this.move=value;
        }
    }

    public class Cat : Animal
    {
        public Cat() : base()
        {
            
            this.type = AnimalType.CAT.ToString();
            this.Move = "Cats run";
        }
    }

    public class Salmon : Animal
    {
        public Salmon()
        {
            this.type = AnimalType.SALMON.ToString();
            this.Move = "Salmon swim";
        }
    }

    public class BaseSpecifiedConcreteClassConverter : DefaultContractResolver
    {
        protected override JsonConverter ResolveContractConverter(Type objectType)
        {
            // Base class specified here
            if (typeof(Animal).IsAssignableFrom(objectType) && !objectType.IsAbstract)
                return null; // pretend TableSortRuleConvert is not specified (thus avoiding a stack overflow)
            return base.ResolveContractConverter(objectType);
        }
    }

    public class AnimalConverter : JsonConverter
    {
        static readonly JsonSerializerSettings SpecifiedSubclassConversion = new JsonSerializerSettings() 
        { 
            ContractResolver = new BaseSpecifiedConcreteClassConverter() 
        };

        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(Animal));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            switch (jo["type"].Value<String>())
            {
                case "CAT":
                    return JsonConvert.DeserializeObject<Cat>(jo.ToString(), SpecifiedSubclassConversion);
                case "SALMON":
                    return JsonConvert.DeserializeObject<Salmon>(jo.ToString(), SpecifiedSubclassConversion);
                default:
                    throw new Exception();
            }
            throw new NotImplementedException();
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException(); // won't be called because CanWrite returns false
        }
    }

    /*
     * These tests are just for screwing around with Json and aren't verifying
     * anything really.
     * 
     */

    [TestClass]
    public class JsonTests
    {
        [TestMethod]
        public void SaveAnimals()
        {
            List<Animal> animals = new List<Animal>
            {
                new Cat(),
                new Salmon()
            };

            JsonSerializer serializer = new JsonSerializer
            {
                NullValueHandling = NullValueHandling.Ignore,
                // TypeNameHandling = TypeNameHandling.All
                TypeNameHandling = TypeNameHandling.Auto
            };

            using (StreamWriter sw = new StreamWriter("animals.json"))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                writer.Formatting = Formatting.Indented;
                serializer.Serialize(writer, animals);
            }
        }

        [TestMethod]
        public void LoadAnimals()
        {
            
            JsonSerializer serializer = new JsonSerializer
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            using (StreamReader sr = new StreamReader("animals.json"))
            {
                using (JsonReader reader = new JsonTextReader(sr))
                {
                    List<Animal> animals = serializer.Deserialize<List<Animal>>(reader);
                    foreach (Animal animal in animals)
                    {
                        Console.WriteLine(animal.Move);
                    }
                }
            }
        }
    }
}
