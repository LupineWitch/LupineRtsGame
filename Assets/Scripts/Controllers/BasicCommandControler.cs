using Assets.Scripts.Classes.Commands;
using Assets.Scripts.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;
using static UnityEngine.InputSystem.InputAction;

public class BasicCommandControler : MonoBehaviour
{
    [SerializeField]
    private Tilemap mainTilemap;
    [SerializeField]
    private GameObject selectionBox;

    private Vector2 startPosition;
    private BasicControls basicControls;
    private InputAction pointerPosition;
    private List<BasicUnitScript> selectedObjects;
    private MouseGridHelper mouseGridhelper;

    private void Awake()
    {
        _ = mainTilemap ?? throw new ArgumentNullException(nameof(mainTilemap) + "field is null");
        _ = selectionBox ?? throw new ArgumentNullException(nameof(selectionBox) + "field is null");

        basicControls = new BasicControls();
        selectedObjects = new List<BasicUnitScript>();
        mouseGridhelper = new MouseGridHelper(mainTilemap);
    }

    private void OnEnable()
    {
        basicControls.CommandControls.Enable();
        basicControls.CommandControls.MainPointerDrag.started += MainPointerDrag_started;
        basicControls.CommandControls.MainPointerDrag.performed += MainPointerDrag_performed;
        basicControls.CommandControls.MainPointerDrag.canceled += MainPointerDrag_canceled;

        basicControls.CommandControls.SendCommand.performed += SendCommand;
        pointerPosition = basicControls.CommandControls.PointerPosition;
        selectionBox.SetActive(false);
    }

    private void MainPointerDrag_started(CallbackContext obj)
    {
        foreach (var unit in selectedObjects)
            unit.isSelected = false;

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

        Vector2 pointerPos = pointerPosition.ReadValue<Vector2>();
        pointerPos = Camera.main.ScreenToWorldPoint(pointerPos);
        Collider2D[] hits = Physics2D.OverlapAreaAll(startPosition, pointerPos);
        Debug.Log(string.Format("Start position: {0}, end position: {1}", startPosition, pointerPos));
        foreach (var hit in hits)
        {
            BasicUnitScript unitScript = hit.gameObject.GetComponent<BasicUnitScript>() ;
            if (unitScript == null)
                continue;

            selectedObjects.Add(unitScript);
            unitScript.isSelected = true;
            //Debug.Log(hit.gameObject.name);
        }

        startPosition = default;
        selectionBox.SetActive(false);
    }


    private void SendCommand(CallbackContext obj)
    {
        Vector2 mousePos = basicControls.CommandControls.PointerPosition.ReadValue<Vector2>();
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        Vector3Int cellPos = mouseGridhelper.GetTopCell(mousePos);

        foreach (BasicUnitScript unitScript in selectedObjects)
        {
            DirectMoveCommand<BasicUnitScript> moveOrder = new DirectMoveCommand<BasicUnitScript>(unitScript, cellPos, unitScript.unitSpeed);
            Debug.Log(cellPos);
            unitScript.SetCommand(moveOrder);
        }
    }

}
