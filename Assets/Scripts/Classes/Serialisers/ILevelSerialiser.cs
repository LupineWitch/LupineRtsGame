using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.Classes.Serialisers
{
    public interface ILevelSerialiser
    {
        public void SerialiseLevel(Tilemap tilemap, string filename);

        public void DeserialiseLevel(Tilemap tilemap, string filename);
    }
}
