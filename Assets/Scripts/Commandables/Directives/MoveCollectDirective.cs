using Assets.Scripts.Classes.Commands;
using Assets.Scripts.Controllers;
using Assets.Scripts.Helpers;
using Assets.Scripts.Objects.Buildings;
using Assets.Scripts.Objects.ResourceNodes;
using Assets.Scripts.Objects.Units;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

namespace Assets.Scripts.Commandables.Directives
{
    public class MoveCollectDirective : CommandDirective
    {
        public override ContextCommandDelegator ContextCommandDelegator => MoveCollect;

        private void MoveCollect(CallbackContext inputActionContext, BasicCommandControler commander, List<ISelectable> selectedObjects)
        {
            var mousePos = GetCurrentMousePosition(inputActionContext);
            Collider2D gameObject = Physics2D.OverlapPoint(mousePos);
            TopCellResult cellResult = commander.GetTopCellResult(mousePos);

            ResourceNodeTree node = gameObject?.GetComponent<ResourceNodeTree>() ?? null;
            if (node == null || !node.CanBeMined)
            {
                if (!cellResult.found)
                    return;

                foreach (BasicUnitScript deputy in selectedObjects.Where(o => o is BasicUnitScript))
                    deputy.SetCommand(new AStarMoveCommand(commander, deputy, cellResult.topCell, commander.MapManager, deputy.unitSpeed));
            }
            else
            {
                var storages = commander.BuildingsManager.BuildingsContainer.GetComponentsInChildren<ResourceBuilding>();
                float shortestDistance = float.MaxValue;
                ResourceBuilding closestStorage = null;
                var nodePos = commander.MapManager.TransformToCellPosition(node.transform);
                foreach (ResourceBuilding storage in storages)
                {
                    var storagePos = commander.MapManager.TransformToCellPosition(storage.transform);
                    float distance = Vector3.Distance(storagePos, nodePos);
                    if (distance < shortestDistance || closestStorage == null)
                    {
                        shortestDistance = distance;
                        closestStorage = storage;
                    }
                }

                foreach (WorkerUnit deputy in selectedObjects.Where(o => o is WorkerUnit))
                    deputy.SetCommand(new CollectResourceCommand(closestStorage, node, deputy, commander));
            }
        }

        public override void OnDirectiveDeselection(BasicCommandControler controller)
        {
            //Do Nothing
        }

        public override void OnDirectiveSelection(BasicCommandControler controller)
        {
            //Do Nothing
        }
    }
}
