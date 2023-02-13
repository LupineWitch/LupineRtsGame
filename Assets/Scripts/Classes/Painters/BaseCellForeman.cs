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

        public virtual bool TryPaintCell(Vector3Int coords, Color color)
        {
            if (!tilemap.HasTile(coords))
                return false;

            TileFlags tileFlags = tilemap.GetTileFlags(coords);
            tilemap.SetTileFlags(coords, TileFlags.None);
            tilemap.SetColor(coords, color);
            tilemap.SetTileFlags(coords, tileFlags);
            
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
            TileFlags tileFlags = tilemap.GetTileFlags(coords);
            tilemap.SetTileFlags(coords, TileFlags.None);
            tilemap.SetTile(coords, tile);
            tilemap.SetTileFlags(coords, TileFlags.None);
            tilemap.SetColor(coords, color);
            tilemap.SetTileFlags(coords, tileFlags);

            return true;
        }
    }
}
