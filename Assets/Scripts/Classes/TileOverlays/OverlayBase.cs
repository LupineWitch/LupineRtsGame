using Assets.Scripts.Classes.Painters;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEngine.Tilemaps.TilemapRenderer;
using UnityEngine.WSA;

namespace Assets.Scripts.Classes.TileOverlays
{
    public abstract class OverlayBase: IDisposable, ICloneable
    {

        protected readonly Tilemap terrainLayer;
        protected readonly GameObject overlaysObject;
        protected readonly Tilemap overlayLayer;
        private BaseCellForeman _overlayPainter;
        protected BaseCellForeman overlayPainter
        {
            get
            {
                if (_overlayPainter == null)
                    _overlayPainter = new BaseCellForeman(overlayLayer);

                return _overlayPainter;
            }

            set => _overlayPainter = value;
        }
        protected bool disposedValue;

        protected OverlayBase(Tilemap terrainLayer, GameObject overlaysObject, TileBase tile)
        {
            this.terrainLayer = terrainLayer;
            this.overlaysObject = overlaysObject;

            this.terrainLayer = terrainLayer;
            this.overlaysObject = overlaysObject;
            TilemapRenderer tilemapRenderer = new GameObject("pathOverlay").AddComponent<TilemapRenderer>();
            tilemapRenderer.sortOrder = SortOrder.TopRight;
            tilemapRenderer.sortingOrder = 3;
            tilemapRenderer.mode = Mode.Individual;
            tilemapRenderer.material = terrainLayer.GetComponent<TilemapRenderer>().material;
            overlayLayer = tilemapRenderer.GetComponent<Tilemap>();
            overlayLayer.color = new Color(1, 1, 1, 0.50f);
            overlayLayer.gameObject.transform.SetParent(overlaysObject.transform);
            OverlayTile = tile;
        }

        public TileBase OverlayTile { get; }

        public abstract object Clone();

        public void DestroyOverlay() => overlayLayer.ClearAllTiles();

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

        protected abstract void Draw(IEnumerable<Vector3Int> pathCells);
    }
}