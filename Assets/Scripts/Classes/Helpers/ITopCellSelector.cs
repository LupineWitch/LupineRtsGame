using UnityEngine;

namespace Assets.Scripts.Helpers
{
    internal struct TopCellResult
    {
        public Vector3Int topCell;
        public bool found;

        public TopCellResult(Vector3Int topCell, bool found)
        {
            this.topCell = topCell;
            this.found = found;
        }
    }
    internal interface ITopCellSelector
    {
        TopCellResult GetTopCell(Vector2 mouseWorldPos);
    }
}