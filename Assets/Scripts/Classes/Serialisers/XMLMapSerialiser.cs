using Assets.Scripts.Managers;
using Assets.Scripts.Classes.Models.Level;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Data.SqlTypes;
using Unity.VisualScripting;

namespace Assets.Scripts.Classes.Serialisers
{
    public class XMLMapSerialiser : IMapSerialiser
    {
        public void DeserialiseMapFromAFileToTileMap(Tilemap map, string filepath, MapManager mapManger)
        {


        }

        public void DeserialiseMapModelToTilemap(Tilemap map, MapModel mapModel)
        {
            throw new NotImplementedException();
        }

        public void SerialiseTilemapToAFile(Tilemap map, string filepath, string mapName, MapManager mapManger)
        {
            //Build MapModel
            MapModel mapModel = new MapModel(mapName);
            foreach (Vector3Int position in map.cellBounds.allPositionsWithin)
            {
                if (!map.HasTile(position))
                    continue;

                Tile tileAtPos = map.GetTile<Tile>(position);
                mapModel.AddCell(tileAtPos, position);
            }
            //TODO: Get Objects from MapManager
            XmlDocument document = new XmlDocument();
            document.AppendChild(mapModel.SerialiseToNode(document));
            document.Save(filepath);
        }
    }
}
