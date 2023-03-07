using Assets.Scripts.Classes.Commands;
using Assets.Scripts.Classes.Events;
using Assets.Scripts.Commandables;
using Assets.Scripts.Commandables.Directives;
using System.Collections.Generic;
using UnityEngine;

public class EntityBase :  MonoBehaviour, ISelectable, IDeputy
{
    public IReadOnlyCollection<CommandDirective> AvailableDirectives => menuActions;
    public CommandDirective DefaultDirective { get => defaultCommand; }
    Sprite ISelectable.Preview { get => preview; set => preview = value; }
    string ISelectable.DisplayLabel { get => displayLabel; set => displayLabel = value; }
    public event SelectedEvent Selected;

    protected CommandDirective defaultCommand;
    protected Command<ICommander, IDeputy> executedCommand;
    protected CommandDirective[] menuActions = new CommandDirective[9];
    protected Coroutine currentlyRunCommandCoroutine = null;
    protected List<Command<ICommander, IDeputy>> currentSubcommands = new List<Command<ICommander, IDeputy>>();
    protected List<Coroutine> currentSubcoroutines = new List<Coroutine>();
    protected bool isSelected { get; set; } = false;

    private Sprite preview;
    private string displayLabel = "Placeholder Entity Label";

    void Awake()
    {
        preview = gameObject.GetComponent<SpriteRenderer>().sprite;
    }

    // Update is called once per frame
    void Update()
    {
        if (executedCommand != default)
            HandleCommandExecution();
    }

    void OnDestroy()
    {
        StopAllCoroutines();
    }

    public bool CanBeSelectedBy(BasicCommandControler selector) => true;

    public bool IsSelectedBy(BasicCommandControler possibleOwner) => isSelected;

    public void SetCommand(Command<ICommander, IDeputy> command)
    {
        if (this.executedCommand != null)
            this.executedCommand.CancelCommand();

        if (currentlyRunCommandCoroutine != null)
            this.StopCoroutine(currentlyRunCommandCoroutine);

        foreach (var coroutine in currentSubcoroutines)
            this.StopCoroutine(coroutine);

        this.currentSubcoroutines.Clear();
        this.executedCommand = command;
    }

    public void SetSubcommand(Command<ICommander, IDeputy> command)
    {
        currentSubcommands.Add(command);
        currentSubcoroutines.Add(StartCoroutine(command.CommandCoroutine()));
    }

    public bool TrySelect(BasicCommandControler selector)
    {
        this.isSelected = true;
        Selected?.Invoke(this, new SelectedEventArgs(selector, true));
        return true;
    }

    public bool TryUnselect(BasicCommandControler selector)
    {
        if (!isSelected)
            return false;

        this.isSelected = false;
        Selected?.Invoke(this, new SelectedEventArgs(selector, false));
        return true;
    }

    private void HandleCommandExecution()
    {
        switch (executedCommand.GetCurrentState())
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
}