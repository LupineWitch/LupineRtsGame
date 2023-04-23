using Assets.Scripts.Classes.Commands;
using Assets.Scripts.Classes.Events;
using Assets.Scripts.Commandables;
using Assets.Scripts.Commandables.Directives;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

public class EntityBase : MonoBehaviour, ISelectable, IDeputy
{
    [JsonProperty]
    public string PrefabName { get => prefabName; set => prefabName = value; }

    public IReadOnlyCollection<CommandDirective> AvailableDirectives => menuActions;
    public CommandDirective DefaultDirective { get => defaultDirective; }
    public Sprite Preview { get => preview; set => preview = value; }
    public string DisplayLabel { get => displayLabel; set => displayLabel = value; }
    public event SelectedEvent Selected;
    public event OwnerChangedEvent OwnerChanged;
    public BasicCommandControler Owner { get; protected set; }

    protected CommandDirective defaultDirective;
    protected Command<ICommander, IDeputy> executedCommand;
    protected CommandDirective[] menuActions = new CommandDirective[9];
    protected Coroutine currentlyRunCommandCoroutine = null;
    protected List<Command<ICommander, IDeputy>> currentSubcommands = new();
    protected List<Coroutine> currentSubcoroutines = new();
    protected bool IsSelected { get; set; } = false;

    [SerializeField]
    private Sprite preview;
    [SerializeField]
    private string prefabName = string.Empty;
    private string displayLabel = "Placeholder Entity Label";

    protected virtual void Awake()
    {
        if (preview == null)
            preview = gameObject.GetComponent<SpriteRenderer>().sprite;

        if(string.IsNullOrEmpty(prefabName))
            prefabName = gameObject.name;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (executedCommand != default)
            HandleCommandExecution();
    }

    protected virtual void OnDestroy()
    {
        StopAllCoroutines();
    }

    public bool CanBeSelectedBy(BasicCommandControler selector) => true;

    public bool IsSelectedBy(BasicCommandControler possibleOwner) => IsSelected;

    public void SetCommand(Command<ICommander, IDeputy> command)
    {
        if (executedCommand != null)
            this.executedCommand.CancelCommand();

        if (currentlyRunCommandCoroutine != null)
            this.StopCoroutine(currentlyRunCommandCoroutine);

        foreach (var coroutine in currentSubcoroutines)
            if (coroutine != null)
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
        this.IsSelected = true;
        Selected?.Invoke(this, new SelectedEventArgs(selector, true));
        return true;
    }

    public bool TryUnselect(BasicCommandControler selector)
    {
        if (!IsSelected)
            return false;

        this.IsSelected = false;
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

    public void RaiseSelectedEvent(object sender, EventArgs e) => this.Selected?.Invoke(sender as ISelectable, e as SelectedEventArgs);

    public virtual void ChangeOwner(BasicCommandControler newOwner)
    {
        this.Owner = newOwner;
        OwnerChanged?.Invoke(this, EventArgs.Empty);
    }
}