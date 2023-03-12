using Assets.Scripts.Commandables;
using Assets.Scripts.Objects.Buildings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Assets.Scripts.Classes.Commands
{
    public class ResumeBuildingCommand : Command<ICommander, IDeputy>
    {
        BuildingBase target;
        BuildingManager manager;
        private float buildTime = 10f;

        public ResumeBuildingCommand(IDeputy recieverType, ICommander sender, BuildingBase target, BuildingManager buildingManager) : base(recieverType, sender)
        {
            this.target = target;
            this.manager = buildingManager;
            CurrentState = CommandState.Queued;
        }

        public override IEnumerator CommandCoroutine(Action<CommandResult> resultCallback, Action<CommandState> stateCallback)
        {
            this.commandResultCallback = resultCallback;
            this.currentStateCallback = stateCallback;
            BasicUnitScript unit = reciever as BasicUnitScript;
            Vector3Int moveTo = manager.GetClosestPointNearBuildSite(unit.transform, target.TilePosition, target);
            CurrentState = CommandState.Starting;
            AStarMoveCommand moveCmd = new AStarMoveCommand(sender, unit, moveTo, sender.MapManager , unit.unitSpeed);
            reciever.SetSubcommand(moveCmd);
            while (moveCmd.CurrentState != CommandState.Ended)
                yield return null;

            while (target.BuildProgress < 1.0f)
            {
                target.BuildProgress += Time.deltaTime / buildTime;
                currentStateCallback(CommandState.InProgress);
                yield return null;
            }

            target.GetComponent<SpriteRenderer>().enabled = true;
            var constructionSite = target.GetComponentInChildren<ConstructionSiteBase>();
            if(constructionSite != null)
                UnityEngine.Object.Destroy(constructionSite.gameObject);

            CurrentState = CommandState.Ending;
            CommandResult = CommandResult.Success;
            CurrentState = CommandState.Ended;
            yield break;
        }
    }
}
