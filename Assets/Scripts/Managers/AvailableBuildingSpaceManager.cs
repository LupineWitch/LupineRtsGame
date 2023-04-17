using Assets.Scripts.Classes.Events;
using Assets.Scripts.Classes.Static;
using Assets.Scripts.Classes.TileOverlays;
using Assets.Scripts.Helpers;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.Managers
{
    public class AvailableBuildingSpaceManager : MonoBehaviour
    {
        //Constants
        [SerializeField]
        private const string buildAccessTileName = "Assets/Tilepallets/UtilityPaletteAssets/BasicWhiteTile.asset";

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
        private Dictionary<Vector3Int, bool> areCellsOccupiedByBuildings;
        private List<Vector3Int> allPositionsWithinBounds = new List<Vector3Int>();
        private BuildingBase currentlySelectedBuilding;
        private bool shouldShow;

        //Delegates
        private TopCellResult GetTopCellAtMousePos(Vector2 mousePosition) => topCellSelector.GetTopCell(Camera.main.ScreenToWorldPoint(mousePosition));
        private ColorPredicate isCellBuildable;

        public Tilemap GetTilemap => mainTilemap;

        public void Show(bool shouldShow)
        {
            this.shouldShow = shouldShow;
        }

        public void SetSelectedBuilding(BuildingBase building) => currentlySelectedBuilding = building;
        public void UnsetSelectedBuilding() => currentlySelectedBuilding = null;

        protected virtual void Awake()
        {
            basicControls = new BasicControls();
            topCellSelector = new TopCellSelector(mainTilemap);
            var loadingHandle = Addressables.LoadAssetAsync<Tile>(buildAccessTileName);
            loadingHandle.Completed += (obj) =>
            {
                tileToSet = obj.Result;
                buildingOverlay = new OverlayBuildingZone(buildingAreaDiameter, mainTilemap, overlaysRoot, tileToSet);
            };

            areCellsOccupiedByBuildings = new Dictionary<Vector3Int, bool>();
            isCellBuildable = DefaultBuildingZonePredicate;
        }

        protected virtual void OnEnable()
        {
            basicControls.CommandControls.Enable();
        }

        protected virtual void Update()
        {
            DrawBuildingOverlay();
        }

        private void DrawBuildingOverlay()
        {
            Vector2 currentMousePos = basicControls.CommandControls.PointerPosition.ReadValue<Vector2>();
            TopCellResult tileClickResult = GetTopCellAtMousePos(currentMousePos);
            bool shouldDrawThisFrame = tileClickResult.found && shouldShow && currentlySelectedBuilding != null;
            if (!shouldDrawThisFrame)
                return;

            Vector2Int size = currentlySelectedBuilding.BuildingSize;
            Vector3Int bottomLeftCorner = new Vector3Int(tileClickResult.topCell.x - (size.x.GetEvenInteger() / 2), tileClickResult.topCell.y - (size.y.GetEvenInteger() / 2), tileClickResult.topCell.z);
            BoundsInt newBounds = new BoundsInt(bottomLeftCorner, new Vector3Int(size.x, size.y, 1));
            allPositionsWithinBounds.Clear();
            foreach (var pos in newBounds.allPositionsWithin)
                allPositionsWithinBounds.Add(pos);

            buildingOverlay.DrawUsingColorPredicateAndCenter(allPositionsWithinBounds, tileClickResult.topCell, isCellBuildable);
        }

        public virtual bool IsCellOccupiedByBuilding(Vector3Int cell) => areCellsOccupiedByBuildings.TryGetValue(cell, out bool isOccupied) ? isOccupied : false;

        private Color DefaultBuildingZonePredicate(Vector3Int cell, Tilemap tilemap)
        {
            switch (currentlySelectedBuilding.IsCellAvailableForBuilding(cell, this))
            {
                case CellConstructionSuitability.Available:
                    return Color.green;
                case CellConstructionSuitability.Affected:
                    return Color.yellow;
                case CellConstructionSuitability.Unavailable:
                default:
                    return Color.red;
            }
        }

        public void RemoveCellsFromBuildingGrid(IEnumerable<Vector3Int> cells)
        {
            foreach (var cell in cells)
                if (areCellsOccupiedByBuildings.ContainsKey(cell))
                    areCellsOccupiedByBuildings[cell] = true;
                else
                    areCellsOccupiedByBuildings.Add(cell, true);
        }
        internal void RemoveCellsFromBuildingGrid(BoundsInt.PositionEnumerator enumerator)
        {
            foreach (var cell in enumerator)
                if (areCellsOccupiedByBuildings.ContainsKey(cell))
                    areCellsOccupiedByBuildings[cell] = true;
                else
                    areCellsOccupiedByBuildings.Add(cell, true);
        }

        public void AddCellsToBuildingGrid(IEnumerable<Vector3Int> cells)
        {
            foreach (var cell in cells)
                if (areCellsOccupiedByBuildings.ContainsKey(cell))
                    areCellsOccupiedByBuildings[cell] = false;
        }

        public void OnCommanderSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UnsetSelectedBuilding();
            Show(false);
            buildingOverlay.DestroyOverlay();
        }

        internal void AddCellsToBuildingGrid(BoundsInt.PositionEnumerator positionEnumerator)
        {
            foreach (var cell in positionEnumerator)
                if (areCellsOccupiedByBuildings.ContainsKey(cell))
                    areCellsOccupiedByBuildings[cell] = false;
        }
    }
}
