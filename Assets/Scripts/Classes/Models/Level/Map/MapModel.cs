using Assets.Scripts.Classes.Models.Entity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.Classes.Models.Level.Map
{

    public class MapModel
    {
        [JsonProperty]
        public string Name { get; private set; }
        [JsonProperty]
        public string Version { get; protected set; }
        [JsonProperty]
        public Dictionary<int, List<TileModel>> MapLayers { get; set; }
        [JsonProperty]
        public List<SerializedEntityBase> MapEntities { get; set; }
        [JsonProperty]
        public List<StartingConditionsModel> StartingPositions { get; set; }
        [JsonProperty]
        public BoundsInt MapSize { get; set; }
        [JsonProperty]
        public string Difficult { get; set; }

        private readonly List<TileObjectModel> containedObjects;

        public MapModel(string name)
        {
            Name = name;
            Version = "v0.0.1";
            MapLayers = new Dictionary<int, List<TileModel>>();
            containedObjects = new List<TileObjectModel>();
        }

        public void AddCell(Tile tile, Vector3Int position)
        {
            TileModel model = new()
            {
                TileType = tile.GetType(),
                Position = position,
                Sprite = tile.sprite.name
            };

            if (!MapLayers.ContainsKey(position.z))
                MapLayers.Add(position.z, new List<TileModel>());

            MapLayers[position.z].Add(model);
        }

        public void AddTileObject(TileObjectModel tileObject) => containedObjects.Add(tileObject);

        public void AddTileObjects(IEnumerable<TileObjectModel> tileObjects) => containedObjects.AddRange(tileObjects);

    }
}
