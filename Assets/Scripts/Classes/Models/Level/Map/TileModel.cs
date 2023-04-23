using Newtonsoft.Json;
using System;
using System.Xml;
using UnityEngine;

namespace Assets.Scripts.Classes.Models.Level.Map
{
    public class TileModel
    {
        [JsonProperty]
        public Type TileType { get; set; }
        [JsonProperty]
        public Vector3Int Position { get; set; }
        [JsonProperty]
        public string Sprite { get; set; }
    }
}
