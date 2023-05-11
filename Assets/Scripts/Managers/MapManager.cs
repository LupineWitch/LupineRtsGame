using Assets.Scripts.Pathfinding;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Assets.Scripts.Classes.DPL;
using Assets.Scripts.Classes.Serialisers;
using System.IO;
using Assets.Scripts.Classes.Events;
using Assets.Scripts.Classes.Models.Entity;
using UnityEngine.Video;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Linq;
using System.Threading.Tasks;
using Mono.Cecil.Cil;
using Assets.Scripts.Objects.ResourceNodes;
using Assets.Scripts.Classes.Models.Level;
using Assets.Scripts.Faction;
using Assets.Scripts.Classes.Helpers;
using Assets.Scripts.Classes.Models.Level.Map;
using System.Collections;

namespace Assets.Scripts.Managers
{
    public class MapManager : MonoBehaviour
    {
        public PathingGridBase PathingGrid { get { return pathingGrid; } }
        public Tilemap MainTilemap { get { return mainTilemap; } }
        public float CellSize { get => cellSize; }
        public event Action TilemapLoaded;

        private const string Faction_Prefabs_Path = "Assets/Prefabs/SceneHierarchy/Faction";

        [SerializeField]
        private Tilemap mainTilemap;
        [SerializeField]
        private LoadMapPersistenceData loadedMapData;
        [SerializeField]
        private bool loadMapFromFile = true;
        [SerializeField]
        private AvailableBuildingSpaceManager buildingSpaceManager;
        [SerializeField]
        private GameObject UnitsContainer;
        [SerializeField]
        private GameObject AmbientContainer;
        [SerializeField]
        private GameObject BuildingsContainer;

        private PathingGridBase pathingGrid;
        private float cellSize;
        private bool initialised = false;
        private Dictionary<string, List<EntityBase>> factionEntities;

        protected virtual void Awake()
        {
            //load map from the model
            if (loadMapFromFile)
            {
                IMapSerialiser mapDeserialiser = new JsonMapSerialiser();
                mainTilemap.ClearAllTiles();
                mapDeserialiser.DeserialiseMapModelToTilemap(this.mainTilemap, this.loadedMapData.LoadedMapModel, this);
            }
            //Generate path based on map
            pathingGrid = new SingleLayerPathingGrid(mainTilemap.cellBounds.xMax, mainTilemap.cellBounds.yMax);
            pathingGrid.PruneInvalidConnectionsBetweenNodesBasedOnHeigth(mainTilemap);
            cellSize = mainTilemap.cellSize.magnitude;
        }

        public void BuildingCreatedCallback(object sender, BuildingEventArgs args)
        {
            List<Vector3Int> occupiedPositions = GetOverlappingWorldPointsForBuilding(sender, args);

            pathingGrid.RemoveNodesFromPathingGrid(occupiedPositions);
            buildingSpaceManager.RemoveCellsFromBuildingGrid(args.OccupiedBounds.Value.allPositionsWithin.GetEnumerator());
        }

        public void BuildingDestroyedCallback(object sender, BuildingEventArgs args)
        {
            List<Vector3Int> occupiedPositions = GetOverlappingWorldPointsForBuilding(sender, args);
            pathingGrid.ReaddNodesToPathingGrid(occupiedPositions, mainTilemap);
            buildingSpaceManager.AddCellsToBuildingGrid(args.OccupiedBounds.Value.allPositionsWithin.GetEnumerator());
        }

        public Vector3Int TransformToCellPosition(Transform transform) => mainTilemap.WorldToCell(transform.position);

        public List<SerializedEntityBase> GetEntitiesToSerialise()
        {
            Transform[] entitiesParents = new Transform[]
            {
                BuildingsContainer.transform, AmbientContainer.transform, UnitsContainer.transform
            };
            List<SerializedEntityBase> entitiesToSerialize = new();

            foreach(Transform parent in entitiesParents)
            {
                foreach (Transform gameObjTransfomr in parent.transform)
                {
                    GameObject gameObj = gameObjTransfomr.gameObject;
                    string prefabName = string.Empty;
                    if (gameObj.TryGetComponent(out EntityBase entityComp))
                        prefabName = entityComp.PrefabName;
                    else if (gameObj.TryGetComponent(out ResourceNodeTree resourceNode))
                        prefabName = resourceNode.PrefabName;


                    var entity = new SerializedEntityBase
                    {
                        Position = gameObj.transform.position,
                        PrefabName = prefabName,
                        ParentName = parent.name,
                    };

                    if (entityComp != null && entityComp.Owner != null)
                        entity.FactionName = entityComp.Owner.Faction.FactionName;

                    entity.Components.AddRange(gameObj.GetComponents<ISerializableEntityComponent>());
                    entitiesToSerialize.Add(entity);
                }                
            }
            return entitiesToSerialize;
        }
    
        public async void DeserialiseInMapManager(MapModel model)
        {
            GameObject[] loadedAssets = await LoadPrefabsFromAddress(@"Assets/Prefabs");
            this.factionEntities = DeserialiseGivenEntities(model.MapEntities, loadedAssets);
            SetStartingPositions(model.StartingPositions, loadedAssets);
        }

        public List<StartingConditionsModel> GetSerialisableStartingPositions()
        {
            var factionContainer = this.transform.parent.Find("Factions");
            List<StartingConditionsModel> startingPositions = new();

            foreach(var faction in factionContainer.GetComponentsInChildren<BaseFaction>())
            {
                var startingManager = faction.GetComponentInChildren<StartingConditionsManager>();
                var startModel = new StartingConditionsModel
                {
                    StartingBuildingPrefabName = startingManager.StartingBuildingPrefab.PrefabName,
                    OrginalStartingFactionPrefabName = startingManager.transform.parent.gameObject.name,
                    OrginalStartingFactionName = startingManager.transform.parent.GetComponentInChildren<CommandControllerBase>().Faction.FactionName,
                    StartingResources = startingManager.MergeResourceListsIntoDictionary(),
                    StartingPosition = startingManager.gameObject.transform.position
                };
                startingPositions.Add(startModel);
            }

            return startingPositions;
        }

        private List<Vector3Int> GetOverlappingWorldPointsForBuilding(object sender, BuildingEventArgs args)
        {
            BuildingBase building = sender as BuildingBase;
            List<Vector3Int> occupiedPositions = new();
            foreach (Vector3Int pos in args.OccupiedBounds?.allPositionsWithin)
            {
                var cellCenter = mainTilemap.GetCellCenterWorld(pos);
                var distanceToCollider = Vector2.Distance(building.Collider.ClosestPoint(cellCenter), cellCenter);
                if (building.Collider.OverlapPoint(cellCenter) || distanceToCollider <= cellSize)
                    occupiedPositions.Add(new Vector3Int(pos.x, pos.y, building.BuildingLayer));
            }

            return occupiedPositions;
        }

        private Dictionary<string, List<EntityBase>> DeserialiseGivenEntities(IEnumerable<SerializedEntityBase> deserialisableEntities, GameObject[] loadedAssets)
        {
            Transform entitiesContainer = this.gameObject.transform.Find("Entities");
            if(entitiesContainer == null)
            {
                Debug.LogError($"Can't find \"Entities\" Game Object in the 'Map Manger>Entities' hierarchy!!");
                return null;
            }

            Dictionary<string, List<EntityBase>> entitiesInFaction = new();
            foreach(var entity in deserialisableEntities)
            {
                GameObject entityPrefab = default;

                if (string.IsNullOrEmpty(entity.PrefabName))
                {
                    var entityComp = entity.Components.FirstOrDefault(comp => comp is EntityBase);
                    if (entityComp == default)
                    {
                        Debug.LogWarning($"Can't find prefab for {entity}");
                        continue;
                    }

                    entityPrefab = loadedAssets.FirstOrDefault(prefab =>
                    {
                        if (prefab.TryGetComponent<EntityBase>(out EntityBase prefabEntityComp))
                            return prefabEntityComp.GetType() == entityComp.GetType();
                        else
                            return false;

                    });
                }
                else
                    entityPrefab = loadedAssets.FirstOrDefault(prefab => prefab.name == entity.PrefabName);

                if (entityPrefab == default)
                {
                    Debug.LogWarning($"Can't find prefab for {entity}");
                    continue;
                }

                GameObject instantiatedPrefab = Instantiate(entityPrefab, entitiesContainer.Find(entity.ParentName).transform);
                instantiatedPrefab.transform.position = entity.Position;
                if (instantiatedPrefab.TryGetComponent(out EntityBase instantiatedEntity))
                {
                    if (!entitiesInFaction.ContainsKey(entity.FactionName))
                        entitiesInFaction.Add(entity.FactionName, new List<EntityBase>());

                    entitiesInFaction[entity.FactionName].Add(instantiatedEntity);
                }
            }

            return entitiesInFaction;
        }

        private async Task<GameObject[]> LoadPrefabsFromAddress(string prefabDirectoryPath)
        {
            string[] prefabFilePaths = Directory.GetFiles(prefabDirectoryPath, "*.prefab", SearchOption.AllDirectories);

            // Load all prefabs asynchronously using Addressables
            List<AsyncOperationHandle<GameObject>> loadOperationHandles = new();
            foreach (string filePath in prefabFilePaths)
            {
                var loadOperationHandle = Addressables.LoadAssetAsync<GameObject>(filePath.Replace('\\','/')); //normalize windows path format
                loadOperationHandles.Add(loadOperationHandle);
            }

            await Task.WhenAll(loadOperationHandles.Select(handle => handle.Task));

            // Collect all loaded prefabs into a list
            var loadedPrefabs = new List<GameObject>(loadOperationHandles.Count);
            foreach (var loadOperationHandle in loadOperationHandles)
            {
                if (loadOperationHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    loadedPrefabs.Add(loadOperationHandle.Result);
                }
            }

            // Release the load operation handles
            foreach (var loadOperationHandle in loadOperationHandles)
            {
                Addressables.Release(loadOperationHandle);
            }

            return loadedPrefabs.ToArray();
        }

        private async void SetStartingPositions(List<StartingConditionsModel> startingPositions, GameObject[] buildingPrefabs)
        {
            var factionContainer = this.transform.parent.Find("Factions");
            var loadedFactionPrefabs = await this.LoadPrefabsFromAddress(Faction_Prefabs_Path);

            foreach (var conditionsModel in startingPositions)
            {
                var factionPrefab = loadedFactionPrefabs.FirstOrDefault(prefab => prefab.name == conditionsModel.OrginalStartingFactionPrefabName);
                var factionInstance = Instantiate(factionPrefab, Vector3.zero, Quaternion.identity, factionContainer.transform);
                var startManager = factionInstance.GetComponentInChildren<StartingConditionsManager>();
                var buildingManager = factionInstance.GetComponentInChildren<BuildingManager>();
                startManager.StartingResources = conditionsModel.StartingResources;
                startManager.BuildingsParent = BuildingsContainer;
                Debug.LogWarning($"Starting position: {conditionsModel.StartingPosition}");
                startManager.transform.position = conditionsModel.StartingPosition;
                var foundPrefab = buildingPrefabs.FirstOrDefault(prefab => prefab.name == conditionsModel.StartingBuildingPrefabName);
                if (foundPrefab == default)
                    Debug.LogWarning($"Haven't found prefab {conditionsModel.StartingBuildingPrefabName}");

                startManager.StartingBuildingPrefab = foundPrefab == null ? null : foundPrefab.GetComponent<BuildingBase>();
            }
        }

        private void AddFactionsToDeserialisedElements(Dictionary<string, List<EntityBase>> factionEntities)
        {
            GameObject factionsContainer = this.GetReferenceManagerInScene().FactionContainer;
            var allLoadedFactions = factionsContainer.GetComponentsInChildren<BaseFaction>();
            if(allLoadedFactions.Length <= 0)
                return;

            foreach (var factionKeyEntitiesPair in factionEntities)
            {
                BaseFaction faction = allLoadedFactions.FirstOrDefault(f => f.FactionName.Equals(factionKeyEntitiesPair.Key, StringComparison.OrdinalIgnoreCase));
                if (faction == null)
                {
                    Debug.LogWarning($"Can't find faction named: {factionKeyEntitiesPair.Key}");
                    continue;
                }
                var commander = faction.GetComponentInChildren<CommandControllerBase>();
                if (commander == null)
                {
                    Debug.LogError($"Commander null here! Faction: {factionKeyEntitiesPair.Key}");
                    continue;
                }

                foreach (var entity in factionKeyEntitiesPair.Value)
                    entity.ChangeOwner(commander);
            }

            this.initialised = true;
        }
    
        protected virtual void LateUpdate()
        {
            if (!initialised && factionEntities != default)
                AddFactionsToDeserialisedElements(factionEntities);
        }
    }
}
