using Assets.Scripts.Classes.Models.Level;
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
        private const string tilePalletsBasePath = "Graphics\\Tilepallets\\UtilityPaletteAssets";
        [SerializeField]
        private const string buildAccessTileName = "BasicWhiteTile";
        
        [SerializeField]
        private Tilemap mainTilemap;
        [SerializeField]
        private Tilemap buildingSpaceTilemap;

        private bool shouldShow {get; set;}

        private BasicControls basicControls;
        private ITopCellSelector topCellSelector;
        private int buildingAreaDiameter = 7;
        private int buildingAreaRadius => (buildingAreaDiameter % 2 == 0 ? (buildingAreaDiameter + 1) / 2 : buildingAreaDiameter / 2);
        private BoundsInt previousBounds = default;
        Tile tileToSet;
        InputAction pointerPosition;
        //Editor-only debug variables
#if UNITY_EDITOR
        private Vector2 prevMousePos;
#endif

        private TopCellResult GetTopCellAtMousePos(Vector2 mousePosition) => topCellSelector.GetTopCell(Camera.main.ScreenToWorldPoint(mousePosition));        

        private void Awake()
        {
            basicControls = new BasicControls();
            topCellSelector = new TopCellSelector(mainTilemap);
            tileToSet = Resources.Load<Tile>(Path.Combine(tilePalletsBasePath, buildAccessTileName));
        }

        private void OnEnable()
        {
            basicControls.CommandControls.Enable();
            pointerPosition = basicControls.CommandControls.PointerPosition;
        }

        private void Update()
        {
            Vector2 currentMousePos = pointerPosition.ReadValue<Vector2>();
            var result = GetTopCellAtMousePos(currentMousePos);
            if (!result.found || !shouldShow)
            {
                buildingSpaceTilemap.ClearAllTiles();
                return;
            }
            Vector3Int bottomLeftCorner = new Vector3Int(result.topCell.x - buildingAreaRadius, result.topCell.y - buildingAreaRadius, 0);
            BoundsInt newBounds = new BoundsInt(bottomLeftCorner, new Vector3Int(buildingAreaDiameter, buildingAreaDiameter, 1));
            DrawTilesAroundMousePosition(newBounds);
        }

        private void DrawTilesAroundMousePosition(BoundsInt bounds)
        {
            ///Dont draw again, if bounds are the same
            buildingSpaceTilemap.ClearAllTiles();
            if (bounds.Equals(previousBounds))
                return;

            int positionCount = 0;
            foreach (var position in bounds.allPositionsWithin)
            {
                buildingSpaceTilemap.SetTile(position, tileToSet);
                positionCount++;
            }

            if (positionCount != buildingAreaDiameter * buildingAreaDiameter)
                Debug.LogErrorFormat("Some fields are not in bounds!!!\nCounted positions: {}\nExpected: {1}", positionCount, buildingAreaDiameter * buildingAreaDiameter);
        }

        public void Show(bool shouldShow)
        {
            this.shouldShow = shouldShow;
        }
    }
}
