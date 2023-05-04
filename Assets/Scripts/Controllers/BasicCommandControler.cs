using Assets.Scripts.Classes.Events;
using Assets.Scripts.Commandables;
using Assets.Scripts.Commandables.Directives;
using Assets.Scripts.Controllers;
using Assets.Scripts.Helpers;
using Assets.Scripts.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using static UnityEngine.InputSystem.InputAction;

public class BasicCommandControler : CommandControllerBase
{
    public event SelectionChangedEvent SelectionChanged;
    public event CommandContextChangedEvent CommandContextChanged;

    [SerializeField]
    private GameObject selectionBox;

    private Vector2 startPosition;
    private BasicControls basicControls;
    private InputAction pointerPosition;
    private CommandDirective currentCommandDirective;
    private Vector3Int previousCell = Vector3Int.zero;
    private Color previousCellColor;
    private bool wasGuiClickedThisFrame = false;

    public void SetCurrentCommandDirective(int actionId)
    {
        if (actionId < 0)
        {
            currentCommandDirective = null;
            return;
        }

        ResetControllerContext();
        if (CurrentSelectionRepresentative == default)
        {
            Debug.LogErrorFormat("{0} field is null when setting an action", nameof(CurrentSelectionRepresentative));
            return;
        }
        CommandDirective directive = CurrentSelectionRepresentative.AvailableDirectives.ElementAt(actionId);
        if (directive is ImmediateDirective action)
            action.ExecuteImmediately(this, selectedObjects);
        else
            currentCommandDirective = directive;

        directive?.OnDirectiveSelection(this);
    }

    protected override void Awake()
    {
        _ = selectionBox ?? throw new ArgumentNullException(nameof(selectionBox) + "field is null");
        base.Awake();

        basicControls = new BasicControls();
        selectedObjects = new List<ISelectable>();
        previousCellColor = mainTilemap.GetColor(previousCell);
        currentCommandDirective = null;

        SelectionChanged += buildSpaceManager.OnCommanderSelectionChanged;
    }

    protected override void Update()
    {
        ColorCellAtPointer();
        wasGuiClickedThisFrame = EventSystem.current.IsPointerOverGameObject();
    }

    protected virtual void OnEnable()
    {
        basicControls.CommandControls.Enable();
        basicControls.CommandControls.MainPointerDrag.started += MainPointerDrag_started;
        basicControls.CommandControls.MainPointerDrag.performed += MainPointerDrag_performed;
        basicControls.CommandControls.MainPointerDrag.canceled += MainPointerDrag_canceled;

        basicControls.CommandControls.SendCommand.performed += SendCommandForSelectedEntities;
        pointerPosition = basicControls.CommandControls.PointerPosition;
        basicControls.CommandControls.IncreaseDecrease.performed += ChangeTimeScale;
        selectionBox.SetActive(false);
    }
   
    protected override void SendCommandForSelectedEntities(CallbackContext obj)
    {
        if (currentCommandDirective?.ContextCommandDelegator != null)
            currentCommandDirective.ContextCommandDelegator(obj, this, selectedObjects);
    }

    private void ColorCellAtPointer()
    {
        mainTilemap.SetColor(previousCell, previousCellColor);

        Vector2 mousePos = basicControls.CommandControls.PointerPosition.ReadValue<Vector2>();
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        TopCellResult topCellRes = topCellSelector.GetTopCell(mousePos);

        if (!topCellRes.found)
            return;

        previousCellColor = mainTilemap.GetColor(topCellRes.topCell);
        previousCell = topCellRes.topCell;
        mainTilemap.SetTileFlags(topCellRes.topCell, TileFlags.None);
        mainTilemap.SetColor(topCellRes.topCell, Color.green);
    }

    private void ResetControllerContext()
    {
        currentCommandDirective = null;
        buildSpaceManager.Show(false);
    }

    private void MainPointerDrag_started(CallbackContext obj)
    {
        if (wasGuiClickedThisFrame)
            return;

        foreach (var unit in selectedObjects)
            _ = unit.TryUnselect(this);

        selectedObjects.Clear();
        Vector2 pointerPos = pointerPosition.ReadValue<Vector2>();
        pointerPos = Camera.main.ScreenToWorldPoint(pointerPos);
        startPosition = pointerPos; // mainTilemap.WorldToLocal(pointerPos);
        selectionBox.SetActive(true);
    }

    private void MainPointerDrag_performed(CallbackContext obj)
    {
        if (wasGuiClickedThisFrame)
            return;

        //Draw selection box
        Vector2 pointerPos = pointerPosition.ReadValue<Vector2>();
        pointerPos = Camera.main.ScreenToWorldPoint(pointerPos);
        Vector2 lowerLeft = new Vector2(
            Mathf.Min(startPosition.x, pointerPos.x),
            Mathf.Min(startPosition.y, pointerPos.y));
        Vector2 upperRight = new Vector2(
            Mathf.Max(startPosition.x, pointerPos.x),
            Mathf.Max(startPosition.y, pointerPos.y));

        selectionBox.transform.position = lowerLeft;
        selectionBox.transform.localScale = upperRight - lowerLeft;
    }

    private void MainPointerDrag_canceled(CallbackContext obj)
    {
        if (!wasGuiClickedThisFrame)
        {
            HandleSelectionUnderSelectionRect();
            SetCommandContextAccordingToSelection();
        }
        startPosition = default;
        selectionBox.SetActive(false);

        currentCommandDirective?.OnDirectiveDeselection(this);
    }

    private void SetCommandContextAccordingToSelection()
    {
        if (selectedObjects.Count <= 0)
        {
            ResetControllerContext();
            CommandContextChanged.Invoke(this, null);
            return;
        }

        ISelectable firstSelected = selectedObjects.First();
        Type firstType = firstSelected.GetType();
        CommandContextChangedArgs commandContextEventArgs;
        if (selectedObjects.TrueForAll(selected => firstType == selected.GetType()))
        { //Create single context
            IDeputy deputyEntity = firstSelected as IDeputy;
            if (deputyEntity == null)
            {
                Debug.LogError("deputyEntity is null");
                return;
            }

            this.CurrentSelectionRepresentative = deputyEntity;
            currentCommandDirective = deputyEntity.DefaultDirective;
            commandContextEventArgs = new CommandContextChangedArgs(deputyEntity.AvailableDirectives);

        }
        else //Define shared common command context
        {
            SharedCommandContext newSharedContext = new SharedCommandContext(selectedObjects.Cast<IDeputy>());
            this.CurrentSelectionRepresentative = newSharedContext;
            commandContextEventArgs = new CommandContextChangedArgs(CurrentSelectionRepresentative.AvailableDirectives);
        }

        CommandContextChanged.Invoke(this, commandContextEventArgs);
    }

    private void HandleSelectionUnderSelectionRect()
    {
        Vector2 pointerPos = pointerPosition.ReadValue<Vector2>();
        pointerPos = Camera.main.ScreenToWorldPoint(pointerPos);
        Collider2D[] hits = Physics2D.OverlapAreaAll(startPosition, pointerPos);
        foreach (var hit in hits)
        {
            ISelectable selected = hit.gameObject.GetComponent<ISelectable>();
            if (selected == null)
                continue;

            if (selected.TrySelect(this))
                selectedObjects.Add(selected);
        }

        SetCurrentCommandDirective(-1);
        SelectionChanged?.Invoke(this, new SelectionChangedEventArgs(selectedObjects));
    }

    private void ChangeTimeScale(CallbackContext context)
    {

        float deltaTimeScale = context.ReadValue<float>() * Time.deltaTime;

        Time.timeScale *= (1 + deltaTimeScale);
        Debug.LogFormat("Timescale is now: {0}", Time.timeScale);
    }
}
