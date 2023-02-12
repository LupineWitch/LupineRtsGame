using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.Classes.Helpers
{
    public static class TilemapUtilities
    {
        private static readonly Vector3Int[] NeighbourDirections =
        {
            Vector3Int.up,
            Vector3Int.right,
            Vector3Int.down,
            Vector3Int.left,
            //diagonal neighbours
            Vector3Int.up + Vector3Int.right,
            Vector3Int.up + Vector3Int.left,
            Vector3Int.down + Vector3Int.right,
            Vector3Int.down + Vector3Int.left
        };

        public static ReadOnlyArray<Vector3Int> GetNeighbourDirections { get => NeighbourDirections; }

        /// <summary>
        /// Finds the top tile with specified position in the given tilemap.
        /// </summary>
        /// <param name="position">Position to find the top tile in</param>
        /// <returns>The position of the top tile</returns>
        public static Vector3Int GetTopTilePosition(this Tilemap tilemap, Vector3Int position)
        {
            Vector3Int topTile = position;
            Vector3Int tempTile = topTile;

            for(int z = 0; z <= tilemap.cellBounds.zMax; z++)
            {
                tempTile.z = z;
                if(tilemap.HasTile(tempTile))
                    topTile = tempTile;
            }

            return topTile;
        }

        /// <summary>
        /// Gets the dictionary of (topmost) neighbouring tiles and if heigth difference betwween them and given <paramref name="position"/>
        /// </summary>
        /// <param name="position">Position of tiles to find neighbours of</param>
        /// <returns>Dictionary of heigth differences between  <paramref name="position"/> and its neighbours, keyed by neighbour.</returns>
        public static Dictionary<Vector3Int, int> GetNeighbouringNodes(this Tilemap tilemap, Vector3Int position)
        {
            Dictionary<Vector3Int, int> neighbours = new Dictionary<Vector3Int, int>(); 
           Vector3Int topTilePosition = tilemap.GetTopTilePosition(position);

            foreach(Vector3Int direction in NeighbourDirections)
            {
                Vector3Int currentNeighbour = tilemap.GetTopTilePosition(position + direction);
                //is this tile in the expected bounds??
                if (!tilemap.cellBounds.Contains(currentNeighbour))
                    continue;                    

                int heigthDiff = currentNeighbour.z - topTilePosition.z;
                neighbours.Add(currentNeighbour, Math.Abs(heigthDiff));
            }
            return neighbours;
        }
    }
}
