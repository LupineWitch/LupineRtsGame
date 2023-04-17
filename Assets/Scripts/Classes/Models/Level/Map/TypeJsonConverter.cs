using System;
using Newtonsoft.Json;
using System.Reflection;

namespace Assets.Scripts.Classes.Models.Level.Map
{
    public class TypeJsonConverter : JsonConverter<Type>
    {
        public override Type ReadJson(JsonReader reader, Type objectType, Type existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            string s = reader.Value as string;
            Type foundType = Assembly.GetExecutingAssembly().GetType(s);

            if (foundType == null)
                foundType = SearchForType(s);

            return foundType;
        }

        public override void WriteJson(JsonWriter writer, Type value, JsonSerializer serializer)
        {
            writer.WriteValue(value.FullName);
        }

        private static Type SearchForType(string TypeName)
        {

            // Try Type.GetType() first. This will work with types defined
            // by the Mono runtime, etc.
            var type = Type.GetType(TypeName);

            // If it worked, then we're done here
            if (type != null)
                return type;

            // Get the name of the assembly (Assumption is that we are using
            // fully-qualified type names)
            var assemblyName = TypeName.Substring(0, TypeName.IndexOf('.'));

            // Attempt to load the indicated Assembly
            var assembly = Assembly.Load(assemblyName);
            if (assembly == null)
                return null;

            // Ask that assembly to return the proper Type
            return assembly.GetType(TypeName);

        }
    }
}
