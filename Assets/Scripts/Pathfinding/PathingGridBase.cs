using Assets.Scripts.Classes.Helpers;
using Roy_T.AStar.Paths;
using Roy_T.AStar.Primitives;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEngine.GraphicsBuffer;
using Grid = Roy_T.AStar.Grids.Grid;

namespace Assets.Scripts.Pathfinding
{
    public abstract class PathingGridBase
    {
        //private readonly PathFinder pathFinder;
        protected Grid grid;

        protected Velocity baseVelocity = Velocity.FromKilometersPerHour(10);

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
            if (start.HasNegativeComponent() || target.HasNegativeComponent())
                return null;

            int cachedZ = start.z;
            var pathFinder = new PathFinder();
            GridPosition astarGridPosStart = start.ToAstarGridPosition();
            GridPosition astarGridPosTarget = target.ToAstarGridPosition();
            Path foundPath = pathFinder.FindPath(astarGridPosStart, astarGridPosTarget, this.grid);

            if (foundPath.Edges.Count < 0)
                return null;

            Queue<Vector3Int> positionsToGoTo = new Queue<Vector3Int>(foundPath.Edges.Count + 1);
            foreach (Roy_T.AStar.Graphs.IEdge edge in foundPath.Edges)
                positionsToGoTo.Enqueue(edge.End.Position.ToUnityVector3Int(cachedZ));

            return positionsToGoTo;
        }


        public virtual bool PathExistsBetweenNodes(Vector3Int from, Vector3Int to)
        {
            if (from.HasNegativeComponent() || to.HasNegativeComponent())
                return false;

            var pathFinder = new PathFinder();
            GridPosition astarGridPosStart = from.ToAstarGridPosition();
            GridPosition astarGridPosTarget = to.ToAstarGridPosition();
            Path foundPath = pathFinder.FindPath(astarGridPosStart, astarGridPosTarget, this.grid);

            if (foundPath.Type == PathType.ClosestApproach || foundPath.Edges.Count == 0)
                return false;
            else
                return true;

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

        public virtual void RemoveNodesFromPathingGrid(IEnumerable<Vector3Int> gridPositions)
        {
            foreach (var pos in gridPositions)
            {
                grid.RemoveDiagonalConnectionsIntersectingWithNode(pos.ToAstarGridPosition());
                grid.DisconnectNode(pos.ToAstarGridPosition());
            }
        }

        public abstract void ReaddNodesToPathingGrid(IEnumerable<Vector3Int> nodes, Tilemap toTilemap);

        public abstract bool CanTwoNodesConnect(Vector3Int fromNode, Vector3Int toNode);
        
        public override string ToString()
        {
            string connectedLeft = "|##<##|";
            string connectedRight = "|##>##|";
            string connectedDown = "|--^--|";
            string connectedUp = "|__V__|";

            string connectedDownLeft = "|__\\__|";
            string connectedDownRight = "|__/__|";
            string connectedUpLeft = "|--/--|";
            string connectedUpRight = "|--\\--|";

            string[,] cells = new string[3 * grid.Rows, 3 * grid.Columns];

            for (int i = 0; i < cells.GetLength(0); i++)
                for (int j = 0; j < cells.GetLength(1); j++)
                    cells[i,j] = "|XX-XX|";

            int textY = 0; 
            for (int y = 0; y < grid.Rows; y++)
            {
                int textX = 0;
                for (int x = 0; x < grid.Columns; x++)
                {
                    var node = grid.GetNode(new GridPosition(x, y));
                    cells[textY + 1, textX + 1] = string.Format("{0, 3:D3};{1, 3:D3}",x, y);
                    foreach (var edge in node.Outgoing)
                    {
                        if (edge.End.Position.X > node.Position.X)
                        {
                            if (edge.End.Position.Y > node.Position.Y)
                                cells[textY + 2, textX + 2] = connectedUpRight;
                            else if (edge.End.Position.Y == node.Position.Y)
                                cells[textY + 1, textX + 2] = connectedRight;
                            else if (edge.End.Position.Y < node.Position.Y)
                                cells[textY + 0, textX + 2] = connectedDownRight;
                        }
                        else if (edge.End.Position.X < node.Position.X)
                        {
                            if (edge.End.Position.Y < node.Position.Y)
                                cells[textY + 0, textX + 0] = connectedDownLeft;
                            else if (edge.End.Position.Y == node.Position.Y)
                                cells[textY + 1, textX + 0] = connectedLeft;
                            else if (edge.End.Position.Y > node.Position.Y)
                                cells[textY + 2, textX + 0] = connectedUpLeft;
                        }
                        else if (edge.End.Position.X == node.Position.X)
                        {
                            if (edge.End.Position.Y < node.Position.Y)
                                cells[textY + 0, textX + 1] = connectedDown;
                            else if (edge.End.Position.Y > node.Position.Y)
                                cells[textY + 2, textX + 1] = connectedUp;
                        }
                    }
                    textX += 3;
                }
                textY += 3;
            }

            StringBuilder gridTextbuilder = new StringBuilder();
            for(int y = 0; y < cells.GetLength(0); y++)
            {
                for(int x = 0; x < cells.GetLength(1); x++)
                    gridTextbuilder.Append(cells[y,x]);

                gridTextbuilder.Append('\n');
            }
            return gridTextbuilder.ToString();
        }
    }
}