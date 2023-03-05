using Assets.Scripts.Commandables;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Classes.Commands
{
    public class BuildCommand : Command<ICommander, IDeputy>
    {
        public float Progress => buildTime == 0 ? 0f : (elapsedBuildTime / buildTime);

        private float buildTime = 10.0f;//seconds
        private float elapsedBuildTime = 0f;
        private BuildingBase buildingPrefab;
        Vector3Int placementPosition;
        private BuildingManager buildingManager;

        public BuildCommand(IDeputy reciever, BasicCommandControler sender, Vector3Int placement, BuildingBase prefab, BuildingManager buildingManager) : base(reciever, sender)
        {
            this.buildingPrefab = prefab;
            this.placementPosition = placement;
            this.buildingManager = buildingManager;
            CommandResult = CommandResult.NoResult;
            CurrentState = CommandState.Queued;
        }

        public override IEnumerator CommandCoroutine(Action<CommandResult> resultCallback, Action<CommandState> stateCallback)
        {
            _ = resultCallback ?? throw new ArgumentNullException(nameof(resultCallback));
            _ = stateCallback ?? throw new ArgumentNullException(nameof(stateCallback));
            commandResultCallback = resultCallback;
            currentStateCallback = stateCallback;

            CurrentState = CommandState.Starting;
            BasicCommandControler commander = base.sender as BasicCommandControler ?? throw new InvalidCastException();
            BasicUnitScript builder = base.reciever as BasicUnitScript ?? throw new InvalidCastException();
            var buildersPosition = commander.MapManager.TransformToCellPosition(builder.transform);
            var targetPosition = buildingManager.GetClosestPointNearBuildSite(buildersPosition, placementPosition, buildingPrefab);
            AStarMoveCommand moveToBuildingPosition = new AStarMoveCommand(base.sender, builder, targetPosition, commander.MapManager, builder.unitSpeed);
            builder.SetSubcommand(moveToBuildingPosition);

            //wait for command to end
            while (moveToBuildingPosition.CurrentState != CommandState.Ended)
                yield return null;
            
            CurrentState = CommandState.InProgress;
            
            while(elapsedBuildTime < buildTime)
            {
                elapsedBuildTime += Time.deltaTime;
                currentStateCallback(CommandState.InProgress);
                yield return null;
            }

            //Finished building
            buildingManager.TryToPlaceBuildingInWorld(placementPosition, buildingPrefab);
            CurrentState = CommandState.Ending;
            base.CommandResult = CommandResult.Success;
            CurrentState = CommandState.Ended;
        }        
    }
}
