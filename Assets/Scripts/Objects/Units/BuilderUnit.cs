using Assets.Scripts.Commandables.Directives;
using UnityEngine.AddressableAssets;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using Assets.Scripts.Objects.Buildings;

namespace Assets.Scripts.Objects.Units
{
    public class BuilderUnit : BasicUnitScript
    {

        protected override void Awake()
        {
            base.Awake();
            defaultDirective = new ContextDirective();
            var asyncLoadHandle = Addressables.LoadAssetAsync<GameObject>("Assets/Prefabs/Buildings/Farmhouse/Farmhouse.prefab");
            asyncLoadHandle.Completed += AsyncLoadHandle_Completed;
            Addressables.LoadAssetAsync<GameObject>("Assets/Prefabs/Buildings/StorageHut.prefab").Completed += (loadHandle) =>
            {
                ResourceBuilding storage = loadHandle.Result.GetComponent<ResourceBuilding>();
                menuActions[2] = new BuildDirective(storage);
            };
            Addressables.LoadAssetAsync<GameObject>("Assets/Prefabs/Buildings/Barracks.prefab").Completed += (loadHandle) =>
            {
                BarracksBuilding barracks = loadHandle.Result.GetComponent<BarracksBuilding>();
                menuActions[3] = new BuildDirective(barracks);
            };
            DisplayLabel = "Builder";
        }

        private void AsyncLoadHandle_Completed(AsyncOperationHandle<GameObject> loadHandle)
        {
            BuildingBase buildingBase = loadHandle.Result.GetComponent<BuildingBase>();
            menuActions[0] = new MoveDirective();
            menuActions[1] = new BuildDirective(buildingBase);
        }
    }
}
