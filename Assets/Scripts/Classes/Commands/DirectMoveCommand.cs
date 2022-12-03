using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.Classes.Commands
{
    internal sealed class DirectMoveCommand<MovingType> : Command<MovingType> 
    {
        private Vector3 destination;
        private Vector3 currentPosition;
        private float movingSpeed = 1f;

        GameObject movingGameObject;

        public DirectMoveCommand(MovingType movingObject, Vector3 destination, float movingSpeed) : base(movingObject)
        {
            this.destination = destination;
            this.movingSpeed = movingSpeed;

            if (movingObject is MonoBehaviour objectComponent)
                movingGameObject = objectComponent.gameObject;
            else if (movingObject is GameObject go)
                movingGameObject = go;
            else
                throw new ArgumentException(string.Format("{0} class initialized with invalid type: {1}, valid type must derive either from {2} or {3}", 
                    nameof(DirectMoveCommand<MovingType>), typeof(MovingType).Name, nameof(GameObject), nameof(MonoBehaviour)),nameof(movingObject) );

            SetCurentState(CommandState.Queued);
        }

        public override CommandState ExecuteOnUpdate()
        { 
            if(Vector3.Distance(currentPosition, destination) < 0.1f)
            {
                EndCommand();
                return CommandState.Ended;
            }

            Vector3 newPosition = Vector3.MoveTowards(currentPosition, destination, movingSpeed * Time.deltaTime);
            movingGameObject.transform.position = newPosition;
            this.currentPosition = movingGameObject.transform.position;
            return CommandState.InProgress;
        }

        public override void StartCommand()
        {
            this.currentPosition = movingGameObject.transform.position;
            this.SetCurentState(CommandState.InProgress);
        }
    }
}
