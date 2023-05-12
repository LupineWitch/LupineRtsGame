using Assets.Scripts.Commandables;
using Assets.Scripts.Objects.Buildings;
using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Classes.Commands
{
    public class ProductionCommand : Command<ICommander, IDeputy>
    {
        private EntityBase unitPrefab;
        private float buildTime;

        private float elapsedTime = .0f;
        public ProductionCommand(float buildTime, EntityBase builtUnitPrefab, IDeputy recieverType, ICommander sender) : base(recieverType, sender)
        {
            unitPrefab = builtUnitPrefab;
            CurrentState = CommandState.Queued;
            this.buildTime = buildTime;
        }

        public override IEnumerator CommandCoroutine(Action<CommandResult> resultCallback, Action<CommandState> stateCallback)
        {

            base.commandResultCallback = resultCallback;
            base.currentStateCallback = stateCallback;
            CurrentState = CommandState.InProgress;
            BasicCommandControler commander = sender as BasicCommandControler;
            var producer = reciever as BarracksBuilding;
            while (elapsedTime < buildTime)
            {
                producer.ProductionProgress = elapsedTime / buildTime;
                elapsedTime += Time.deltaTime;
                CurrentState = CommandState.InProgress;
                yield return null;
            }

            var instantiatedUnit = UnityEngine.Object.Instantiate(unitPrefab, commander.UnitsContainer.transform);
            instantiatedUnit.transform.position = producer.ExitPoint.transform.position;
            instantiatedUnit.ChangeOwner(commander);
            CurrentState = CommandState.Ended;
            CommandResult = CommandResult.Success;
        }
    }
}
