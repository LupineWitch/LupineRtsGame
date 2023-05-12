using Assets.Scripts.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Objects.Buildings
{
    public class BuildingFarm : MultipleSpriteBuilding
    {
        private PlayerResourceManager resourceManager;
        private float passedTime = 0f;
        private float productionCycleTime = 20f;
        private int amountProduced = 10;

        protected override void Awake()
        {
            base.Awake();
            OwnerChanged += BuildingFarm_OwnerChanged;
        }

        private void BuildingFarm_OwnerChanged(object sender, EventArgs e)
        {
            var farm = sender as BuildingFarm;
            if (farm != null)
                resourceManager = farm.Owner.Faction.GetComponentInChildren<PlayerResourceManager>();
        }

        public override void Initialize(int builidngLayer, BoundsInt occupiedBounds, Vector3Int tilePosition)
        {
            base.Initialize(builidngLayer, occupiedBounds, tilePosition);
            StartCoroutine(MakeFoodCoroutine());
        }

        private IEnumerator MakeFoodCoroutine()
        {
            while(true)
            {
                if(passedTime < productionCycleTime)
                    passedTime += Time.deltaTime;
                else
                {
                    resourceManager.ChangeResourceLevel("food", amountProduced);
                    passedTime = 0f;
                }

                yield return null;
            }
        }
    }
}
