using Assets.Scripts.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Classes.Factories.Building
{
    public interface IBuildingFactory
    {
        public BuildingBase CreateAndPlaceBuildingBasedOnPrefab(BuildingBase prefab, Vector3Int tilePosition, GameObject parent, MapManager map);
    }
}
