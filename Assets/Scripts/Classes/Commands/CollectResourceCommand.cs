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

        public CollectResourceCommand(ResourceBuilding storage, ResourceNodeTree node, IDeputy recieverType, ICommander sender) : base(recieverType, sender)
        {
            this.targetStorage = storage;
            this.targetNode = node;
        }

        public override IEnumerator CommandCoroutine(Action<CommandResult> resultCallback, Action<CommandState> stateCallback)
        {
            this.commandResultCallback = resultCallback;
            this.currentStateCallback = stateCallback;
            BasicCommandControler controler = this.sender as BasicCommandControler;
            var cellPosOfTargetNode = controler.MapManager.TransformToCellPosition(targetNode.transform);
            WorkerUnit worker = reciever as WorkerUnit;
            //Assuming this doesnt change
            var cellPosOfStorage = controler.MapManager.TransformToCellPosition(targetStorage.transform);
            while(targetNode.CanBeMined)
            {
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

                yield return new WaitForSeconds(targetNode.TimeToGather / worker.GatheringEfficiency);
                worker.Capacity = targetNode.TryGather(worker.Capacity);
                AStarMoveCommand depositToStorage = new AStarMoveCommand(this.sender, worker, cellPosOfStorage, controler.MapManager, worker.unitSpeed);
                worker.SetSubcommand(depositToStorage);

                while (aStarMoveCommand.CurrentState.IsActiveState())
                    yield return null;
            }

            this.CurrentState = CommandState.Ended;
            this.CommandResult = CommandResult.Success;
        }
    }
}
