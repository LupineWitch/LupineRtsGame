using Assets.Scripts.Classes.Commands;
using Assets.Scripts.Controllers;
using Assets.Scripts.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Commandables.Directives
{
    public class ContextDirective : CommandDirective
    {

        public override ContextCommandDelegator ContextCommandDelegator => DelegateOnContext;

        private void DelegateOnContext(InputAction.CallbackContext inputActionContext, BasicCommandControler commander, List<ISelectable> selectedObjects)
        {
            InputAction pointerAction = inputActionContext.action.actionMap.FindAction(nameof(BasicControls.CommandControls.PointerPosition));
            Vector2 mousePos = pointerAction.ReadValue<Vector2>();
            mousePos = Camera.main.ScreenToWorldPoint(mousePos);
            Collider2D possibleBuilding = Physics2D.OverlapPoint(mousePos);
            TopCellResult cellResult = commander.GetTopCellResult(mousePos);

            BuildingBase building = possibleBuilding?.GetComponent<BuildingBase>() ?? null;
            if (building == null || building.BuildProgress >= 1.0f)
            {
                if (!cellResult.found)
                    return;

                foreach(BasicUnitScript deputy in selectedObjects.Where(o => o is BasicUnitScript))
                    deputy.SetCommand(new AStarMoveCommand(commander, deputy, cellResult.topCell, commander.MapManager, deputy.unitSpeed));
            }else
                foreach (BasicUnitScript deputy in selectedObjects.Where(o => o is BasicUnitScript))
                    deputy.SetCommand(new ResumeBuildingCommand( deputy, commander, building, commander.BuildingsManager));
        }

        public override void OnDirectiveDeselection(BasicCommandControler controler)
        {
            //Do nothing
        }

        public override void OnDirectiveSelection(BasicCommandControler controler)
        {
            //do Nothing
        }
    }
}
