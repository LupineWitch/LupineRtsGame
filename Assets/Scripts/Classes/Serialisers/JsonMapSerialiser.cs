using Assets.Scripts.Classes.Models.Entity;
using Assets.Scripts.Classes.Models.Level;
using Assets.Scripts.Classes.Models.Level.Map;
using Assets.Scripts.Managers;
using Newtonsoft.Json;
using Newtonsoft.Json.UnityConverters.Math;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.Classes.Serialisers
{
    public class JsonMapSerialiser : IMapSerialiser
    {
        private const string tilePalletsBasePath = "Graphics\\Tilepallets\\DefaultPaletteAssets\\";

        public void DeserialiseMapFromAFileToTileMap(Tilemap map, string filepath, MapManager mapManger)
        {
            string jsonContents = File.ReadAllText(filepath);
            var settings = new JsonSerializerSettings
            {
                Converters = new JsonConverter[]
                {
                    new TypeJsonConverter(),
                    new ISerializableComponentJsonConverter()
                },
            };

            MapModel mapModel = JsonConvert.DeserializeObject<MapModel>(jsonContents, settings);
            DeserialiseMapModelToTilemap(map, mapModel, mapManger);
        }

        public void DeserialiseMapModelToTilemap(Tilemap map, MapModel mapModel, MapManager mapManger)
        {
            //Turn map model into TileMap
            foreach (var layer in mapModel.MapLayers)
            {
                foreach (TileModel tileModel in layer.Value)
                {
                    //TODO: add polymorphism check
                    Tile tileToSet = Resources.Load<Tile>(Path.Combine(tilePalletsBasePath, tileModel.Sprite));
                    map.SetTile(tileModel.Position, tileToSet);
                }
            }

           
            mapManger.DeserialiseInMapManager(mapModel);
        }

        public void SerialiseTilemapToAFile(Tilemap map, string filepath, string mapName, MapManager mapManger)
        {
            MapModel mapModel = new(mapName);
            foreach (Vector3Int position in map.cellBounds.allPositionsWithin)
            {
                if (!map.HasTile(position))
                    continue;

                Tile tileAtPos = map.GetTile<Tile>(position);
                mapModel.AddCell(tileAtPos, position);
            }

            mapModel.MapEntities = mapManger.GetEntitiesToSerialise();
            mapModel.startingPositions = mapManger.GetSerialisableStartingPositions();

            var settings = new JsonSerializerSettings
            {
                Converters = new List<JsonConverter>()
                {
                    new TypeJsonConverter(),
                    new Vector3Converter(),
                    new Vector3IntConverter()
                },
                ContractResolver = new JsonEntityCustomPropertyResolver(),
                NullValueHandling = NullValueHandling.Ignore,
            };

            string jsonContent = JsonConvert.SerializeObject(mapModel, settings);
            File.WriteAllText(filepath, jsonContent);
        }
    }
}
