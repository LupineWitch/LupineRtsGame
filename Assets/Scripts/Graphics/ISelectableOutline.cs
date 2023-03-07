using Assets.Scripts.Commandables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ISelectableOutline : MonoBehaviour
{
    private BasicCommandControler controller;
    private Material outlineMaterial;
    private ISelectable parent;
    
    void Awake()
    {
        outlineMaterial = GetComponent<SpriteRenderer>().material;
        parent = gameObject.GetComponent<ISelectable>();
        parent.Selected += OnSelection;
    }

    private void OnSelection(ISelectable sender, Assets.Scripts.Classes.Events.SelectedEventArgs e)
    {
        controller = e.CommandControler;
    }

    void Update()
    {
        outlineMaterial.SetInteger("_IsSelected", parent.IsSelectedBy(controller) ? 1 : 0);
    }
}
