using Assets.Scripts.Classes.Models.Level;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Classes.DPL
{
    [CreateAssetMenu(fileName = "Data", menuName = "Data Persistence/LoadMapPersistenceData")]
    public class LoadMapPersistenceData : ScriptableObject
    {
        public string LoadedMapFile { get; set; }
        public MapModel LoadedMapModel { get; set; }
    }
}
