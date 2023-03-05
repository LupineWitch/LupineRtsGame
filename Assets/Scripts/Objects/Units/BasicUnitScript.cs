using Assets.Scripts.Classes.Commands;
using Assets.Scripts.Classes.Enitities;
using Assets.Scripts.Classes.Helpers;
using Assets.Scripts.Commandables;
using Assets.Scripts.Commandables.Directives;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public sealed class BasicUnitScript : MonoBehaviour, ISelectable, IDeputy
{
    public float unitSpeed = 10f;
    public IReadOnlyCollection<CommandDirective> AvailableDirectives => menuActions;
    public CommandDirective DefaultDirective { get => defaultCommand;}

    Sprite ISelectable.Preview { get => preview; set => preview = value; }
    string ISelectable.DisplayLabel { get => displayLabel; set => displayLabel = value; }

    private CommandDirective defaultCommand;
    private Material outlineMaterial;
    private Command<ICommander, IDeputy> executedCommand;
    private bool isSelected { get; set; } = false;
    private CommandDirective[] menuActions = new CommandDirective[9];
    private Sprite preview;
    private string displayLabel = "Placeholder Unit Label";
    private Coroutine currentlyRunCommandCoroutine = null;
    private List<Command<ICommander, IDeputy>> currentSubcommands = new List<Command<ICommander, IDeputy>>();
    private List<Coroutine> currentSubcoroutines = new List<Coroutine>();

    // Start is called before the first frame 
    void Awake()
    {
        preview = gameObject.GetComponent<SpriteRenderer>().sprite;
    }

    void Start()
    {
        outlineMaterial = GetComponent<SpriteRenderer>().material;
        defaultCommand = menuActions[0] = new MoveDirective();
        menuActions[1] = new BuildDirective();
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
                currentlyRunCommandCoroutine = StartCoroutine(executedCommand.CommandCoroutine());
                break;
            case CommandState.Ended:
                executedCommand = null;
                break;
        }

        int i = 0;
        while (i < currentSubcommands.Count)
        {
            switch (currentSubcommands[i].GetCurrentState())
            {
                case CommandState.Starting:
                case CommandState.InProgress:
                case CommandState.Ending:
                case CommandState.Cold:
                case CommandState.Queued:
                    break;
                case CommandState.Ended:
                    currentSubcommands.RemoveAt(i);
                    continue;
            }
            i++;
        } 

    }

    public void SetCommand(Command<ICommander, IDeputy> command)
    {
        if (this.executedCommand != null)
            this.executedCommand.CancelCommand();

        if (currentlyRunCommandCoroutine != null)
            this.StopCoroutine(currentlyRunCommandCoroutine);

        foreach(var coroutine in currentSubcoroutines)
            this.StopCoroutine(coroutine);

        this.currentSubcoroutines.Clear();
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
        if (!isSelected)
            return false;

        this.isSelected = false;
        return true;
    }

    public void SetSubcommand(Command<ICommander, IDeputy> command)
    {
        currentSubcommands.Add(command);
        currentSubcoroutines.Add(StartCoroutine(command.CommandCoroutine()));
    }

     public void OnDestroy()
    {
        StopAllCoroutines();
    }

}
