using Assets.Scripts.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Tilemaps;

namespace Assets.Editor.Classes
{
    public interface IMapSerialiser
    {
        public void SerialiseTilemapToAFile(Tilemap map, string filepath, string mapName, MapManager mapManger);

        public void DeserialiseMapFromAFileToTileMap(Tilemap map, string filepath, MapManager mapManger);
    }
}
