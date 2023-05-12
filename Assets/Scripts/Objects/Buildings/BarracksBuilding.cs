using Assets.Scripts.Classes.Events;
using Assets.Scripts.Commandables.Directives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.Objects.Buildings
{
    public class BarracksBuilding : BuildingBase
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
        private float productionProgress = 0f;

        [SerializeField]
        private GameObject exitPoint;

        [SerializeField]
        private BasicUnitScript[] unitsToProduce = new BasicUnitScript[9];

        protected override void Awake()
        {
            base.Awake();
            int i = 0;
            foreach (var unitPrefab in unitsToProduce.Where(u => u != null ))
            {
                this.menuActions[i] = new ProductionDirective(unitPrefab, 5.0f);
                i++;
            }
            ProductionProgressed += buildingProgressBar.RespondToUpdatedProgress;
        }

    }
}
