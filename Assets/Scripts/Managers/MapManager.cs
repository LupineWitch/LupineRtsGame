using Assets.Scripts.Pathfinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;
using Assets.Scripts.Classes.Models.Level;

namespace Assets.Scripts.Managers
{
    public class MapManager : MonoBehaviour
    {
        public PathingGrid PathingGrid { get { return pathingGrid; } }
        public Tilemap UsedTilemap { get { return mainTilemap; } }

        private PathingGrid pathingGrid;
        private LevelLoader mapLoader;
        [SerializeField]
        private Tilemap mainTilemap;

        private void Awake()
        {
            mapLoader = new LevelLoader(mainTilemap);
            mapLoader.LoadMap();
            pathingGrid = new PathingGrid(mainTilemap.cellBounds.xMax, mainTilemap.cellBounds.yMax);
            pathingGrid.PruneInvalidConnectionsBetweenNodesBasedOnHeigth(mainTilemap);
        }

        private void Start()
        {
            //eeh..
        }
    }
}
