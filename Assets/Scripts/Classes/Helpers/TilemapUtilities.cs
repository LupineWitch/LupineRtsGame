using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
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

        /// <summary>
        /// Finds the top tile with specified position in the given tilemap.
        /// </summary>
        /// <param name="position">Position to find the top tile in</param>
        /// <returns>The position of the top tile</returns>
        public static Vector3Int GetTopTilePosition(this Tilemap tilemap, Vector3Int position)
        {
            Vector3Int topTile = position;
            Vector3Int tempTile = topTile;

            for(int z = 0; z < tilemap.cellBounds.zMax; z++)
            {
                tempTile.z = z;
                if(tilemap.HasTile(tempTile))
                    topTile = tempTile;
            }

            return topTile;
        }

        /// <summary>
        /// Gets the dictionary of (topmost) neighbouring tiles and if they're directly pathable from given <paramref name="position"/>
        /// </summary>
        /// <param name="position">Position of tiles to find neighbours of</param>
        /// <returns>Dictionary of tiles and if there are directly pathable to.</returns>
        public static Dictionary<Vector3Int, bool> GetNeighbouringNodes(this Tilemap tilemap, Vector3Int position)
        {
            Dictionary<Vector3Int, bool> pathableNeigbours = new Dictionary<Vector3Int, bool>(); 
            Vector3Int topTilePosition = tilemap.GetTopTilePosition(position);
            bool isPathableTo = true;
            foreach(Vector3Int direction in NeighbourDirections)
            {
                Vector3Int currentNeighbour = tilemap.GetTopTilePosition(topTilePosition + direction);
                int heigthDiff = currentNeighbour.z - topTilePosition.z;
                if (heigthDiff != 0)
                {
                    Debug.Log($"Difference: {heigthDiff}");
                    Debug.Log($"Abs of difference: {Math.Abs(heigthDiff)}, smaller bigger or equal to two: {Math.Abs(heigthDiff) >= 2}");
                }

                if(Math.Abs(heigthDiff) >= 2)
                {
                    Debug.Log("Pathable set to false!");
                    isPathableTo = false;
                }

                if(!isPathableTo)
                    Debug.Log($"Should Remove edgde betweeen: {currentNeighbour} and {topTilePosition}");
                
                if(isPathableTo && heigthDiff != 0)
                    Debug.Log($"Should Remove edgde betweeen: {currentNeighbour} and {topTilePosition}, but something went wrong... height diff: {heigthDiff}");

                pathableNeigbours.Add(currentNeighbour, isPathableTo);
            }

            return pathableNeigbours;
        }
    }
}
