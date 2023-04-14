using Assets.Scripts.Classes.GameData;
using Assets.Scripts.Classes.Helpers;
using Assets.Scripts.Commandables;
using Assets.Scripts.Objects.Buildings;
using Assets.Scripts.Objects.ResourceNodes;
using Assets.Scripts.Objects.Units;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.Classes.Commands
{
    public class CollectResourceCommand : Command<ICommander, IDeputy>
    {
        private ResourceBuilding targetStorage;
        private ResourceNodeTree targetNode;
        private Action<CommandState> orginalCallback;

        public CollectResourceCommand(ResourceBuilding storage, ResourceNodeTree node, IDeputy recieverType, ICommander sender) : base(recieverType, sender)
        {
            this.targetStorage = storage;
            this.targetNode = node;
        }

        public override IEnumerator CommandCoroutine(Action<CommandResult> resultCallback, Action<CommandState> stateCallback)
        {
            this.orginalCallback = stateCallback;
            this.commandResultCallback = resultCallback;
            this.currentStateCallback = SetNewCommand;
            CurrentState = CommandState.Starting;
            BasicCommandControler controler = this.sender as BasicCommandControler;
            Vector3Int cellPosOfTargetNode = controler.MapManager.TransformToCellPosition(targetNode.transform);
            WorkerUnit worker = reciever as WorkerUnit;
            //Assuming this doesnt change
            Vector3Int cellPosOfStorage = controler.MapManager.TransformToCellPosition(targetStorage.transform);
            cellPosOfStorage = controler.BuildingsManager.GetClosestPointNearBuildSite(worker.transform, cellPosOfStorage, targetStorage);
            float timePassed = 0f;
            CurrentState = CommandState.InProgress;
            AStarMoveCommand aStarMoveCommand = new AStarMoveCommand(this.sender, worker, cellPosOfTargetNode, controler.MapManager, worker.unitSpeed);
            worker.SetSubcommand(aStarMoveCommand);
            while (aStarMoveCommand.CurrentState.IsActiveState())
                yield return null;

            if (aStarMoveCommand.CommandResult != CommandResult.Success)
            {
                this.CurrentState = CommandState.Ended;
                this.CommandResult = aStarMoveCommand.CommandResult;
                yield break;
            }

            timePassed = 0;
            while (timePassed < (targetNode.TimeToGather / worker.GatheringEfficiency))
            {
                timePassed += Time.deltaTime;
                yield return null; 
            }

            worker.CarriedResource = new Tuple<RtsResource,int>(targetNode.Resource, targetNode.TryGather(worker.Capacity));
            AStarMoveCommand depositToStorage = new AStarMoveCommand(this.sender, worker, cellPosOfStorage, controler.MapManager, worker.unitSpeed);
            worker.SetSubcommand(depositToStorage);

            while (depositToStorage.CurrentState.IsActiveState())
                yield return null;

            if (targetStorage.TryToDepositResource(worker.CarriedResource.Item1, worker.CarriedResource.Item2))
                worker.CarriedResource = null;

            this.CommandResult = CommandResult.Success;
            this.CurrentState = CommandState.Ended;
            yield break;
        }

        private void SetNewCommand(CommandState state)
        {
            orginalCallback(state);
            if(state == CommandState.Ended && CommandResult != CommandResult.Cancelled)
            {
                this.reciever.SetCommand(new CollectResourceCommand(this.targetStorage, targetNode, reciever, sender));
            }
        }
    }
}
