using UnityEngine;

namespace Assets.Scripts.Helpers
{
    internal interface ITopCellSelector
    {
        Vector3Int GetTopCell(Vector2 mouseWorldPos);
    }
}