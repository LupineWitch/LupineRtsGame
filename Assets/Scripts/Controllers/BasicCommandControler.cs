using Assets.Scripts.Classes.Commands;
using Assets.Scripts.Classes.Events;
using Assets.Scripts.Classes.Helpers;
using Assets.Scripts.Classes.TileOverlays;
using Assets.Scripts.Commandables;
using Assets.Scripts.Controllers;
using Assets.Scripts.Helpers;
using Assets.Scripts.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;
using static UnityEngine.InputSystem.InputAction;

public class BasicCommandControler : MonoBehaviour, ICommander
{
    public event SelectionChangedEvent SelectionChanged;
    public event CommandContextChangedEvent CommandContextChanged;


    public MapManager MapManager => this.mapManager;

    [SerializeField]
    private Tilemap mainTilemap;
    [SerializeField]
    private GameObject selectionBox;
    [SerializeField]
    private MapManager mapManager;
    [SerializeField]
    private AvailableBuidlingSpaceManager buildSpaceManager;
    [SerializeField]
    private BuildingManager buildingManager;

    #region TempTesting properties to be moved to other classes
    [SerializeField]
    private GameObject UnitContainer;
    [SerializeField]
    private GameObject UnitPrefab;
    #endregion

    private Vector2 startPosition;
    private BasicControls basicControls;
    private InputAction pointerPosition;
    private InputAction SelectAction;
    private List<ISelectable> selectedObjects;
    private ITopCellSelector topCellSelector;
    private ContextCommandDelegator currentContextDelegator;
    private Vector3Int previousCell = Vector3Int.zero;
    private Color previousCellColor;
    private IDeputy CurrentSelectionRepresentative;
    private bool wasGuiClickedThisFrame = false;

    public void SetCurrentAction(int actionId)
    {
        Debug.LogFormat("Action ID:{0}", actionId);
        ResetControllerContext();
        if(CurrentSelectionRepresentative == default)
        {
            Debug.LogErrorFormat("{0} field is null when setting and action", nameof(CurrentSelectionRepresentative));
            return;
        }
        currentContextDelegator = CurrentSelectionRepresentative.AvailableDirectives.ElementAt(actionId)?.ContextCommandDelegator ?? default;
    }

    public TopCellResult GetTopCellResult(Vector2 inputValue) => topCellSelector.GetTopCell(inputValue);

    private void Awake()
    {
        _ = mainTilemap ?? throw new ArgumentNullException(nameof(mainTilemap) + "field is null");
        _ = selectionBox ?? throw new ArgumentNullException(nameof(selectionBox) + "field is null");

        basicControls = new BasicControls();
        selectedObjects = new List<ISelectable>();
        topCellSelector = new TopCellSelector(mainTilemap);
        previousCellColor = mainTilemap.GetColor(previousCell);
        currentContextDelegator = null;

        SelectionChanged += buildSpaceManager.OnCommanderSelectionChanged;
    }

    private void OnEnable()
    {
        basicControls.CommandControls.Enable();
        basicControls.CommandControls.MainPointerDrag.started += MainPointerDrag_started;
        basicControls.CommandControls.MainPointerDrag.performed += MainPointerDrag_performed;
        basicControls.CommandControls.MainPointerDrag.canceled += MainPointerDrag_canceled;

        basicControls.CommandControls.SendCommand.performed += SendCommandForSelectedEntities;
        pointerPosition = basicControls.CommandControls.PointerPosition;
        SelectAction = basicControls.CommandControls.Select;
        basicControls.CommandControls.IncreaseDecrease.performed += ChangeTimeScale;
        selectionBox.SetActive(false);
    }

    private void Update()
    {
        ColorCellAtPointer();
        if (SelectAction.WasPerformedThisFrame())
            wasGuiClickedThisFrame = EventSystem.current.IsPointerOverGameObject();
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
        currentContextDelegator = null;
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
        if (wasGuiClickedThisFrame)
            return;

        HandleSelectionUnderSelectionRect();
        SetCommandContextAccordingToSelection();
    }

    private void SetCommandContextAccordingToSelection()
    {
        if(selectedObjects.Count <= 0)
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
            currentContextDelegator = deputyEntity.DefaultDirective.ContextCommandDelegator;
            commandContextEventArgs = new CommandContextChangedArgs(deputyEntity.AvailableDirectives);

        }else //Define shared common command context
        {
            commandContextEventArgs = new CommandContextChangedArgs(default);
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
            ISelectable unitScript = hit.gameObject.GetComponent<ISelectable>();
            if (unitScript == null)
                continue;

            if(unitScript.TrySelect(this))
                selectedObjects.Add(unitScript as BasicUnitScript);
        }

        startPosition = default;
        selectionBox.SetActive(false);
        SetCurrentAction(0);
        SelectionChanged?.Invoke(this, new SelectionChangedEventArgs(selectedObjects));
    }

    private void SendCommandForSelectedEntities(CallbackContext obj)
    {
        //TODO: Get click context, UI etc.
        if(currentContextDelegator != null)
            currentContextDelegator(obj, this, selectedObjects);
    }

    private void SpawnUnitFromPrefab(CallbackContext obj, List<ISelectable> selectedObjects)
    {
        Vector2 mousePos = basicControls.CommandControls.PointerPosition.ReadValue<Vector2>();
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        worldPos.z = 2.2f;

        Instantiate(this.UnitPrefab, worldPos, Quaternion.identity, UnitContainer.transform);
    }

    private void PlaceBuilding(CallbackContext obj, List<ISelectable> selectedObjects)
    {
        Vector2 mousePos = basicControls.CommandControls.PointerPosition.ReadValue<Vector2>();
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        TopCellResult cellResult = topCellSelector.GetTopCell(mousePos);
        buildingManager.TryToPlaceBuildingInWorld(cellResult.topCell);
    }

    private void ChangeTimeScale(CallbackContext context)
    {
        
        float deltaTimeScale = context.ReadValue<float>() * Time.deltaTime;

        Time.timeScale *= (1 + deltaTimeScale);
        Debug.LogFormat("Timescale is now: {0}", Time.timeScale);
    }
}
