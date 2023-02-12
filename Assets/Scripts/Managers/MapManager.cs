using Assets.Scripts.Pathfinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;
using Assets.Scripts.Classes.Models.Level;
using Assets.Scripts.Classes.DPL;
using Assets.Scripts.Classes.Serialisers;

namespace Assets.Scripts.Managers
{
    public class MapManager : MonoBehaviour
    {
        public PathingGridBase PathingGrid { get { return pathingGrid; } }
        public Tilemap UsedTilemap { get { return mainTilemap; } }

        [SerializeField]
        private Tilemap mainTilemap;
        [SerializeField]
        private LoadMapPersistenceData loadedMapData;
        [SerializeField]
        private bool loadMapFromFile = true;

        private PathingGridBase pathingGrid;

        private void Awake()
        {
            //load map from the model
            if(loadMapFromFile)
            { 
                IMapSerialiser mapDeserialiser = new JsonMapSerialiser();
                mainTilemap.ClearAllTiles();
                mapDeserialiser.DeserialiseMapModelToTilemap(this.mainTilemap, this.loadedMapData.LoadedMapModel);
            }
            //Generate path based on map
            pathingGrid = new SingleLayerPathingGrid(mainTilemap.cellBounds.xMax, mainTilemap.cellBounds.yMax);
            pathingGrid.PruneInvalidConnectionsBetweenNodesBasedOnHeigth(mainTilemap);
        }

        private void Start()
        {
            //eeh..
        }
    }
}
