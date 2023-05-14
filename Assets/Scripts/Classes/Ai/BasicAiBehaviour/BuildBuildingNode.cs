using Assets.Scripts.Classes.Ai.BehaviourTree;
using Assets.Scripts.Classes.Commands;
using Assets.Scripts.Classes.Helpers;
using Assets.Scripts.Commandables.Directives;
using Assets.Scripts.Controllers;
using Assets.Scripts.Managers;
using Assets.Scripts.Objects.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Classes.Ai.BasicAiBehaviour
{
    public class BuildBuildingNode : AiTreeNodeBase
    {
        private string buildingIdName;
        //Cache controller;
        private BasicAIController controller = null;
        private BuildCommand command = null;

        public BuildBuildingNode(string buildingIdName)
        {
            this.buildingIdName = buildingIdName;
        }

        public override NodeState Evaluate()
        {
            if (command != null && command.CurrentState.IsActiveState())
                return NodeState.Running;

            string buildCountString = $"{buildingIdName}_count";
            //Find free builder
            //Who is able to build a building
            //Find suitable spot -> select random place in bounds centered on the builder
            if (controller == null)
                controller = GetData("AiController") as BasicAIController;

            if (controller == null) return NodeState.Failure;

            int? buildingsCount = GetData(buildCountString) as int? ?? 0;
            if (buildingsCount > 0)
                return NodeState.Success;

            var builders = controller.GetEntities<BuilderUnit>(BasicAIController.UNITS_CONTAINER_NAME, u =>
            {
                if (u.Faction != controller.Faction)
                    return false;
                if (u.CurrentCommandState.IsActiveState())
                    return false;

                return true;
            });
            //get building prefab
            var buildDirective = builders.First().AvailableDirectives.First(dir => 
            {
                if (dir is BuildDirective buildDir)
                    return buildDir.BuildingPrefab.IdName.Equals(buildingIdName);
                else
                    return false;
            }) as BuildDirective;


            foreach (var builder in builders)
            {
                Vector3Int positionCell = controller.MapManager.MainTilemap.WorldToCell(builder.transform.position);
                BoundsInt boundsInt = new BoundsInt(positionCell, new Vector3Int(25, 25, 1));
                foreach (var cell in boundsInt.allPositionsWithin)
                {
                    if (!buildDirective.BuildingPrefab.ValidatePlacement(cell, controller.BuildingSpaceManager))
                        continue;

                    command = new BuildCommand(builder, controller, cell, buildDirective.BuildingPrefab, controller.BuildingsManager);
                    command.CommandFinished += (sender, result) =>
                    {
                        if (result == CommandResult.Success)
                            SetData(buildCountString, buildingsCount + 1);
                        this.command = null;
                    };
                    builder.SetCommand(command);
                    return NodeState.Running;
                }
            }

            return NodeState.Failure;
        }
    }
}
