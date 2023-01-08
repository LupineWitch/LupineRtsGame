using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Classes.Helpers;
using Roy_T.AStar;
using Roy_T.AStar.Grids;
using Roy_T.AStar.Paths;
using Roy_T.AStar.Primitives;
using Roy_T.AStar.Serialization;
using UnityEngine;
using UnityEngine.Tilemaps;
using Grid = Roy_T.AStar.Grids.Grid;
using GridUnity = UnityEngine.Grid;

namespace Assets.Scripts.Pathfinding
{
    public class PathingGrid
    {
        //private readonly PathFinder pathFinder;
        private Grid grid;

        /// <summary>
        /// Initialises new <see cref="PathingGrid"/> instance.
        /// </summary>
        /// <param name="x">The width of pathfidning grid.</param>
        /// <param name="y">The heigth of pathfidning grid.</param>
        public PathingGrid(int x, int y)
        {
            GridSize gridSize = new GridSize(x, y);
            Size cellSize = new Size(Distance.FromMeters(1f), Distance.FromMeters(1f));
            //Initialize basic grid with all cells pathable.
            this.grid = Grid.CreateGridWithLateralAndDiagonalConnections(gridSize, cellSize, Velocity.FromKilometersPerHour(10));
        }

        public void PruneInvalidConnectionsBetweenNodesBasedOnHeigth(Tilemap fromTilemap)
        {
            //assume 0 is tilemap min(as it should)
            for (int x = 0; x < fromTilemap.cellBounds.xMax; x++)
                for (int y = 0; y < fromTilemap.cellBounds.yMax; y++)
                {
                    Vector3Int currentPos = new Vector3Int(x, y, 0);
                    Dictionary<Vector3Int, bool> neighbours = fromTilemap.GetNeighbouringNodes(currentPos);
                    foreach (var neighbour in neighbours)
                    {
                        if (!neighbour.Value)
                        {
                            this.grid.RemoveEdge(currentPos.ToAstarGridPosition(), neighbour.Key.ToAstarGridPosition());
                        }
                    }
                }
        }

        public Queue<Vector3Int> GetFastestPath(Vector3Int start, Vector3Int target)
        {
            var pathFinder = new PathFinder();
            GridPosition astarGridPosStart = start.ToAstarGridPosition();
            GridPosition astarGridPosTarget = target.ToAstarGridPosition();
            Path foundPath = pathFinder.FindPath(astarGridPosStart, astarGridPosTarget, this.grid);
            Queue<Vector3Int> positionsToGoTo = new Queue<Vector3Int>(foundPath.Edges.Count);

            foreach(Roy_T.AStar.Graphs.IEdge edge in foundPath.Edges)
            {
                var cringyTempVar = edge.End.Position.ToUnityVector3Int();
                positionsToGoTo.Enqueue(cringyTempVar);
            }

            return positionsToGoTo;
        }
    }
}