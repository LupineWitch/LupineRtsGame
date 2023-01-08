using Assets.Scripts.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Tilemaps;
using UnityEngine;

namespace Assets.Scripts.Classes.Helpers
{
    internal class CursorTileSelector : ITopCellSelector
    {
        //Tweak based on you tile origin; constant offset applied to mouse coordinates
        public Vector3 mouseOffset = new Vector3(0, -0.5f, 0);

        //y difference for each 1 unit of z; My tiles were .25 units tall
        public float ydelta = -0.5f;
        //0 to zMax are iterated through when selecting a tile
        private int zMax;
        private Tilemap tilemap;

        public CursorTileSelector(Tilemap mainTilemap)
        {
            tilemap = mainTilemap;
            zMax = tilemap.cellBounds.zMax;
        }


        public Vector3Int GetTopCell(Vector2 mouseWorldPos)
        {

            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cell = tilemap.WorldToCell(mousePos);

            //Iterate through tile layers starting at the top
            for (int i = zMax; i >= 0; i--)
            {
                //Adjust mouse world coordinate to include z (i)
                Vector3 newCell = new Vector3(mousePos.x, mousePos.y + i * ydelta, i);
                newCell += mouseOffset;
                //check cell
                cell = tilemap.WorldToCell(newCell);
                if (tilemap.HasTile(cell))
                {
                    cell = IterateToTopCell(cell);
                    return cell;
                }
            }
            return cell;
        }

        /// <summary>
        /// Get a cell that is not covered by any tiles
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        private Vector3Int IterateToTopCell(Vector3Int cell)
        {
            //default to given cell
            Vector3Int newCell = cell;
            //iterate from given cell -> upwards
            for (int i = cell.z; i <= zMax; i++)
            {
                Vector3Int _cell = new Vector3Int(cell.x, cell.y, i);
                if (tilemap.HasTile(_cell))
                    newCell = _cell;
                else
                    break;
            }
            return newCell;
        }
    }
}
