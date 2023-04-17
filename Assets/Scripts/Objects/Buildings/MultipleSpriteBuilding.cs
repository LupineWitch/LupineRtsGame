using UnityEngine;

namespace Assets.Scripts.Objects.Buildings
{
    public class MultipleSpriteBuilding : BuildingBase
    {
        private SpriteRenderer[] renderers;
        public override void ShowSprite(bool show)
        {
            if (renderers == null || renderers.Length == 0)
                renderers = GetComponentsInChildren<SpriteRenderer>();

            foreach (var renderer in renderers)
                renderer.enabled = show;
        }
    }
}
