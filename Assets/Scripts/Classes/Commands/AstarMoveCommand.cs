using Assets.Scripts.Classes.Helpers;
using Assets.Scripts.Classes.TileOverlays;
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
    public class AStarMoveCommand<SenderType>: Command<SenderType, BasicUnitScript>
    {
        private Queue<Vector3Int> positionsToVisit;
        private Tilemap tilemap;

        private Vector3Int startCell; 
        private Vector3 startPos;
        private Vector3Int targetCell;
        private Vector3 currentPosition;

        private Vector3Int currentTargetCell;
        private Vector3 currentTargetPosition;

        private float movingSpeed = 1f;
        private bool shouldDequeueNextPoint = true;

        private PathingGridBase pathingGrid;
        private GameObject movingGameObject;
        private Rigidbody2D rigidbody2D;

        #region DEBUG fields
        private OverlayAstarPath overlay;
        #endregion

        public AStarMoveCommand(SenderType sender, BasicUnitScript reciver, Vector3Int target, MapManager mapManager, float speed) : base(reciver,sender)
        {
            tilemap = mapManager.UsedTilemap;             
            this.targetCell = target;
            this.pathingGrid = mapManager.PathingGrid;
            this.movingSpeed = speed;

            if (reciver is MonoBehaviour objectComponent)
                movingGameObject = objectComponent.gameObject;
            else
                throw new ArgumentException(string.Format("{0} class initialized with invalid type: {1}, valid type must derive either from {2} or {3}",
                    nameof(AStarMoveCommand<SenderType>), reciver.GetType().Name, nameof(GameObject), nameof(MonoBehaviour)), nameof(reciver));

            rigidbody2D = movingGameObject.GetComponent<Rigidbody2D>();
            CurrentState = CommandState.Queued;
        }

        public void SetDebugOverlay(OverlayAstarPath overlay) => this.overlay = overlay;

        public override IEnumerator CommandCoroutine(Action<CommandResult> resultCallback, Action<CommandState> stateCallback)
        {
            commandResultCallback = resultCallback;
            currentStateCallback = stateCallback;
            bool shouldKeepGoing = true;
            Vector3 pos = movingGameObject.transform.position;
            startCell = tilemap.WorldToCell(pos);

            //if (true || (targetCell - startCell).HasNegativeComponent())
            //    startCell += (Vector3Int)Vector2Int.one;

            positionsToVisit = pathingGrid.GetFastestPath(startCell, targetCell);
            if (positionsToVisit == null || !positionsToVisit.Any())
            {
                Debug.LogFormat("Pathing from {0} to {1} yielded empty path", pos, targetCell);
                CurrentState = CommandState.Ended;
                CommandResult = CommandResult.Failed;
                shouldKeepGoing = false;
                yield break;
            }

            //A Queue can be enumerated without distrubing its contents
            overlay?.DrawPath(positionsToVisit);
            currentPosition = movingGameObject.transform.position;
            CurrentState = CommandState.InProgress;
            yield return new WaitForFixedUpdate();

            while(shouldKeepGoing)
            {
                if (shouldDequeueNextPoint)
                {
                    if (!positionsToVisit.TryDequeue(out Vector3Int nextPoint))
                    {
                        shouldKeepGoing = false;
                        continue;
                    }

                    this.currentTargetCell = nextPoint;
                    this.currentTargetPosition = positionsToVisit.Count == 0 ? tilemap.GetCellCenterWorld(this.currentTargetCell) : tilemap.CellToWorld(this.currentTargetCell);
                    Debug.LogFormat("Going from {0} to {1}", this.currentPosition, currentTargetPosition);
                    shouldDequeueNextPoint = false;
                }

                if (Vector2.Distance(currentPosition, this.currentTargetPosition) < 0.09f)
                    shouldDequeueNextPoint = true;

                float stepBySpeed = movingSpeed * Time.fixedDeltaTime;// keep step in temp variable, some wierd behaviour happens when trying to do so inline
                Vector2 newPosition = Vector2.MoveTowards(currentPosition, this.currentTargetPosition, stepBySpeed);
                if (rigidbody2D)
                    rigidbody2D.MovePosition(newPosition);
                else
                    movingGameObject.transform.position = newPosition;

                this.currentPosition = movingGameObject.transform.position;
                yield return new WaitForFixedUpdate();
            }
            CurrentState = CommandState.Ended;
            CommandResult = CommandResult.Success;
            overlay?.Dispose();
            yield break; //make sure we exited the coroutine
        }

        public override CommandResult CancelCommand()
        {
            this.overlay.Dispose();
            this.overlay = null;
            return base.CancelCommand();
        }
    }
}
