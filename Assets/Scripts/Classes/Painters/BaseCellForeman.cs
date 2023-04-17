using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.Classes.Painters
{
    public class BaseCellForeman
    {
        private readonly Tilemap tilemap;
        protected int overlayZOffset = 0;

        public BaseCellForeman(Tilemap tilemap, int overlayZOffset = 0)
        {
            this.tilemap = tilemap;
            this.overlayZOffset = overlayZOffset;
        }

        public virtual bool TryPaintCell(Vector3Int coords, Color color)
        {
            Vector3Int tempCoords = coords;
            tempCoords.z = coords.z + overlayZOffset;

            if (!tilemap.HasTile(tempCoords))
            {
                Debug.LogWarningFormat("No tile: {0}!", coords);
                return false;
            }

            TileFlags tileFlags = tilemap.GetTileFlags(tempCoords);
            tilemap.SetTileFlags(tempCoords, TileFlags.None);
            tilemap.SetColor(tempCoords, color);
            tilemap.SetTileFlags(tempCoords, tileFlags);

            return true;
        }

        public virtual void PaintCells(IEnumerable<Vector3Int> cells, Color color)
        {
            foreach (var cell in cells)
                _ = TryPaintCell(cell, color);
        }

        public virtual void CreatePaintedCells(IEnumerable<Vector3Int> cells, TileBase tile, Color color)
        {
            foreach (var cell in cells)
                _ = TryCreatePaintedCell(cell, color, tile);
        }

        public virtual bool TryCreatePaintedCell(Vector3Int coords, Color color, TileBase tile)
        {
            _ = tile ?? throw new ArgumentNullException(nameof(tile));

            Vector3Int tempCoords = coords;
            tempCoords.z = coords.z + overlayZOffset;
            TileFlags tileFlags = tilemap.GetTileFlags(tempCoords);
            tilemap.SetTileFlags(tempCoords, TileFlags.None);
            tilemap.SetTile(tempCoords, tile);
            tilemap.SetTileFlags(tempCoords, TileFlags.None);
            tilemap.SetColor(tempCoords, color);
            tilemap.SetTileFlags(tempCoords, tileFlags);

            return true;
        }
    }
}
