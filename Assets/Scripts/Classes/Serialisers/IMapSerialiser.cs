using Assets.Scripts.Classes.Models.Level.Map;
using Assets.Scripts.Managers;
using System.Threading.Tasks;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.Classes.Serialisers
{
    public interface IMapSerialiser
    {
        public void SerialiseTilemapToAFile(Tilemap map, string filepath, string mapName, MapManager mapManger);

        public void DeserialiseMapFromAFileToTileMap(Tilemap map, string filepath, MapManager mapManger);

        public void DeserialiseMapModelToTilemap(Tilemap map, MapModel mapModel, MapManager mapManger);
    }
}
