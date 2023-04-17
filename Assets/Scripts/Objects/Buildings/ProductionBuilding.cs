using Assets.Scripts.Classes.Events;
using Assets.Scripts.Commandables.Directives;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Assets.Scripts.Objects.Buildings
{
    public class ProductionBuilding : BuildingBase
    {
        public event ProgressEvent ProductionProgressed;

        public float ProductionProgress
        {
            get => productionProgress;
            set
            {
                if (!buildingProgressBar.gameObject.activeSelf)
                {
                    buildingProgressBar.GameObject().SetActive(true);
                    buildingProgressBar.SetProgress(0);
                }

                ProductionProgressed?.Invoke(this, new ProgressEventArgs(value));
                productionProgress = value;
            }
        }

        public GameObject ExitPoint => exitPoint;

        [SerializeField]
        private GameObject exitPoint;

        private float productionProgress = 0f;

        protected override void Awake()
        {
            base.Awake();
            AsyncOperationHandle<GameObject> unitAssetHandle = Addressables.LoadAssetAsync<GameObject>("Assets/Prefabs/Units/BaseUnit.prefab");
            unitAssetHandle.Completed += (opHandle) => this.menuActions[0] = new ProductionDirective(opHandle.Result.GetComponent<EntityBase>(), 5.0f);
            ProductionProgressed += buildingProgressBar.RespondToUpdatedProgress;
        }
    }
}
