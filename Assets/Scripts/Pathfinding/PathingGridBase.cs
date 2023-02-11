using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Grid = Roy_T.AStar.Grids.Grid;

namespace Assets.Scripts.Pathfinding
{
    public abstract class PathingGridBase
    {
        //private readonly PathFinder pathFinder;
        protected Grid grid;

        public abstract Queue<Vector3Int> GetFastestPath(Vector3Int start, Vector3Int target);
        public abstract void PruneInvalidConnectionsBetweenNodesBasedOnHeigth(Tilemap fromTilemap);
    }
}