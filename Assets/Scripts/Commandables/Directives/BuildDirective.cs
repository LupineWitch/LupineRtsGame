using Assets.Scripts.Classes.Commands;
using Assets.Scripts.Classes.Static;
using Assets.Scripts.Controllers;
using Assets.Scripts.Helpers;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Commandables.Directives
{
    public class BuildDirective : CommandDirective
    {
        public BuildingBase BuildingPrefab { get => buildingPrefab; }
        private BuildingBase buildingPrefab;

        public BuildDirective()
        {
            string path = Path.Combine(ResourceNames.ButtonIconsResourcePath, ResourceNames.GUISpriteSheet);
            base.ButtonIcon = ResourcesUtilities.LoadSpriteFromSpritesheet(path, ResourceNames.BuildingIcon);
        }

        public BuildDirective(BuildingBase prefab) : this()
        {
            buildingPrefab = prefab;
        }

        public override ContextCommandDelegator ContextCommandDelegator => DelegateBuildCommand;


        public override void OnDirectiveDeselection(BasicCommandControler controller)
        {
            controller.BuildingSpaceManager.Show(false);
            controller.BuildingSpaceManager.UnsetSelectedBuilding();
        }

        public override void OnDirectiveSelection(BasicCommandControler controller)
        {
            controller.BuildingSpaceManager.SetSelectedBuilding(BuildingPrefab);
            controller.BuildingSpaceManager.Show(true);
        }

        private void DelegateBuildCommand(InputAction.CallbackContext obj, BasicCommandControler commander, List<ISelectable> selectedObjects)
        {
            InputAction pointerAction = obj.action.actionMap.FindAction(nameof(BasicControls.CommandControls.PointerPosition));
            Vector2 mousePos = pointerAction.ReadValue<Vector2>();
            mousePos = Camera.main.ScreenToWorldPoint(mousePos);
            TopCellResult clickResult = commander.GetTopCellResult(mousePos);

            if (!clickResult.found)
                return;

            //Validate if bulding can be built here
            if (!(BuildingPrefab.ValidatePlacement(clickResult.topCell, commander.BuildingSpaceManager)))
                return;

            foreach (BasicUnitScript unit in selectedObjects)
            {
                unit.SetCommand(new BuildCommand(unit, commander, clickResult.topCell, BuildingPrefab ?? commander.BuildingsManager.BuildingPrefab, commander.BuildingsManager));
            }

        }
    }
}
