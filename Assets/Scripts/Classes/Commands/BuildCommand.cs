using Assets.Scripts.Commandables;
using Assets.Scripts.Objects.Buildings;
using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Classes.Commands
{
    public class BuildCommand : Command<ICommander, IDeputy>
    {
        public float Progress => buildProgress;

        private float buildTime = 10.0f;//seconds
        private float buildProgress = 0f;
        private BuildingBase buildingPrefab;
        Vector3Int placementPosition;
        private BuildingManager buildingManager;

        public BuildCommand(IDeputy reciever, CommandControllerBase sender, Vector3Int placement, BuildingBase prefab, BuildingManager buildingManager) : base(reciever, sender)
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
            CommandControllerBase commander = base.sender as CommandControllerBase ?? throw new InvalidCastException();
            BasicUnitScript builder = base.reciever as BasicUnitScript ?? throw new InvalidCastException();
            var buildersPosition = commander.MapManager.TransformToCellPosition(builder.transform);
            var targetPosition = buildingManager.GetClosestPointNearBuildSite(buildersPosition, placementPosition, buildingPrefab);
            AStarMoveCommand moveToBuildingPosition = new AStarMoveCommand(base.sender, builder, targetPosition, commander.MapManager, builder.unitSpeed);
            builder.SetSubcommand(moveToBuildingPosition);

            //wait for command to end
            while (moveToBuildingPosition.CurrentState != CommandState.Ended)
                yield return null;

            CurrentState = CommandState.InProgress;
            var newBuilding = buildingManager.TryToPlaceBuildingInWorld(placementPosition, buildingPrefab);
            newBuilding.ShowSprite(false);
            newBuilding.ChangeOwner(commander);

            ConstructionSiteBase constructionSite = ConstructionSiteBase.GetConstructionSite(newBuilding, buildingManager.ConstructionSitePrefab);
            while (newBuilding.BuildProgress < 1.0f)
            {
                newBuilding.BuildProgress += Time.deltaTime / buildTime;
                buildProgress = newBuilding.BuildProgress;
                currentStateCallback(CommandState.InProgress);
                yield return null;
            }

            //Finished building
            newBuilding.ShowSprite(true);
            MonoBehaviour.Destroy(constructionSite.gameObject);
            CurrentState = CommandState.Ending;
            base.CommandResult = CommandResult.Success;
            CurrentState = CommandState.Ended;
        }
    }
}
