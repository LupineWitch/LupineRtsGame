using Assets.Scripts.Classes.Helpers;
using Roy_T.AStar.Paths;
using Roy_T.AStar.Primitives;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using Grid = Roy_T.AStar.Grids.Grid;

namespace Assets.Scripts.Pathfinding
{
    public abstract class PathingGridBase
    {
        //private readonly PathFinder pathFinder;
        protected Grid grid;

        /// <summary>
        /// Initialises new <see cref="PathingGridBase"/> instance.
        /// </summary>
        /// <param name="x">The width of pathfidning grid.</param>
        /// <param name="y">The heigth of pathfidning grid.</param>
        public PathingGridBase(int x, int y)
        {
            GridSize gridSize = new GridSize(x, y);
            Size cellSize = new Size(Distance.FromMeters(1f), Distance.FromMeters(1f));
            //Initialize basic grid with all cells pathable.
            this.grid = Grid.CreateGridWithLateralAndDiagonalConnections(gridSize, cellSize, Velocity.FromKilometersPerHour(10));
        }

        public virtual Queue<Vector3Int> GetFastestPath(Vector3Int start, Vector3Int target)
        {
            var pathFinder = new PathFinder();
            GridPosition astarGridPosStart = start.ToAstarGridPosition();
            GridPosition astarGridPosTarget = target.ToAstarGridPosition();
            Path foundPath = pathFinder.FindPath(astarGridPosStart, astarGridPosTarget, this.grid);
            Queue<Vector3Int> positionsToGoTo = new Queue<Vector3Int>(foundPath.Edges.Count);

            foreach (Roy_T.AStar.Graphs.IEdge edge in foundPath.Edges)
                positionsToGoTo.Enqueue(edge.End.Position.ToUnityVector3Int());

            return positionsToGoTo;
        }
        public abstract void PruneInvalidConnectionsBetweenNodesBasedOnHeigth(Tilemap fromTilemap);

        public virtual bool CheckIfTwoNodesAreConnected(Vector3Int firstPosition, Vector3Int secondPosition)
        {
            var firstNode = this.grid.GetNode(firstPosition.ToAstarGridPosition());
            var secondNode = this.grid.GetNode(secondPosition.ToAstarGridPosition());

            var outgoingEdge = firstNode.Outgoing.FirstOrDefault(IE => IE.End == secondNode);
            var incomingEdge = secondNode.Incoming.FirstOrDefault(IE => IE.Start == firstNode);

            if (outgoingEdge == null || incomingEdge == null)
                return false;
            if (outgoingEdge != incomingEdge)
                return false;

            return true;
        }
    }
}