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
using System.IO;

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
            string logFilePath = Path.Combine(Environment.GetFolderPath(
                                                      Environment.SpecialFolder.LocalApplicationData),
                                                      "LupineLogs",
                                                      "PathingGrid.log");
            Directory.CreateDirectory(Path.GetDirectoryName(logFilePath));
            StreamWriter fileHandle = File.CreateText(logFilePath);
            Debug.LogFormat("Log path: {0}", logFilePath);
            string temp = pathingGrid.ToString();
            fileHandle.Write(temp);
            fileHandle.Close();
            fileHandle.Dispose();
        }

        private void Start()
        {
            //Shade each cell layer

        }
    }
}
