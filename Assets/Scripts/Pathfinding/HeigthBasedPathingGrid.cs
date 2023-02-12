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
    public sealed class HeigthBasedPathingGrid : PathingGridBase
    {
        public HeigthBasedPathingGrid(int x, int y) : base(x, y)
        {
        }

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
                        if(neighbour.Key.HasNegativeComponent())
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
    }
}