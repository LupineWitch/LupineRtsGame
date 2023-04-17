using Assets.Scripts.Managers;
using UnityEngine;

namespace Assets.Scripts.Classes.Factories.Building
{
    public class PrefabbedBuildingFactory : IBuildingFactory
    {
        public BuildingBase CreateAndPlaceBuildingBasedOnPrefab(BuildingBase prefab, Vector3Int tilePosition, GameObject parent, MapManager map)
        {
            BuildingBase newBuilding = UnityEngine.Object.Instantiate(prefab, map.MainTilemap.CellToWorld(tilePosition),
                                                                      Quaternion.identity, parent.transform);

            newBuilding.transform.localPosition -= 0.5f * (new Vector3(0, newBuilding.SpriteHeigth - map.MainTilemap.layoutGrid.cellSize.y));
            Vector3Int bottomLeftCorner = new Vector3Int(tilePosition.x - Mathf.CeilToInt(newBuilding.BuildingSize.x / 2),
                                                         tilePosition.y - Mathf.CeilToInt(newBuilding.BuildingSize.y / 2),
                                                         tilePosition.z);

            BoundsInt gridBounds = new BoundsInt(bottomLeftCorner, new Vector3Int(newBuilding.BuildingSize.x, newBuilding.BuildingSize.y, 1));

            newBuilding.Created += map.BuildingCreatedCallback;
            newBuilding.Destroyed += map.BuildingDestroyedCallback;
            newBuilding.Initialize(tilePosition.z, gridBounds, tilePosition);

            return newBuilding;
        }
    }
}
