﻿using Roy_T.AStar.Primitives;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Classes.Helpers
{
    public static class VectorUtilities
    {
        public static IReadOnlyCollection<Vector3Int> CardinalDirections = new Vector3Int[]
        {
            Vector3Int.up,
            Vector3Int.right,
            Vector3Int.down,
            Vector3Int.left,
        };

        public static readonly IReadOnlyCollection<Vector3Int> DiagonallDirections = new Vector3Int[]
        {
            Vector3Int.up + Vector3Int.right,
            Vector3Int.up + Vector3Int.left,
            Vector3Int.down + Vector3Int.right,
            Vector3Int.down + Vector3Int.left
        };



        public static IEnumerable<Vector3Int> GetAllCardinalNeighbours(this Vector3Int vector)
        {
            foreach (var direction in CardinalDirections)
                yield return vector + direction;
        }

        public static IEnumerable<Vector3Int> GetAllDiagonalNeighbours(this Vector3Int vector)
        {
            foreach (var direction in DiagonallDirections)
                yield return vector + direction;
        }
        /// <summary>
        /// Checks if given position vector should be considered valid on default tilemap.
        /// </summary>
        /// <param name="vector">Checked position</param>
        /// <returns>If vectors is valid on current tilemap</returns>
        public static bool IsValidVector(this Vector3Int vector)
        {
            return vector.x >= 0 && vector.y >= 0 && vector.z >= 0;
        }

        /// <summary>
        /// Converts <see cref="Vector3Int"/> to <see cref="GridPosition"/>
        /// </summary>
        /// <returns><<see cref="GridPosition"/> with <paramref name="vector"/>'s X and Y values.</returns>
        public static GridPosition ToAstarGridPosition(this Vector3Int vector) => new GridPosition(vector.x, vector.y);

        public static Vector3Int ToUnityVector3Int(this Position position) => new Vector3Int((int)position.X, (int)position.Y);

        /// <summary>
        /// Converts <see cref="Position"/> to Unity's <see cref="Vector3Int"/>, with it's Z coordinate set to <paramref name="z"/>
        /// </summary>
        /// <param name="z">Value of Z coordinate resulting <see cref="Vector3Int"/> should have.</param>
        /// <returns><see cref="Vector3Int"/> with <paramref name="position"/>'s X and Y values. Z value is set to <paramref name="z"/> parameter.</returns>
        public static Vector3Int ToUnityVector3Int(this Position position, int z) => new Vector3Int((int)position.X, (int)position.Y, z);

        /// <summary>
        /// Check if any of the <paramref name="vector"'s component is negative/>
        /// </summary>
        /// <param name="vector"></param>
        /// <returns><see cref="true"/> if any of the components is negative,
        /// <see cref="false"/> if all components are positive</returns>
        public static bool HasNegativeComponent(this Vector3Int vector) => vector.x < 0 || vector.y < 0 || vector.z < 0;
    }
}
