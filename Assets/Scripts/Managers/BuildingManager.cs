using Assets.Scripts.Classes.Factories.Building;
using Assets.Scripts.Classes.Painters;
using Assets.Scripts.Helpers;
using Assets.Scripts.Managers;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;


//Implement as Entity manager?
public class BuildingManager : MonoBehaviour
{
    public BuildingBase BuildingPrefab => buildingPrefab;

    [SerializeField]
    private BuildingBase buildingPrefab;
    [SerializeField]
    private GameObject buildingParent;
    [SerializeField]
    private Tilemap mainTilemap;
    [SerializeField]
    private MapManager mapManager;

    private IBuildingFactory buildingsFactory;

    private void Awake()
    {
        buildingsFactory = new PrefabbedBuildingFactory();
    }

    public bool TryToPlaceBuildingInWorld(Vector3Int chosenCenterTile) 
    {
        buildingsFactory.CreateAndPlaceBuildingBasedOnPrefab(buildingPrefab, chosenCenterTile, buildingParent, mapManager);
        return true;
    }
}
