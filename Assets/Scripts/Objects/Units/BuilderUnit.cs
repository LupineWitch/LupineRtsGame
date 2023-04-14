using Assets.Scripts.Commandables.Directives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            Addressables.LoadAssetAsync<GameObject>("Assets/Prefabs/Buildings/LumberHut.prefab").Completed += (loadHandle) =>
            {
                ResourceBuilding storage = loadHandle.Result.GetComponent<ResourceBuilding>();
                menuActions[2] = new BuildDirective(storage);
            };
            asyncLoadHandle.Completed += AsyncLoadHandle_Completed;
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
