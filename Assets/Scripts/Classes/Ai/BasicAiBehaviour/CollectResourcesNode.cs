using Assets.Scripts.Classes.Ai.BehaviourTree;
using Assets.Scripts.Classes.Helpers;
using Assets.Scripts.Controllers;
using Assets.Scripts.Objects.Units;
using System.Linq;
using Assets.Scripts.Objects.ResourceNodes;
using UnityEngine;
using Assets.Scripts.Classes.Commands;
using Assets.Scripts.Objects.Buildings;

namespace Assets.Scripts.Classes.Ai.BasicAiBehaviour
{

    public class CollectResourcesNode : AiTreeNodeBase
    {
        private readonly string resourceDefName;

        public CollectResourcesNode(string resourceDefName)
        {
            this.resourceDefName = resourceDefName;
        }

        public override NodeState Evaluate()
        {
            BasicAIController controller = GetData("AiController") as BasicAIController;

            if(controller == null) return NodeState.Failure;

            var workers = controller.GetEntities<WorkerUnit>(BasicAIController.UNITS_CONTAINER_NAME,
                u => u.Faction == controller.Faction && !u.CurrentCommandState.IsActiveState());

            if(workers == null || !workers.Any())
                return NodeState.Failure;

            var mapManager = controller.MapManager;
            if(mapManager == null) return NodeState.Failure;

            var startingPosition = controller.transform.parent.GetComponentInChildren<StartingConditionsManager>();

            var nodesSortedByDistance = controller.GetResourceNodes<ResourceNodeBase>(BasicAIController.AMBIENT_CONTAINER_NAME,
                n => n.Resource.IdName.Equals(resourceDefName)).OrderBy(n => Vector2.Distance(
                    startingPosition.transform.position, n.transform.position));
            var storgeFacilities = controller.GetEntities<ResourceBuilding>(BasicAIController.BUILDINGS_CONTAINER_NAME,
                sf => sf.Faction == controller.Faction);

            if (!(nodesSortedByDistance.Any() && storgeFacilities.Any()))
                return NodeState.Failure;

            foreach (WorkerUnit workerUnit in workers)
            {
                var unitPosition = workerUnit.transform.position;
                var selectedNode = nodesSortedByDistance.OrderBy(rn => Vector2.Distance(unitPosition, rn.transform.position)).First();
                var selectedStorage = storgeFacilities.OrderBy(sf => Vector2.Distance(unitPosition, sf.transform.position)).First();
                var gatherCommand = new CollectResourceCommand(selectedStorage, selectedNode, workerUnit, controller);
                workerUnit.SetCommand(gatherCommand);
            }
            return NodeState.Success;
        }
    }
}
