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
using UnityEngine.SceneManagement;
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
    private List<ISelectable> selectablesToHighlight = new List<ISelectable>();
    private List<ISelectable> selectablesToHighlightUnderSelectionRect = new List<ISelectable>();

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
        var sceneRoot = SceneManager.GetActiveScene().GetRootGameObjects().First(obj => obj.TryGetComponent<ReferenceManager>(out _));
        //Resolve null field from global reference manager
        var refManager = sceneRoot.GetComponent<ReferenceManager>();
        if (selectionBox == null)
            selectionBox = refManager.SelectionBox;

        base.Awake();

        basicControls = new BasicControls();
        selectedObjects = new List<ISelectable>();
        previousCellColor = mainTilemap.GetColor(previousCell);
        currentCommandDirective = null;

        SelectionChanged += buildSpaceManager.OnCommanderSelectionChanged;
    }

    protected override void Update()
    {
        selectablesToHighlight.ForEach(s => s.HighlightEntity(false));
        selectablesToHighlight.Clear();

        ColorCellAtPointer();
        HighlightSelectablesAtPointer();
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

    private void HighlightSelectablesAtPointer()
    {
        Vector2 mousePos = basicControls.CommandControls.PointerPosition.ReadValue<Vector2>();
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        RaycastHit2D[] hits = Physics2D.RaycastAll(worldPos, Vector2.zero);
        foreach (var hit in hits)
        {
            if (hit.collider != null &&
                hit.transform.gameObject.TryGetComponent(out ISelectable pointedObject))
            {
                pointedObject.HighlightEntity(true);
                selectablesToHighlight.Add(pointedObject);
            }
        }
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

        selectablesToHighlightUnderSelectionRect.ForEach(s => s.HighlightEntity(false));
        selectablesToHighlightUnderSelectionRect.Clear();
        
        //Draw selection box
        Vector2 pointerPos = pointerPosition.ReadValue<Vector2>();
        pointerPos = Camera.main.ScreenToWorldPoint(pointerPos);
        Vector2 lowerLeft = new(
            Mathf.Min(startPosition.x, pointerPos.x),
            Mathf.Min(startPosition.y, pointerPos.y));
        Vector2 upperRight = new(
            Mathf.Max(startPosition.x, pointerPos.x),
            Mathf.Max(startPosition.y, pointerPos.y));

        selectionBox.transform.position = lowerLeft;
        selectionBox.transform.localScale = upperRight - lowerLeft;

        foreach (var selectable in GetAllSelectablesInSelectionRect())
        {
            selectable.HighlightEntity(true);
            selectablesToHighlightUnderSelectionRect.Add(selectable);
        }
    }

    private void MainPointerDrag_canceled(CallbackContext obj)
    {
        this.selectablesToHighlightUnderSelectionRect.ForEach(s => s.HighlightEntity(false));

        if (!wasGuiClickedThisFrame)
        {
            ProcessSelectionRect();
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
            if (CommandContextChanged == null)
                Debug.LogError($"No {nameof(CommandContextChanged)} listeners found");
            else
                CommandContextChanged.Invoke(this, null);

            return;
        }

        ISelectable firstSelected = selectedObjects.FirstOrDefault( s => s is IDeputy);
        if(firstSelected == default)
        {
            Debug.LogWarning("No Deputies in selection");
            return;
        }

        Type firstType = firstSelected.GetType();
        CommandContextChangedArgs commandContextEventArgs;
        if (selectedObjects.TrueForAll(selected => firstType == selected.GetType()))
        { //Create single context
            IDeputy deputyEntity = firstSelected as IDeputy;
            this.CurrentSelectionRepresentative = deputyEntity;
            currentCommandDirective = deputyEntity.DefaultDirective;
            commandContextEventArgs = new CommandContextChangedArgs(deputyEntity.AvailableDirectives);

        }
        else //Define shared common command context
        {
            var selectedDeputies = selectedObjects.Where( so => so is IDeputy);
            SharedCommandContext newSharedContext = new SharedCommandContext(selectedDeputies.Cast<IDeputy>());
            this.CurrentSelectionRepresentative = newSharedContext;
            commandContextEventArgs = new CommandContextChangedArgs(CurrentSelectionRepresentative.AvailableDirectives);
        }

        if (CommandContextChanged == null)
            Debug.LogError($"No {nameof(CommandContextChanged)} listeners found");
        else
            CommandContextChanged.Invoke(this, commandContextEventArgs);
    }

    private void ProcessSelectionRect()
    {
        foreach( var selectable in GetAllSelectablesInSelectionRect())
            if (selectable.TrySelect(this))
                selectedObjects.Add(selectable);

        SetCurrentCommandDirective(-1);
        SelectionChanged?.Invoke(this, new SelectionChangedEventArgs(selectedObjects));
    }

    private IEnumerable<ISelectable>  GetAllSelectablesInSelectionRect()
    {
        Vector2 pointerPos = pointerPosition.ReadValue<Vector2>();
        pointerPos = Camera.main.ScreenToWorldPoint(pointerPos);
        Collider2D[] hits = Physics2D.OverlapAreaAll(startPosition, pointerPos);
        foreach (var hit in hits)
        {
            if (!hit.gameObject.TryGetComponent<ISelectable>(out ISelectable selected))
                continue;
            else
                yield return selected;
        }
    }

    private void ChangeTimeScale(CallbackContext context)
    {

        float deltaTimeScale = context.ReadValue<float>() * Time.deltaTime;

        Time.timeScale *= (1 + deltaTimeScale);
        Debug.LogFormat("Timescale is now: {0}", Time.timeScale);
    }
}
