using Assets.Scripts.Classes.Factories.Building;
using Assets.Scripts.Classes.Helpers;
using Assets.Scripts.Classes.Painters;
using Assets.Scripts.Classes.Static;
using Assets.Scripts.Helpers;
using Assets.Scripts.Managers;
using Assets.Scripts.Objects.Buildings;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;


//Implement as Entity manager?
public class BuildingManager : MonoBehaviour
{
    public BuildingBase BuildingPrefab => defaultPrefab;

    public ConstructionSiteBase ConstructionSitePrefab { get => constructionSitePrefab; private set => constructionSitePrefab = value; }

    [SerializeField]
    private BuildingBase defaultPrefab;
    [SerializeField]
    private GameObject buildingParent;
    [SerializeField]
    private Tilemap mainTilemap;
    [SerializeField]
    private MapManager mapManager;
    [SerializeField]
    private ConstructionSiteBase constructionSitePrefab;

    private IBuildingFactory buildingsFactory;

    private void Awake()
    {
        buildingsFactory = new PrefabbedBuildingFactory();
    }

    public BuildingBase TryToPlaceBuildingInWorld(Vector3Int chosenCenterTile)
    {

        return buildingsFactory.CreateAndPlaceBuildingBasedOnPrefab(defaultPrefab, chosenCenterTile, buildingParent, mapManager);
    }

    public BuildingBase TryToPlaceBuildingInWorld(Vector3Int chosenCenterTile, BuildingBase prefab)
    {
        return buildingsFactory.CreateAndPlaceBuildingBasedOnPrefab(prefab, chosenCenterTile, buildingParent, mapManager);
    }

    public Vector3Int GetClosestPointNearBuildSite(Transform from, Vector3Int placementTile, BuildingBase prefab)
    {
        Vector3Int tilePosition = mainTilemap.WorldToCell(from.position);
        return GetClosestPointNearBuildSite(tilePosition, placementTile, prefab);
    }


    public Vector3Int GetClosestPointNearBuildSite(Vector3Int from, Vector3Int placementTile, BuildingBase prefab)
    {
        Vector3Int bottomLeftCorner = new Vector3Int(placementTile.x - prefab.BuildingSize.x.GetEvenInteger() / 2,
                                                     placementTile.y - prefab.BuildingSize.y.GetEvenInteger() / 2,
                                                     placementTile.z);

        Vector3Int boundsSize = new Vector3Int(prefab.BuildingSize.x, prefab.BuildingSize.y, 1);
        BoundsInt boundsInt = new BoundsInt(bottomLeftCorner, boundsSize);
        float minDistance = float.MaxValue;
        Vector3Int closestPositionNearEdge = placementTile;

        foreach (var pos in boundsInt.allPositionsWithin)
        {
            foreach (var neighbour in pos.GetAllCardinalNeighbours())
            {
                if (boundsInt.Contains(neighbour))
                    continue;

                float newDistance = Vector3Int.Distance(from, neighbour);
                if (minDistance > newDistance)
                {
                    minDistance = newDistance;
                    closestPositionNearEdge = neighbour;
                }
            }

            foreach (var neighbour in pos.GetAllDiagonalNeighbours())
            {
                if (boundsInt.Contains(neighbour))
                    continue;

                float newDistance = Vector3Int.Distance(from, neighbour);
                if (minDistance > newDistance)
                {
                    minDistance = newDistance;
                    closestPositionNearEdge = neighbour;
                }
            }
        }

        return closestPositionNearEdge;
    }
}
