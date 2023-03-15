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

        private void Awake()
        {
            defaultCommand = new ContextDirective();
            var asyncLoadHandle = Addressables.LoadAssetAsync<BuildingBase>("Assets/Prefabs/Buildings/ProductionBuilding.prefab");
            asyncLoadHandle.Completed += AsyncLoadHandle_Completed;
        }

        private void AsyncLoadHandle_Completed(AsyncOperationHandle<BuildingBase> obj)
        {
            menuActions[0] = new MoveDirective();
            menuActions[1] = new BuildDirective(obj.Result);
        }
    }
}
