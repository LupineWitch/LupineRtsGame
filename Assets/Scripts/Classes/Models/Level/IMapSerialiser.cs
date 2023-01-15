using Assets.Scripts.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.Classes.Models.Level
{
    internal interface IMapSerialiser
    {
        public void SerialiseTilemapToAFile(Tilemap map, string filepath, MapManager mapManger);
    }
}
