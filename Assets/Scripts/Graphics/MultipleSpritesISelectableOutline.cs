using Assets.Scripts.Classes.Events;
using Assets.Scripts.Commandables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            foreach(var material in  instancedSubMaterials)
                material.SetInteger("_IsSelected", parent.IsSelectedBy(controller) ? 1 : 0);
        }
    }
}
