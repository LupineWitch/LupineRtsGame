using Assets.Scripts.Classes.Helpers;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.Pathfinding
{
    public sealed class SingleLayerPathingGrid : PathingGridBase
    {
        public SingleLayerPathingGrid(int x, int y) : base(x, y) { }

        public override bool CanTwoNodesConnect(Vector3Int fromNode, Vector3Int toNode) => fromNode.z == 2 && toNode.z == 2;

        public override void PruneInvalidConnectionsBetweenNodesBasedOnHeigth(Tilemap fromTilemap)
        {
            for (int x = 0; x < fromTilemap.cellBounds.xMax; x++)
                for (int y = 0; y < fromTilemap.cellBounds.yMax; y++)
                {
                    Vector3Int currentPos = new Vector3Int(x, y, 0);
                    currentPos = fromTilemap.GetTopTilePosition(currentPos);
                    if (currentPos.z == 2)
                        continue; //We dont need to remove connections to that node
                    else
                    {
                        this.grid.DisconnectNode(currentPos.ToAstarGridPosition());
                        this.grid.RemoveDiagonalConnectionsIntersectingWithNode(currentPos.ToAstarGridPosition());
                    }
                }
        }

        public override void ReaddNodesToPathingGrid(IEnumerable<Vector3Int> positions, Tilemap toTilemap)
        {
            foreach (var pos in positions)
            {
                foreach (var neighbour in toTilemap.GetNeighbouringNodes(pos))
                    if (neighbour.Value == 0 && pos.z == 2)
                        grid.AddEdge(pos.ToAstarGridPosition(), neighbour.Key.ToAstarGridPosition(), this.baseVelocity);

            }
        }
    }
}
