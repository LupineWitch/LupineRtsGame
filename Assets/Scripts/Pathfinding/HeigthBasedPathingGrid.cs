using System;
using System.Collections.Generic;
using Assets.Scripts.Classes.Helpers;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.Pathfinding
{
    public sealed class HeigthBasedPathingGrid : PathingGridBase
    {
        public HeigthBasedPathingGrid(int x, int y) : base(x, y)
        {
        }

        public override bool CanTwoNodesConnect(Vector3Int fromNode, Vector3Int toNode) => Math.Abs(fromNode.z - toNode.z) <= 1;

        public override void PruneInvalidConnectionsBetweenNodesBasedOnHeigth(Tilemap fromTilemap)
        {
            //assume 0 is tilemap min(as it should)
            for (int x = 0; x < fromTilemap.cellBounds.xMax; x++)
                for (int y = 0; y < fromTilemap.cellBounds.yMax; y++)
                {
                    Vector3Int currentPos = new Vector3Int(x, y, 0);
                    Dictionary<Vector3Int, int> neighbours = fromTilemap.GetNeighbouringNodes(currentPos);
                    foreach (var neighbour in neighbours)
                    {
                        if (neighbour.Key.HasNegativeComponent())
                        {
                            Debug.LogWarningFormat("Tried to remove an edge from a {0} to a negative position {1}!", currentPos, neighbour.Key);
                            continue;
                        }

                        if (neighbour.Value >= 2) //remove edge to that node
                        {
                            var fromNode = currentPos.ToAstarGridPosition();
                            var toNode = neighbour.Key.ToAstarGridPosition();
                            this.grid.RemoveEdge(fromNode, toNode);
                        }
                    }

                }
        }

        public override void ReaddNodesToPathingGrid(IEnumerable<Vector3Int> worldPositions, Tilemap toTilemap)
        {
            foreach (var pos in worldPositions)
            {
                foreach (var neighbour in toTilemap.GetNeighbouringNodes(pos))
                    if (CanTwoNodesConnect(pos, neighbour.Key))
                        grid.AddEdge(pos.ToAstarGridPosition(), neighbour.Key.ToAstarGridPosition(), this.baseVelocity);
            }
        }
    }
}