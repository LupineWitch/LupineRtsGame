using Assets.Scripts.Classes.Commands;
using Assets.Scripts.Classes.Enitities;
using Assets.Scripts.Classes.Helpers;
using Assets.Scripts.Commandables;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public sealed class BasicUnitScript : MonoBehaviour, ISelectable, IDeputy
{
    public float unitSpeed = 10f;
    private bool isSelected { get; set; } = false;
    private MenuAction[] menuActions = new MenuAction[9];

    public IReadOnlyCollection<MenuAction> Actions => throw new System.NotImplementedException();

    private static Sprite preview;
    Sprite ISelectable.Preview { get => preview; set => preview = value; }
    string ISelectable.DisplayLabel { get => "Placeholder Unit Label"; set => throw new System.NotImplementedException(); }
    Command<BasicCommandControler, ISelectable> ISelectable.DefaultCommand { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    private Material outlineMaterial;
    private Command<ICommander, IDeputy> executedCommand;

    private Coroutine currentlyRunCommandCoroutine = null;
    // Start is called before the first frame 
    void Awake()
    {
        preview = gameObject.GetComponent<SpriteRenderer>().sprite;
    }

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
                currentlyRunCommandCoroutine = StartCoroutine(executedCommand.CommandCoroutine());
                break;
            case CommandState.Ended:
                executedCommand = null;
                break;
        }

    }

    public void SetCommand(Command<ICommander, IDeputy> command)
    {
        if (this.executedCommand != null)
            this.executedCommand.CancelCommand();

        if (currentlyRunCommandCoroutine != null)
            this.StopCoroutine(currentlyRunCommandCoroutine);

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

    public void OnDestroy()
    {
        StopAllCoroutines();
    }

}
