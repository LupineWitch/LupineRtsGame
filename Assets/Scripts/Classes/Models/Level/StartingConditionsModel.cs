using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Classes.Models.Level
{
    public class StartingConditionsModel
    {
        [JsonProperty]
        public string OrginalStartingFactionPrefabName;
        [JsonProperty]
        public Dictionary<string, int> StartingResources = new();
        [JsonProperty]
        public string StartingBuildingPrefabName = null;
        [JsonProperty]
        public Vector3 StartingPosition;
    }
}
