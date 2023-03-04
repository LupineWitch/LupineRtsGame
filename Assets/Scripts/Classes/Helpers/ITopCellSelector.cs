using UnityEngine;

namespace Assets.Scripts.Helpers
{
    public struct TopCellResult
    {
        public Vector3Int topCell;
        public bool found;

        public TopCellResult(Vector3Int topCell, bool found)
        {
            this.topCell = topCell;
            this.found = found;
        }
    }
    public interface ITopCellSelector
    {
        TopCellResult GetTopCell(Vector2 mouseWorldPos);
    }
}