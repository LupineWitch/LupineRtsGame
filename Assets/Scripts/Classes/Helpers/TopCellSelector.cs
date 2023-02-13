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
    internal class TopCellSelector : ITopCellSelector
    {

        //y difference for each 1 unit of z;
        [SerializeField]
        private float halfCellHeight = 0.345f;
        //0 to zMax are iterated through when selecting a tile
        private int zMax;
        private Tilemap _tilemap;

        public TopCellSelector(Tilemap tilemap)
        {
            _tilemap = tilemap;
            zMax = _tilemap.cellBounds.zMax;
        }


        public TopCellResult GetTopCell(Vector2 mouseWorldPos)
        {
            //Debug.LogFormat("{0}: {1}", nameof(mouseWorldPos), mouseWorldPos);
            Vector3Int gridCell = _tilemap.WorldToCell(mouseWorldPos);
            //Debug.LogFormat("WorldToCell: {0}", gridCell);

            mouseWorldPos.y -= zMax * halfCellHeight; // to be adapted depending on your max Z and the start of the loop below
            for (int z = zMax; z > -1; z--)
            {
                gridCell = _tilemap.WorldToCell(mouseWorldPos);
                gridCell.z = z;
                var tile = _tilemap.GetTile(gridCell);
                if (tile != null)
                {
                    //Debug.Log("grid Z" + gridCell + " found " + tile.name);
                    if (!_tilemap.HasTile(gridCell))
                        break;

                    return new TopCellResult(gridCell, true); // If you need only the first tile encountered
                }
                mouseWorldPos.y += halfCellHeight;
            }

            return new TopCellResult(default, false);
        }
    }
}
