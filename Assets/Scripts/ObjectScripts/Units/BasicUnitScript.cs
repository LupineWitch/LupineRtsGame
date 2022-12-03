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
    private Command<BasicUnitScript> executedCommand;
        

    // Start is called before the first frame update
    void Start()
    {
        outlineMaterial = GetComponent<SpriteRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        outlineMaterial.SetInteger("_IsSelected", isSelected ? 1 : 0);

        HandleCommandExecution();
    }

    private void HandleCommandExecution()
    {
        if (executedCommand == null)
            return;

        if (executedCommand.GetCurentState.IsActiveState())
            executedCommand.ExecuteOnUpdate();
        
        switch(executedCommand.GetCurentState)
        {
            case CommandState.Queued:
                executedCommand.StartCommand();
                break;
            case CommandState.Ended:
                executedCommand = null;
                break;
        }
    }

    public virtual void SetCommand(Command<BasicUnitScript> command)
    {
        this.executedCommand = command;
    }
}
