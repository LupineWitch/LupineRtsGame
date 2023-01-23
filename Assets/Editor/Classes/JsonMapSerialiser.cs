﻿using Assets.Scripts.Classes.Models.Level;
using Assets.Scripts.Managers;
using Codice.CM.SEIDInfo;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Editor.Classes
{
    public class JsonMapSerialiser : IMapSerialiser
    {
        private const string tilePalletsBasePath = "Graphics\\Tilepallets\\DefaultPaletteAssets\\";

        public void DeserialiseMapFromAFileToTileMap(Tilemap map, string filepath, MapManager mapManger)
        {
            string jsonContents = File.ReadAllText(filepath);
            var settings = new JsonSerializerSettings
            {
                Converters = new[]
                {
                    new TypeJsonConverter()
                },
            };
            
            MapModel mapModel = JsonConvert.DeserializeObject<MapModel>(jsonContents, settings);
            //Turn map model into TileMap
            foreach(var layer in mapModel.MapLayers)
            {
                foreach(TileModel tileModel in layer.Value)
                {
                    Type tileType = tileModel.TileType;
                    //TODO: add plymorphism check
                    
                    Tile tileToSet = Resources.Load<Tile>(Path.Combine(tilePalletsBasePath, tileModel.AssetName));
                    map.SetTile(tileModel.Position, tileToSet);
                }
            }
        }

        public void SerialiseTilemapToAFile(Tilemap map, string filepath, string mapName, MapManager mapManger)
        {
            MapModel mapModel = new MapModel(mapName);
            foreach (Vector3Int position in map.cellBounds.allPositionsWithin)
            {
                if (!map.HasTile(position))
                    continue;

                Tile tileAtPos = map.GetTile<Tile>(position);
                mapModel.AddCell(tileAtPos, position);
            }

            var settings = new JsonSerializerSettings
            {
                Converters = new[]
                {
                    new TypeJsonConverter()
                },
            };

            string jsonContent = JsonConvert.SerializeObject(mapModel, settings);
            File.WriteAllText(filepath, jsonContent);
        }
    }
}
