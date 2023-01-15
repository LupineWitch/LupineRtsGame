using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.Classes.Models.Level
{
    public interface ILevelPartModel<Type>
    {
        public XmlNode SerialiseToNode(XmlDocument document);
        public void DeserialiseFromNode(XmlNode node);
    }


    public class MapModel : ILevelPartModel<MapModel>
    {
        public string Name { get; private set; }
        public string Version { get; protected set; }
        public IReadOnlyDictionary<int, List<TileModel>> MapLayers => mapLayers;

        private Dictionary<int, List<TileModel>> mapLayers { get; set; }
        private List<TileObjectModel> containedObjects;

        public MapModel(string name)
        {
            Name = name;
            Version = "v0.0.1";
            mapLayers = new Dictionary<int, List<TileModel>>();
            containedObjects = new List<TileObjectModel>();
        }

        public void AddCell(Tile tile, Vector3Int position)
        {
            TileModel model = new TileModel()
            {
                TileType = tile.GetType(),
                Position = position,
                Sprite = tile.sprite.name
            };

            if (!mapLayers.ContainsKey(position.z))
                mapLayers.Add(position.z, new List<TileModel>());

            mapLayers[position.z].Add(model);
        }

        public void AddTileObject(TileObjectModel tileObject) => containedObjects.Add(tileObject);

        public void AddTileObjects(IEnumerable<TileObjectModel> tileObjects) => containedObjects.AddRange(tileObjects);

        public virtual XmlNode SerialiseToNode(XmlDocument document)
        {
            XmlNode parentElement = document.CreateElement(nameof(MapModel));
            XmlNode nameEl, versionEl, objectListEl, mapLayersEl;
            nameEl = document.CreateElement(nameof(Name));
            nameEl.AppendChild(document.CreateTextNode(Name));
            versionEl = document.CreateElement(nameof(Version));
            versionEl.AppendChild(document.CreateTextNode(Version));
            objectListEl = document.CreateElement(nameof(containedObjects));
            mapLayersEl = document.CreateElement(nameof(mapLayers));
            foreach(var levelTilesPair in mapLayers)
            {
                var layerEl = document.CreateElement("layer");
                layerEl.SetAttribute("Level", levelTilesPair.Key.ToString());
                foreach(TileModel tile in levelTilesPair.Value)
                    layerEl.AppendChild(tile.SerialiseToNode(document));

                mapLayersEl.AppendChild(layerEl);
            }
            parentElement.AppendChild(nameEl);
            parentElement.AppendChild(versionEl);
            parentElement.AppendChild(objectListEl);
            parentElement.AppendChild(mapLayersEl);

            return parentElement;
        }

        public void DeserialiseFromNode(XmlNode node)
        {
            throw new NotImplementedException();
        }

    }
}
