using Assets.Scripts.Classes.Painters;
using Assets.Scripts.Classes.Static;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.Classes.TileOverlays
{
    public class OverlayBuildingZone : OverlayBase
    {
        private int buildingAreaDiameter = 11;
        public int buildingAreaRadius => buildingAreaDiameter.GetEvenInteger() / 2;

        public OverlayBuildingZone(int buildingAreaDiameter, Tilemap terrainLayer, GameObject overlaysObject, TileBase tile) : base(terrainLayer, overlaysObject, tile)
        {
            this.buildingAreaDiameter = buildingAreaDiameter;
        }

        public override object Clone() => new OverlayBuildingZone(buildingAreaDiameter, terrainLayer, overlaysObject, OverlayTile);


        public override void DrawUsingColorPredicate(IEnumerable<Vector3Int> cells, ColorPredicate colorPredicate)
        {
            this.DestroyOverlay();
            
            foreach (var cell in cells)
                overlayPainter.TryCreatePaintedCell(cell, colorPredicate(cell), OverlayTile);
        }

        public void DrawUsingColorPredicateAndCenter(IEnumerable<Vector3Int> cells, Vector3Int center, ColorPredicate colorPredicate)
        {
            this.DestroyOverlay();

            Vector3Int bottomLeftCorner = new Vector3Int(center.x - buildingAreaRadius, center.y - buildingAreaRadius, center.z);
            BoundsInt overlayBounds = new BoundsInt(bottomLeftCorner, new Vector3Int(buildingAreaDiameter, buildingAreaDiameter, 1));
            
            foreach (var cell in overlayBounds.allPositionsWithin)
                overlayPainter.TryCreatePaintedCell(cell, Color.gray, OverlayTile);

            foreach (var buildingCell in cells)
                overlayPainter.TryPaintCell(buildingCell, colorPredicate(buildingCell, terrainLayer));
        }
    }
}
