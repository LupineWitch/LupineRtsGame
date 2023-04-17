using Assets.Scripts.Managers;
using UnityEngine;

namespace Assets.Scripts.Classes.Factories.Building
{
    public interface IBuildingFactory
    {
        public BuildingBase CreateAndPlaceBuildingBasedOnPrefab(BuildingBase prefab, Vector3Int tilePosition, GameObject parent, MapManager map);
    }
}
