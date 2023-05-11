using Assets.Scripts.Commandables;
using Assets.Scripts.Faction;
using UnityEngine;

public class ISelectableOutline : MonoBehaviour
{
    protected CommandControllerBase controller;
    protected ISelectable parent;
    
    private Material outlineMaterial;

    protected virtual void Awake()
    {
        if (TryGetComponent(out SpriteRenderer renderer))
            outlineMaterial = renderer.material;

        parent = gameObject.GetComponent<ISelectable>();
        parent.Selected += OnSelection;
    }

    private void OnSelection(ISelectable sender, Assets.Scripts.Classes.Events.SelectedEventArgs e)
    {
        controller = e.CommandControler;
    }

    protected virtual void Update()
    {
        if(outlineMaterial != null)
        {
            outlineMaterial.SetInteger("_IsSelected", parent.IsSelectedBy(controller) ? 1 : 0);
            outlineMaterial.SetInteger("_IsHighlighted", parent.Highlighted ? 1 : 0);
            if (parent.Faction != null)
            {
                outlineMaterial.SetColor("_OutlineColor", parent.Faction.FactionColor);
                outlineMaterial.SetColor("_HighlightColor", parent.Faction.FactionHighlightColor);
            }
        }
    }
}
