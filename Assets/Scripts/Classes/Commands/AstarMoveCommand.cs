using Assets.Scripts.Managers;
using Assets.Scripts.Pathfinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace Assets.Scripts.Classes.Commands
{
    public class AstarMoveCommand<MovingType> : Command<MovingType>
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

        public AstarMoveCommand(MovingType reciver, Vector3Int target, MapManager mapManager, float speed) : base(reciver)
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
                    nameof(DirectMoveCommand<MovingType>), typeof(MovingType).Name, nameof(GameObject), nameof(MonoBehaviour)), nameof(reciver));

            rigidbody2D = movingGameObject.GetComponent<Rigidbody2D>();
            SetCurentState(CommandState.Queued);
        }

        public override CommandState ExecuteOnUpdate()
        {
            if (shouldDequeueNextPoint)
            {
                if (!positionsToVisit.TryDequeue(out Vector3Int nextPoint))
                    EndCommand();

                this.currentTargetCell = nextPoint;
                this.currentTargetPos = tilemap.CellToLocal(this.currentTargetCell);
                //Debug.Log($"Current Target Cell: {currentTargetCell}");
                //Debug.Log($"Current Target Pos: {currentTargetPos}");
                shouldDequeueNextPoint = false;
            }

            if (Vector2.Distance(currentPosition, this.currentTargetPos) < 0.09f)
            {
                shouldDequeueNextPoint = true;
                return CommandState.InProgress;
            }

            Vector3 newPosition = Vector3.MoveTowards(currentPosition, this.currentTargetPos, movingSpeed * Time.deltaTime);
            if (rigidbody2D)
                rigidbody2D.MovePosition(newPosition);
            else
                movingGameObject.transform.position = newPosition;

            this.currentPosition = movingGameObject.transform.position;
            this.rigidbody2D.velocity = Vector2.zero;
            return CommandState.InProgress;
        }

        public override void StartCommand()
        {
            this.SetCurentState(CommandState.Starting);
            Vector3 pos = movingGameObject.transform.localPosition;
            startCell = tilemap.WorldToCell(pos);
            //Debug.Log($"Start Cell:{startCell}");
            //Debug.Log($"Target Cell:{targetCell}");
            positionsToVisit = pathingGrid.GetFastestPath(startCell, targetCell);
            if(positionsToVisit == null || !positionsToVisit.Any())
            {
                Debug.LogFormat("Pathing from {0} to {1} yielded empty path", pos, targetCell);
                this.EndCommand();
            }
            this.currentPosition = movingGameObject.transform.position;
            this.SetCurentState(CommandState.InProgress);
        }

        public override CommandResult EndCommand()
        {
            if (this.rigidbody2D)
                this.rigidbody2D.velocity = Vector2.zero;

            return base.EndCommand();
        }
    }
}
