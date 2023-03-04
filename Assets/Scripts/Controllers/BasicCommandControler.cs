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
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;
using static UnityEngine.InputSystem.InputAction;

public class BasicCommandControler : MonoBehaviour, ICommander
{
    public event SelectionChangedEvent SelectionChanged;

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
    private List<ISelectable> selectedObjects;
    private ITopCellSelector topCellSelector;
    private ContextCommandDelegator currentContextDelegator;
    private Vector3Int previousCell = Vector3Int.zero;
    private Color previousCellColor;
    private GameObject overlayParent;
    private Tile tileToSet;

    private const string tilePalletsBasePath = "Graphics\\Tilepallets\\UtilityPaletteAssets";
    private const string basicFlatOverlayTile = "BasicWhiteTile";


    public void SetCurrentAction(int actionId)
    {
        ResetControllerContext();
        switch (actionId)
        {
            case 0:
            default:
                currentContextDelegator = BasicMovementOrder;
                break;
            case 1:
                currentContextDelegator = SpawnUnitFromPrefab;
                break;
            case 2:
                buildSpaceManager.SetSelectedBuilding(buildingManager.BuildingPrefab);
                buildSpaceManager.Show(true);
                currentContextDelegator = PlaceBuilding;
                break;

        }
    }


    private void Awake()
    {
        _ = mainTilemap ?? throw new ArgumentNullException(nameof(mainTilemap) + "field is null");
        _ = selectionBox ?? throw new ArgumentNullException(nameof(selectionBox) + "field is null");

        basicControls = new BasicControls();
        selectedObjects = new List<ISelectable>();
        topCellSelector = new TopCellSelector(mainTilemap);
        previousCellColor = mainTilemap.GetColor(previousCell);
        currentContextDelegator = BasicMovementOrder;
        overlayParent = GameObject.Find("Overlays");
        tileToSet = Resources.Load<Tile>(Path.Combine(tilePalletsBasePath, basicFlatOverlayTile));

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
        basicControls.CommandControls.IncreaseDecrease.performed += ChangeTimeScale;
        selectionBox.SetActive(false);
    }

    private void Update()
    {
        mainTilemap.SetColor(previousCell, previousCellColor);

        Vector2 mousePos = basicControls.CommandControls.PointerPosition.ReadValue<Vector2>();
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        TopCellResult topCellRes = topCellSelector.GetTopCell(mousePos);

        if(!topCellRes.found)
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
        HandleSelectionUnderSelectionRect();
    }

    private void HandleSelectionUnderSelectionRect()
    {
        Vector2 pointerPos = pointerPosition.ReadValue<Vector2>();
        pointerPos = Camera.main.ScreenToWorldPoint(pointerPos);
        Collider2D[] hits = Physics2D.OverlapAreaAll(startPosition, pointerPos);
        //Debug.Log(string.Format("Start position: {0}, end position: {1}", startPosition, pointerPos));
        foreach (var hit in hits)
        {
            BasicUnitScript unitScript = hit.gameObject.GetComponent<BasicUnitScript>();
            if (unitScript == null)
                continue;

            selectedObjects.Add(unitScript);
            _ = unitScript.TrySelect(this);
            //Debug.Log(hit.gameObject.name);
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
            currentContextDelegator(obj, selectedObjects);
    }

    private void BasicMovementOrder(CallbackContext context, List<ISelectable> selectedObjects)
    {
        Vector2 mousePos = basicControls.CommandControls.PointerPosition.ReadValue<Vector2>();
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        TopCellResult cellResult = topCellSelector.GetTopCell(mousePos);

        if (!cellResult.found || cellResult.topCell.HasNegativeComponent())
            return;

        foreach (BasicUnitScript unit in selectedObjects)
        {
            AStarMoveCommand moveOrder = new AStarMoveCommand(
                this,
                unit,
                cellResult.topCell,
                mapManager,
                unit.unitSpeed);

            moveOrder.SetDebugOverlay(new OverlayAstarPath(this.mainTilemap, overlayParent, tileToSet));
            unit.SetCommand(moveOrder);
        }
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
