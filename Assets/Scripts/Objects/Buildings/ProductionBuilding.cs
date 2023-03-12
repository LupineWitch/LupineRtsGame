using Assets.Scripts.Classes.Events;
using Assets.Scripts.Classes.Static;
using Assets.Scripts.Commandables.Directives;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

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

        new public void Awake()
        {
            base.Awake();
            ProductionProgressed += buildingProgressBar.RespondToUpdatedProgress;
            this.menuActions[0] = new ProductionDirective(Resources.Load<EntityBase>(Path.Combine(ResourceNames.TestPrefabsPath, ResourceNames.TestUnit)), 5.0f);
        }
    }
}
