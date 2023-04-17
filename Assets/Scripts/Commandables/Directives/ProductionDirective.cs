using Assets.Scripts.Classes.Commands;
using Assets.Scripts.Objects.Buildings;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Commandables.Directives
{
    public class ProductionDirective : ImmediateDirective
    {
        protected string UnitName => unitPrefab.DisplayLabel;
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

        public override void OnDirectiveDeselection(BasicCommandControler controler)
        {
            //Do nothing
        }

        public override void OnDirectiveSelection(BasicCommandControler controler)
        {
            //do Nothing
        }

        public override bool Equals(object obj)
        {
            return obj is ProductionDirective directive &&
                   base.Equals(obj) &&
                   UnitName == directive.UnitName;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), UnitName);
        }
    }
}
