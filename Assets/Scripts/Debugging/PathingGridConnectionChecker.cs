using Assets.Scripts.Classes.Helpers;
using Assets.Scripts.Helpers;
using Assets.Scripts.Managers;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class PathingGridConnectionChecker : MonoBehaviour
{
    [SerializeField]
    private Tilemap pathingDebugTilemap;
    [SerializeField]
    private Tilemap mainTilemap;
    [SerializeField]
    private bool shouldShow = false;
    [SerializeField]
    private MapManager mapManager;
    [SerializeField]
    private int CheckRadius = 4;

    private const string tilePalletsBasePath = "Graphics\\Tilepallets\\UtilityPaletteAssets";
    private const string buildAccessTileName = "BasicWhiteTile";
    private BasicControls basicControls;
    private TopCellSelector topCellSelector;
    private Tile tileToSet;
    InputAction pointerPosition;


    private TopCellResult GetTopCellAtMousePos(Vector2 mousePosition) => topCellSelector.GetTopCell(Camera.main.ScreenToWorldPoint(mousePosition));

    private void Awake()
    {
        basicControls = new BasicControls();
        topCellSelector = new TopCellSelector(mainTilemap);
        tileToSet = Resources.Load<Tile>(Path.Combine(tilePalletsBasePath, buildAccessTileName));
        shouldShow = !shouldShow;
    }

    private void OnEnable()
    {
        basicControls.CommandControls.Enable();
        pointerPosition = basicControls.CommandControls.PointerPosition;
    }

    public void Toggle()
    {
        shouldShow = !this.shouldShow;
    }

    private void Start()
    {
        shouldShow = !shouldShow;
    }
    // Update is called once per frame
    void Update()
    {
        Vector2 currentMousePos = pointerPosition.ReadValue<Vector2>();
        var result = GetTopCellAtMousePos(currentMousePos);
        if (!result.found || !shouldShow)
        {
            pathingDebugTilemap.ClearAllTiles();
            return;
        }
        Vector3Int bottomLeftCorner = new Vector3Int(result.topCell.x - CheckRadius, result.topCell.y - CheckRadius, 0);
        BoundsInt newBounds = new BoundsInt(bottomLeftCorner, new Vector3Int(2 * CheckRadius, 2 * CheckRadius, 1));
        DrawTilesAroundMousePosition(newBounds, result.topCell);
    }


    private void DrawTilesAroundMousePosition(BoundsInt bounds, Vector3Int targetCell)
    {
        ///Dont draw again, if bounds are the same
        pathingDebugTilemap.ClearAllTiles();

        foreach (var position in bounds.allPositionsWithin)
        {
            if (targetCell.HasNegativeComponent() || position.HasNegativeComponent())
                continue;

            if (!(mainTilemap.cellBounds.Contains(targetCell) && mainTilemap.cellBounds.Contains(position)))
                continue;

            var positionWithZAdjusted = mainTilemap.GetTopTilePosition(position);
            Color colorOfTile = mapManager.PathingGrid.PathExistsBetweenNodes(targetCell, position) ? Color.green : Color.red;
            pathingDebugTilemap.SetTile(positionWithZAdjusted, tileToSet);
            pathingDebugTilemap.SetTileFlags(positionWithZAdjusted, TileFlags.None);
            pathingDebugTilemap.SetColor(positionWithZAdjusted, colorOfTile);
        }

        pathingDebugTilemap.SetTile(targetCell, tileToSet);
        pathingDebugTilemap.SetTileFlags(targetCell, TileFlags.None);
        pathingDebugTilemap.SetColor(targetCell, Color.yellow);

    }

}
