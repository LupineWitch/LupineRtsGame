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
using Assets.Scripts.Classes.Events;

namespace Assets.Scripts.Managers
{
    public class MapManager : MonoBehaviour
    {
        public PathingGridBase PathingGrid { get { return pathingGrid; } }
        public Tilemap MainTilemap { get { return mainTilemap; } }
        public float CellSize { get => cellSize; }

        [SerializeField]
        private Tilemap mainTilemap;
        [SerializeField]
        private LoadMapPersistenceData loadedMapData;
        [SerializeField]
        private bool loadMapFromFile = true;

        private PathingGridBase pathingGrid;
        private float cellSize;


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

            cellSize = mainTilemap.cellSize.magnitude;
        }

        public void BuildingCreatedCallback(object sender, BuildingEventArgs args)
        {
            List<Vector3Int> occupiedPositions = GetOverlappingWorldPointsForBuilding(sender, args);

            pathingGrid.RemoveNodesFromPathingGrid(occupiedPositions);
        }

        private List<Vector3Int> GetOverlappingWorldPointsForBuilding(object sender, BuildingEventArgs args)
        {
            BuildingBase building = sender as BuildingBase;
            List<Vector3Int> occupiedPositions = new List<Vector3Int>();
            foreach (Vector3Int pos in args.OccupiedBounds.allPositionsWithin)
            {
                var cellCenter = mainTilemap.GetCellCenterWorld(pos);
                var distanceToCollider = Vector2.Distance(building.Collider.ClosestPoint(cellCenter), cellCenter);
                if (building.Collider.OverlapPoint(cellCenter) || distanceToCollider <= cellSize)
                    occupiedPositions.Add(new Vector3Int(pos.x, pos.y, building.BuildingLayer));
            }

            return occupiedPositions;
        }

        public void BuildingDestroyedCallback(object sender, BuildingEventArgs args) => 
            pathingGrid.ReaddNodesToPathingGrid(GetOverlappingWorldPointsForBuilding(sender, args), mainTilemap);
    
        public Vector3Int TransformToCellPosition(Transform transform) =>  mainTilemap.WorldToCell(transform.position);
        
    }
}
