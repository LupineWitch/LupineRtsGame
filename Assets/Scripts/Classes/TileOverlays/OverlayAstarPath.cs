using Assets.Scripts.Classes.Helpers;
using Assets.Scripts.Classes.Painters;
using Assets.Scripts.Managers;
using Assets.Scripts.Pathfinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.WSA;
using static UnityEngine.Tilemaps.TilemapRenderer;

namespace Assets.Scripts.Classes.TileOverlays
{
    public class OverlayAstarPath : OverlayBase
    {
        public Color PathColor { get; set; } = Color.green;
        public Color PathStartColor { get; set; } = Color.cyan;
        public Color PathEndColor { get; set; } = Color.blue;

        public OverlayAstarPath(Tilemap terrainLayer, GameObject overlaysParent, TileBase tile) : base(terrainLayer, overlaysParent, tile)
        {
            this.overlayPainter = new BaseCellForeman(this.overlayLayer, 0);
        }

        public void DrawManyPaths(IEnumerable<IEnumerable<Vector3Int>> paths)
        {
            overlayLayer.ClearAllTiles();
            foreach (var path in paths)
                this.Draw(path);
        }

        public void DrawPath(IEnumerable<Vector3Int> pathCells)
        {
            overlayLayer.ClearAllTiles();
            this.Draw(pathCells);
        }

        protected override void Draw(IEnumerable<Vector3Int> pathCells)
        {
            IEnumerator<Vector3Int> cellsEnumerator = pathCells.GetEnumerator();
            cellsEnumerator.MoveNext();
            Vector3Int tempCell = cellsEnumerator.Current;
            var positionWithZAdjusted = terrainLayer.GetTopTilePosition(tempCell);
            overlayPainter.TryCreatePaintedCell(positionWithZAdjusted, PathStartColor, OverlayTile);

            while (cellsEnumerator.MoveNext())
            {
                overlayPainter.TryCreatePaintedCell(terrainLayer.GetTopTilePosition(cellsEnumerator.Current), PathColor, OverlayTile);
                tempCell = cellsEnumerator.Current;
            }

            overlayPainter.TryCreatePaintedCell(terrainLayer.GetTopTilePosition(tempCell), PathEndColor, OverlayTile);
        }

        public override object Clone()
        {
            return new OverlayAstarPath(terrainLayer, overlaysObject, OverlayTile)
            {
                PathColor = this.PathColor,
                PathEndColor = this.PathEndColor,
                PathStartColor = this.PathStartColor
            };
        }
    }
}
