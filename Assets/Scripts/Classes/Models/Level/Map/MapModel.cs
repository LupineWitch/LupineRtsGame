using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.Classes.Models.Level.Map
{
    public interface ILevelPartXMLModel<Type>
    {
        public XmlNode SerialiseToNode(XmlDocument document);
        public void DeserialiseFromNode(XmlNode node);
    }


    public class MapModel : ILevelPartXMLModel<MapModel>
    {
        public string Name { get; private set; }
        public string Version { get; protected set; }
        public Dictionary<int, List<TileModel>> MapLayers { get; set; }

        private List<TileObjectModel> containedObjects;

        public MapModel(string name)
        {
            Name = name;
            Version = "v0.0.1";
            MapLayers = new Dictionary<int, List<TileModel>>();
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

            if (!MapLayers.ContainsKey(position.z))
                MapLayers.Add(position.z, new List<TileModel>());

            MapLayers[position.z].Add(model);
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
            mapLayersEl = document.CreateElement(nameof(MapLayers));
            foreach (var levelTilesPair in MapLayers)
            {
                var layerEl = document.CreateElement("layer");
                layerEl.SetAttribute("Level", levelTilesPair.Key.ToString());
                foreach (TileModel tile in levelTilesPair.Value)
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
