using Assets.Scripts.Classes.GameData;
using Assets.Scripts.Classes.Models.Entity;
using Newtonsoft.Json;
using System;
using UnityEngine;

namespace Assets.Scripts.Objects.ResourceNodes
{
    public class ResourceNodeTree : ResourceNodeBase
    {
        [JsonProperty]
        public override Type ComponentsType { get => typeof(ResourceNodeTree); }
    }
}
