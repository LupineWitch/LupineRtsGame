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

namespace Assets.Scripts.Classes.TileOverlays
{
    public class OverlayAstarPath : IDisposable, ICloneable
    {
        public Color PathColor { get; set; } = Color.green;
        public Color PathStartColor { get; set; } = Color.yellow;
        public Color PathEndColor { get; set; } = Color.blue;
        public TileBase OverlayTile { get; }

        private readonly Tilemap terrainLayer;
        private readonly GameObject overlaysObject;
        private readonly Tilemap overlayLayer;
        private readonly BaseCellForeman overlayPainter;
        private bool disposedValue;

        public OverlayAstarPath(Tilemap terrainLayer, GameObject overlaysObject, TileBase tile)
        {
            this.terrainLayer = terrainLayer;
            this.overlaysObject = overlaysObject;
            overlayLayer = new GameObject("pathOverlay").AddComponent<TilemapRenderer>().GetComponent<Tilemap>();
            overlayLayer.color = new Color(1, 1, 1, 0.70f);
            overlayLayer.gameObject.transform.SetParent(overlaysObject.transform);
            OverlayTile = tile;
            overlayPainter = new BaseCellForeman(overlayLayer);
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

        private void Draw(IEnumerable<Vector3Int> pathCells)
        {
            IEnumerator<Vector3Int> cellsEnumerator = pathCells.GetEnumerator();
            cellsEnumerator.MoveNext();
            Vector3Int tempCell = cellsEnumerator.Current;
            overlayPainter.TryCreatePaintedCell(terrainLayer.GetTopTilePosition(tempCell), PathStartColor, OverlayTile, true);

            while (cellsEnumerator.MoveNext())
            {
                overlayPainter.TryCreatePaintedCell(terrainLayer.GetTopTilePosition(cellsEnumerator.Current), PathStartColor, OverlayTile, true);
                tempCell = cellsEnumerator.Current;
            }

            overlayPainter.TryCreatePaintedCell(terrainLayer.GetTopTilePosition(tempCell), PathStartColor, OverlayTile, true);
        }
               
        public void DestroyOverlay() => overlayLayer.ClearAllTiles();

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    overlayLayer.ClearAllTiles();
                    UnityEngine.Object.Destroy(overlayLayer.gameObject);
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~OverlayAstarPath()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public object Clone()
        {
            return new OverlayAstarPath(terrainLayer, overlaysObject, OverlayTile);
        }
    }
}
