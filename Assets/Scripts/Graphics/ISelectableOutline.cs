using Assets.Scripts.Commandables;
using UnityEngine;

public class ISelectableOutline : MonoBehaviour
{
    protected CommandControllerBase controller;
    private Material outlineMaterial;
    protected ISelectable parent;

    protected virtual void Awake()
    {
        var renderer = GetComponent<SpriteRenderer>();
        if (renderer != null)
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
        outlineMaterial?.SetInteger("_IsSelected", parent.IsSelectedBy(controller) ? 1 : 0);
    }
}
