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
            position.z = 0;

            while (tilemap.HasTile(position))
                position += Vector3Int.forward;

            return (position - Vector3Int.back);
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
            bool isPathableTo = false;
            foreach(Vector3Int direction in NeighbourDirections)
            {
                Vector3Int currentNeighbour = topTilePosition + direction;
                if (currentNeighbour.IsValidVector())
                    if (Math.Abs((currentNeighbour - topTilePosition).z) < 2)
                        isPathableTo = true;

                pathableNeigbours.Add(currentNeighbour, isPathableTo);
            }

            return pathableNeigbours;
        }
    }
}
