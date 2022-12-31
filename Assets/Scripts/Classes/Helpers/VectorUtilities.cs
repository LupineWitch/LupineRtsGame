using Roy_T.AStar.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Classes.Helpers
{
    public static class VectorUtilities
    {
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
        public static GridPosition ToAstarGridPosition(this Vector3Int vector) =>  new GridPosition(vector.x, vector.y);

        public static Vector3Int ToUnityVector3Int(this Position position) => new Vector3Int((int)position.X, (int)position.Y);
    }
}
