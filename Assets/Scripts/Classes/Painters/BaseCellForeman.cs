using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.Classes.Painters
{
    public class BaseCellForeman
    {
        private readonly Tilemap tilemap;

        public BaseCellForeman(Tilemap tilemap)
        {
            this.tilemap = tilemap;
        }

        public virtual bool TryPaintCell(Vector3Int coords, Color color, bool force = false)
        {
            if (!tilemap.HasTile(coords))
                return false;

            TileFlags tileFlags = tilemap.GetTileFlags(coords);
            if(force)
                tilemap.SetTileFlags(coords, TileFlags.None);

            if (tileFlags.HasFlag(TileFlags.LockColor))
                return false;

            tilemap.SetColor(coords, color);
            if (force)
                tilemap.SetTileFlags(coords, tileFlags);
            
            return true;
        }

        public virtual void PaintCells(IEnumerable<Vector3Int> cells, Color color, bool force = false)
        {
            foreach (var cell in cells)
                _ = TryPaintCell(cell, color, force);
        }

        public virtual void CreatePaintedCells(IEnumerable<Vector3Int> cells, TileBase tile, Color color, bool force = false)
        {
            foreach (var cell in cells)
                _ = TryCreatePaintedCell(cell, color, tile, force);
        }

        public virtual bool TryCreatePaintedCell(Vector3Int coords, Color color, TileBase tile, bool force = false)
        {

            TileFlags tileFlags = tilemap.GetTileFlags(coords);
            if (force)
                tilemap.SetTileFlags(coords, TileFlags.None);

            if (tileFlags.HasFlag(TileFlags.LockColor) || tileFlags.HasFlag(TileFlags.LockTransform))
                return false;

            tilemap.SetTile(coords, tile);
            tilemap.SetColor(coords, color);
            if (force)
                tilemap.SetTileFlags(coords, tileFlags);

            return true;
        }
    }
}
