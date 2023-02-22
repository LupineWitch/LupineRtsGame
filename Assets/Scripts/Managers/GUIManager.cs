using Assets.Scripts.Commandables;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour
{
    [SerializeField]
    private Image selectedEntityPortrait;
    [SerializeField]
    private BasicCommandControler commandControler;

    private void Awake()
    {
        commandControler.SelectionChanged += SelectionWasUpdated;
        selectedEntityPortrait.enabled = false;
    }

    private void OnDisable()
    {
        selectedEntityPortrait.enabled = false;
    }

    private void SelectionWasUpdated(object sender, Assets.Scripts.Classes.Events.SelectionChangedEventArgs e)
    {
        if(e.SelectedEntities.Count <= 0)
            selectedEntityPortrait.enabled = false;
        else
        {
            var entity = e.SelectedEntities.First();
            selectedEntityPortrait.sprite = entity.Preview;
            selectedEntityPortrait.enabled = true;
        }

    }

}
