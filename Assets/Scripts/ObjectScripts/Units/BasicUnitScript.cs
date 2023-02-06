using Assets.Scripts.Classes.Commands;
using Assets.Scripts.Classes.Enitities;
using Assets.Scripts.Classes.Helpers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BasicUnitScript : MonoBehaviour, ISelectableEntity
{
    private bool isSelected { get; set; } = false;
    public float unitSpeed = 10f;

    private Material outlineMaterial;
    private Command<object,BasicUnitScript> executedCommand;
        

    // Start is called before the first frame update
    void Start()
    {
        outlineMaterial = GetComponent<SpriteRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        outlineMaterial.SetInteger("_IsSelected", isSelected ? 1 : 0);
        if(executedCommand != default)
            HandleCommandExecution();
    }

    private void HandleCommandExecution()
    {
        switch(executedCommand.GetCurrentState())
        {
            case CommandState.Starting:
            case CommandState.InProgress:
            case CommandState.Ending:
                break;
            case CommandState.Cold:
            case CommandState.Queued:
                StartCoroutine(executedCommand.CommandCoroutine());
                break;
            case CommandState.Ended:
                executedCommand = null;
                break;
        }

    }

    public virtual void SetCommand(Command<object,BasicUnitScript> command)
    {
        this.executedCommand = command;
    }

    public bool IsSelectedBy(BasicCommandControler possibleOwner) => isSelected;

    public bool CanBeSelectedBy(BasicCommandControler selector) => true;

    public bool TrySelect(BasicCommandControler selector)
    {
        this.isSelected = true;
        return true;
    }

    public bool TryUnselect(BasicCommandControler selector)
    {
        if(!isSelected)
            return false;

        this.isSelected = false;
        return true;
    }
}
