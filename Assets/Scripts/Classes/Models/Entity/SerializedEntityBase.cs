using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Classes.Models.Entity
{
    public class SerializedEntityBase
    {
        [JsonProperty]
        public Vector3 Position { get; set; }
        [JsonProperty]
        public List<ISerializableEntityComponent> Components { get; set; }
        [JsonProperty]
        public string PrefabName { get; set; } = string.Empty;
        [JsonProperty]
        public string ParentName { get; set; } = "Ambient";
        [JsonProperty]
        public string FactionName { get; set; } = "Neutral";

        public SerializedEntityBase()
        {
            Components = new List<ISerializableEntityComponent>();
        }

        public override string ToString() => $"Prefab:{(string.IsNullOrEmpty(PrefabName) ? "<Empty prefab name>" : PrefabName)};{ParentName}:{Position}";
    }
}
