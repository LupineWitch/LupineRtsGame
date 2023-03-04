using Assets.Scripts.Classes.Events;
using Assets.Scripts.Classes.Helpers;
using Assets.Scripts.Classes.Models.Level;
using Assets.Scripts.Classes.Static;
using Assets.Scripts.Classes.TileOverlays;
using Assets.Scripts.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.Managers
{
    public class AvailableBuidlingSpaceManager : MonoBehaviour
    {
        //Constants
        private const string tilePalletsBasePath = "Graphics\\Tilepallets\\UtilityPaletteAssets";
        [SerializeField]
        private const string buildAccessTileName = "BasicWhiteTile";
        
        //Serializable Fields
        [SerializeField]
        private Tilemap mainTilemap;
        [SerializeField]
        private GameObject overlaysRoot;
        [SerializeField]
        private int buildingAreaDiameter = 11;

        //Fields
        private BasicControls basicControls;
        private ITopCellSelector topCellSelector;
        private Tile tileToSet;
        private OverlayBuildingZone buildingOverlay;
        private InputAction pointerPosition;
        private Dictionary<Vector3Int, bool> areCellsOccupiedByBuildings;
        private List<Vector3Int> allPositionsWithinBounds = new List<Vector3Int>();
        private BuildingBase currentlySelectedBuilding;
        private bool shouldShow;

        //Delegates
        private TopCellResult GetTopCellAtMousePos(Vector2 mousePosition) => topCellSelector.GetTopCell(Camera.main.ScreenToWorldPoint(mousePosition));        
        private ColorPredicate isCellBuildable; 

        public void Show(bool shouldShow)
        {
            this.shouldShow = shouldShow;
        }

        public void SetSelectedBuilding(BuildingBase building) => currentlySelectedBuilding = building;
        public void UnsetSelectedBuilding() => currentlySelectedBuilding = null; 

        private void Awake()
        {
            basicControls = new BasicControls();
            topCellSelector = new TopCellSelector(mainTilemap);
            tileToSet = Resources.Load<Tile>(Path.Combine(tilePalletsBasePath, buildAccessTileName));

            buildingOverlay = new OverlayBuildingZone(buildingAreaDiameter,mainTilemap, overlaysRoot, tileToSet);
            areCellsOccupiedByBuildings = new Dictionary<Vector3Int, bool>();

            isCellBuildable = DefaultBuildingZonePredicate;
        }

        private void OnEnable()
        {
            basicControls.CommandControls.Enable();
            pointerPosition = basicControls.CommandControls.PointerPosition;
        }

        private void Update()
        {
            DrawBuildingOverlay();
        }

        private void DrawBuildingOverlay()
        {
            Vector2 currentMousePos = pointerPosition.ReadValue<Vector2>();
            TopCellResult tileClickResult = GetTopCellAtMousePos(currentMousePos);
            if (!tileClickResult.found || !shouldShow || currentlySelectedBuilding == null)
                return;

            Vector2Int size = currentlySelectedBuilding.BuildingSize;
            Vector3Int bottomLeftCorner = new Vector3Int(tileClickResult.topCell.x - (size.x.GetEvenInteger() / 2), tileClickResult.topCell.y - (size.y.GetEvenInteger() / 2), tileClickResult.topCell.z);
            BoundsInt newBounds = new BoundsInt(bottomLeftCorner, new Vector3Int(size.x, size.y, 1));
            allPositionsWithinBounds.Clear();
            foreach (var pos in newBounds.allPositionsWithin)
                allPositionsWithinBounds.Add(pos);

            buildingOverlay.DrawUsingColorPredicateAndCenter(allPositionsWithinBounds, tileClickResult.topCell, isCellBuildable);
        }

        private Color DefaultBuildingZonePredicate(Vector3Int cell, Tilemap tilemap)
        {
            bool canBePlaced = true;

            if (areCellsOccupiedByBuildings.TryGetValue(cell, out bool isOccupied))
                canBePlaced &= !isOccupied;

            if (!tilemap.HasTile(cell) || tilemap.GetTopTilePosition(cell).z != 2)
                canBePlaced = false;

            return canBePlaced ? Color.green : Color.red;
        }

        public void OnCommanderSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UnsetSelectedBuilding();
            Show(false);
            buildingOverlay.DestroyOverlay();
        }
    }
}
