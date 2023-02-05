using Assets.Scripts.Managers;
using Assets.Scripts.Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace Assets.Scripts.Classes.Commands
{
    public class AStarMoveCommand<SenderType, MovingType> : Command<SenderType, MovingType>
    {
        private Queue<Vector3Int> positionsToVisit;
        private Tilemap tilemap;
        private Vector3Int startCell; 
        private Vector3Int currentTargetCell;
        private Vector3Int targetCell;
        private Vector3 startPos;
        private Vector3 targetPos;
        private Vector3 currentTargetPos;
        private Vector3 currentPosition;
        private PathingGrid pathingGrid;
        private float movingSpeed = 1f;
        private bool shouldDequeueNextPoint = true;

        private GameObject movingGameObject;
        private Rigidbody2D rigidbody2D;

        public AStarMoveCommand(SenderType sender,MovingType reciver, Vector3Int target, MapManager mapManager, float speed) : base(reciver,sender)
        {
            tilemap = mapManager.UsedTilemap;             
            this.targetCell = target;
            this.pathingGrid = mapManager.PathingGrid;
            this.movingSpeed = speed;

            if (reciver is MonoBehaviour objectComponent)
                movingGameObject = objectComponent.gameObject;
            else if (reciver is GameObject go)
                movingGameObject = go;
            else
                throw new ArgumentException(string.Format("{0} class initialized with invalid type: {1}, valid type must derive either from {2} or {3}",
                    nameof(AStarMoveCommand<SenderType, MovingType>), reciver.GetType().Name, nameof(GameObject), nameof(MonoBehaviour)), nameof(reciver));

            rigidbody2D = movingGameObject.GetComponent<Rigidbody2D>();
            currentState = CommandState.Queued;
        }


        public override IEnumerator CommandCoroutine(Action<CommandResult> resultCallback, Action<CommandState> stateCallback)
        {
            commandResultCallback = resultCallback;
            currentStateCallback = stateCallback;
            bool shouldKeepGoing = true;
            Vector3 pos = movingGameObject.transform.position;
            startCell = tilemap.WorldToCell(pos);

            positionsToVisit = pathingGrid.GetFastestPath(startCell, targetCell);
            if (positionsToVisit == null || !positionsToVisit.Any())
            {
                Debug.LogFormat("Pathing from {0} to {1} yielded empty path", pos, targetCell);
                currentState = CommandState.Ended;
                commandResult = CommandResult.Failed;
                shouldKeepGoing = false;
                yield break;
            }
            this.currentPosition = movingGameObject.transform.position;
            currentState = CommandState.InProgress;
            yield return new WaitForFixedUpdate();

            while(shouldKeepGoing)
            {
                if (shouldDequeueNextPoint)
                {
                    if (!positionsToVisit.TryDequeue(out Vector3Int nextPoint))
                    {
                        shouldKeepGoing = false;
                        yield return null;
                    }

                    this.currentTargetCell = nextPoint;
                    this.currentTargetPos = tilemap.CellToWorld(this.currentTargetCell);
                    shouldDequeueNextPoint = false;
                }

                if (Vector2.Distance(currentPosition, this.currentTargetPos) < 0.09f)
                    shouldDequeueNextPoint = true;

                float stepBySpeed = movingSpeed * Time.fixedDeltaTime;// keep step in temp variable, some wierd behaviour happens when trying to do so inline
                Vector2 newPosition = Vector2.MoveTowards(currentPosition, this.currentTargetPos, stepBySpeed);
                if (rigidbody2D)
                    rigidbody2D.MovePosition(newPosition);
                else
                    movingGameObject.transform.position = newPosition;

                this.currentPosition = movingGameObject.transform.position;
                yield return new WaitForFixedUpdate();
            }
            currentState = CommandState.Ended;
            commandResult = CommandResult.Success;
            yield break; //make sure we exited the coroutine
        }

    }
}
