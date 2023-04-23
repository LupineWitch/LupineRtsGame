using Newtonsoft.Json;
using System;

namespace Assets.Scripts.Classes.Models.Entity
{
    public interface ISerializableEntityComponent
    {
        [JsonProperty]
        public Type ComponentsType { get; }
    }
}