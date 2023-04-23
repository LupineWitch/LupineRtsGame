using Newtonsoft.Json;
using System;
using System.Xml;
using UnityEngine;

namespace Assets.Scripts.Classes.Models.Level.Map
{
    public abstract class TileObjectModel 
    {
        [JsonProperty]
        public string Name { get; set; }
        [JsonProperty]
        public Type TileObjectType { get; set; }
        [JsonProperty]
        public Vector3 Position { get; set; }
    }

    public class TileScriptModel : TileObjectModel
    {

    }

    public class TileGameObjectModel : TileObjectModel
    {

    }
}
