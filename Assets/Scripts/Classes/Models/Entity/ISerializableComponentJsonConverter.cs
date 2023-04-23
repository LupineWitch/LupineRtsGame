using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            throw new NotImplementedException("This converter cannot be used for serialisation");
        }
    }
}
