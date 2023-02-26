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
    [SerializeField]
    private BuildingBase buildingPrefab;
    [SerializeField]
    private GameObject buildingParent;
    [SerializeField]
    private Tilemap mainTilemap;
    [SerializeField]
    private MapManager mapManager;

    private float cellSize;

    private void Awake()
    {
        cellSize = mainTilemap.cellSize.magnitude;
    }

    //Factory method?
    //Definietly needs factory pattern
    public bool TryToPlaceBuildingInWorld(Vector3Int chosenCenterTile) 
    {
        BuildingBase newBuilding = Instantiate(buildingPrefab, mainTilemap.CellToWorld(chosenCenterTile),
                                                            Quaternion.identity, buildingParent.transform);

        newBuilding.transform.localPosition -= 0.5f * (new Vector3(0, newBuilding.SpriteHeigth - mainTilemap.layoutGrid.cellSize.y));
        Vector3Int bottomLeftCorner = new Vector3Int(chosenCenterTile.x - Mathf.CeilToInt(newBuilding.BuildingSize.x / 2),
                                                     chosenCenterTile.y - Mathf.CeilToInt(newBuilding.BuildingSize.y / 2),
                                                     chosenCenterTile.z);

        BoundsInt gridBounds = new BoundsInt(bottomLeftCorner, new Vector3Int(newBuilding.BuildingSize.x, newBuilding.BuildingSize.y, 1));
        List<Vector2> occupiedWorldPositions = new List<Vector2>();

        BaseCellForeman debugForeman = new BaseCellForeman(mainTilemap);
        foreach (var pos in gridBounds.allPositionsWithin)
        {
            occupiedWorldPositions.Add(mainTilemap.GetCellCenterWorld(pos));
            debugForeman.TryPaintCell(pos, Color.magenta);
        }

        newBuilding.OnCreated += mapManager.BuildingCreatedCallback;
        newBuilding.OnDestroyed += mapManager.BuildingDestroyedCallback;
        newBuilding.Initialize(cellSize, chosenCenterTile.z, occupiedWorldPositions.ToArray());

        return true;
    }
}
