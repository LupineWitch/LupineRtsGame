﻿using Assets.Scripts.Classes.Commands;
using Assets.Scripts.Controllers;
using Assets.Scripts.Objects.Buildings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Commandables.Directives
{
    public class ProductionDirective : ImmediateDirective
    {
        private EntityBase unitPrefab;
        private float buildTime = 5.0f;

        public ProductionDirective(EntityBase unitPrefab, float buildTime)
        {
            this.unitPrefab = unitPrefab;
            this.buildTime = buildTime;
        }

        public override void ExecuteImmediately(BasicCommandControler commander, List<ISelectable> selectedObjects)
        {
            foreach (ISelectable selectedObj in selectedObjects)
            {
                if (selectedObj is not ProductionBuilding productionBuilding)
                    continue;

                ProductionCommand productionCommand = new ProductionCommand(buildTime, unitPrefab, productionBuilding, commander);
                productionBuilding.SetCommand(productionCommand);
            }
        }


    }
}