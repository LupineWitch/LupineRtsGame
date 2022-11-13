using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Tilemaps;
using UnityEngine;
using TMPro;

namespace Assets.Scripts.Helpers
{
    internal class MouseGridHelper
    {
        //Tweak based on you tile origin/anchor; constant offset applied to mouse coordinates
        public Vector3 mouseOffset = new Vector3(0f, 0f, 0f);

        //y difference for each 1 unit of z;
        public float halfCellHeight = 0.25f;
        //0 to zMax are iterated through when selecting a tile
        private int zMax;
        private Tilemap _tilemap;

        public MouseGridHelper(Tilemap tilemap)
        {
            _tilemap = tilemap;
            zMax = _tilemap.cellBounds.zMax;
        }


        public Vector3Int GetTopCell(Vector2 mouseWorldPos)
        {
            Debug.LogFormat("{0}: {1}", nameof(mouseWorldPos), mouseWorldPos);
            Vector3Int gridCell = _tilemap.WorldToCell(mouseWorldPos);
            Debug.LogFormat("WorldToCell: {0}", gridCell);

            for (int z = zMax; z > -1; z--)
            {
                gridCell.z = z;
                TileBase tile = _tilemap.GetTile(gridCell);
                if (tile != null)
                {
                    Debug.Log("grid" + gridCell + " found " + tile.name);
                    return gridCell; // If you need only the top most tile of that pile
                }
            }

            return Vector3Int.zero;
        }

        public Vector3Int GetTopCell2(Vector2 mouseWorldPos)
        {
            Debug.LogFormat("{0}: {1}", nameof(mouseWorldPos), mouseWorldPos);
            Vector3Int gridCell = _tilemap.WorldToCell(mouseWorldPos);
            Debug.LogFormat("WorldToCell: {0}", gridCell);

            mouseWorldPos.y -= zMax * halfCellHeight; // to be adapted depending on your max Z and the start of the loop below
            for (int z = zMax; z > -1; z--)
            {
                gridCell = _tilemap.WorldToCell(mouseWorldPos);
                gridCell.z = z;
                var tile = _tilemap.GetTile(gridCell);
                if (tile != null)
                {
                    Debug.Log("grid Z" + gridCell + " found " + tile.name);
                    return gridCell; // If you need only the first tile encountered
                }
                mouseWorldPos.y += halfCellHeight;
            }

            return Vector3Int.zero;
        }
    }
}
