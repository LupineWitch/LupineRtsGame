using Assets.Scripts.Classes.Commands;
using Assets.Scripts.Classes.Helpers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BasicUnitScript : MonoBehaviour
{
    public bool isSelected { get; set; } = false;
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
                StartCoroutine(executedCommand.CommandCoroutine(CommandEnded, CommandUpdated));
                break;
            case CommandState.Ended:
                executedCommand = null;
                break;
        }

    }

    private void CommandEnded(CommandResult commandResult)
    {

    }

    private void CommandUpdated(CommandState commandResult)
    {

    }

    public virtual void SetCommand(Command<object,BasicUnitScript> command)
    {
        this.executedCommand = command;
    }
}
