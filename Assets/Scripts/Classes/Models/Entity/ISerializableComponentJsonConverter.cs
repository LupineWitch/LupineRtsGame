using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

namespace Assets.Scripts.Classes.Models.Entity
{
    public class ISerializableComponentJsonConverter : JsonConverter<ISerializableEntityComponent>
    {
        public override ISerializableEntityComponent ReadJson(JsonReader reader, Type objectType, ISerializableEntityComponent existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject obj = JObject.Load(reader);
            string typeName = obj[nameof(ISerializableEntityComponent.ComponentsType)].Value<string>();
            Type implementationType = Type.GetType(typeName);
            var tempObj = Activator.CreateInstance(implementationType) ;
            serializer.Populate(obj.CreateReader(), tempObj);
            return tempObj as ISerializableEntityComponent;
        }

        public override void WriteJson(JsonWriter writer, ISerializableEntityComponent value, JsonSerializer serializer)
        {
            // Call the default serialization behavior
            serializer.Serialize(writer, value);
        }

        //JObject jObject = new();
        //var propertiesToSerialise = value.GetType()
        //     .GetProperties()
        //     .Where(p => p.CustomAttributes.Any(a => a.GetType() == typeof(JsonPropertyAttribute)));
        //    foreach(var property in propertiesToSerialise)
        //        jObject.Add(property.Name, property.GetValue(value).ToString());

        //    jObject.WriteTo(writer);
    }
}
