using Assets.Scripts.Classes.Helpers;
using Assets.Scripts.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.Pathfinding
{
    public sealed class SingleLayerPathingGrid : PathingGridBase
    {
        public SingleLayerPathingGrid(int x, int y) : base(x, y) { }

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
    }
}
