using Assets.Scripts.Classes.Commands;
using Assets.Scripts.Classes.Helpers;
using Assets.Scripts.Classes.Static;
using Assets.Scripts.Controllers;
using Assets.Scripts.Helpers;
using Assets.Scripts.Managers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Commandables.Directives
{
    public class MoveDirective : CommandDirective
    {

        public override ContextCommandDelegator ContextCommandDelegator => MoveCommandContext;

        public override Sprite ButtonIcon
        {
            get 
            {
                string path = Path.Join(ResourceNames.ButtonIconsResourcePath, ResourceNames.GUISpriteSheet);
                return ResourcesUtilities.LoadSpriteFromSpritesheet(path, "IconMarker");
            }
        }

        private void MoveCommandContext(InputAction.CallbackContext inputActionContext, BasicCommandControler commander, List<ISelectable> selectedObjects)
        {
            InputAction pointerAction = inputActionContext.action.actionMap.FindAction(nameof(BasicControls.CommandControls.PointerPosition));
            Vector2 mousePos = pointerAction.ReadValue<Vector2>();
            mousePos = Camera.main.ScreenToWorldPoint(mousePos);
            TopCellResult cellResult = commander.GetTopCellResult(mousePos);

            if (!cellResult.found || cellResult.topCell.HasNegativeComponent())
                return;

            foreach (BasicUnitScript unit in selectedObjects.Where(obj => obj is BasicUnitScript))
            {
                AStarMoveCommand moveOrder = new AStarMoveCommand(
                commander,
                unit,
                cellResult.topCell,
                commander.MapManager,
                unit.unitSpeed);

                //moveOrder.SetDebugOverlay(new OverlayAstarPath(this.mainTilemap, overlayParent, tileToSet));
                unit.SetCommand(moveOrder);
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
    }
}
