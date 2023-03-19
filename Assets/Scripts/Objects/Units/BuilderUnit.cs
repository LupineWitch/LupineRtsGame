using Assets.Scripts.Commandables.Directives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Assets.Scripts.Objects.Units
{
    public class BuilderUnit : BasicUnitScript
    {

        protected override void Awake()
        {
            base.Awake();
            defaultCommand = new ContextDirective();
            var asyncLoadHandle = Addressables.LoadAssetAsync<GameObject>("Assets/Prefabs/Buildings/ProductionBuilding.prefab");
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
