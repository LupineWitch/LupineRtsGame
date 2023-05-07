using UnityEngine;

namespace Assets.Scripts.Graphics
{
    public class MultipleSpritesISelectableOutline : ISelectableOutline
    {
        Material[] instancedSubMaterials;

        protected override void Awake()
        {
            base.Awake();
            SpriteRenderer[] renderers = gameObject.GetComponentsInChildren<SpriteRenderer>();
            instancedSubMaterials = new Material[renderers.Length];
            for (int i = 0; i < renderers.Length; i++)
                instancedSubMaterials[i] = renderers[i].material;
        }

        protected override void Update()
        {
            foreach (var material in instancedSubMaterials)
            {
                material.SetInteger("_IsSelected", parent.IsSelectedBy(controller) ? 1 : 0);
                material.SetColor("_OutlineColor", parent.Faction.FactionColor);
            }
        }
    }
}
