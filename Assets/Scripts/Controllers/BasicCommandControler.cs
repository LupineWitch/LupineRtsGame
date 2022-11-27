using Assets.Scripts.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using static UnityEngine.InputSystem.InputAction;

public class BasicCommandControler : MonoBehaviour
{
    [SerializeField]
    private Tilemap mainTilemap;

    private Vector2 startPosition;

    private CameraControls _cameraControls;
    private InputAction _pointerPosition;

    private List<GameObject> selectedObjects;
    [SerializeField]
    private GameObject selectionBox;


    private void Awake()
    {
        selectedObjects = new List<GameObject>();
    }

    private void OnEnable()
    {
        _ = mainTilemap ?? throw new ArgumentNullException(nameof(mainTilemap) + "field is null");
        _ = selectionBox ?? throw new ArgumentNullException(nameof(selectionBox) + "field is null");


        _cameraControls = new CameraControls();
        _cameraControls.CommandControls.Enable();
        _cameraControls.CommandControls.MainPointerDrag.started += MainPointerDrag_started;
        _cameraControls.CommandControls.MainPointerDrag.performed += MainPointerDrag_performed;
        _cameraControls.CommandControls.MainPointerDrag.canceled += MainPointerDrag_canceled;

        _pointerPosition = _cameraControls.CommandControls.PointerPosition;
        selectionBox.SetActive(false);
    }

    private void MainPointerDrag_started(CallbackContext obj)
    {
        selectedObjects.Clear();
        Vector2 pointerPos = _pointerPosition.ReadValue<Vector2>();
        pointerPos = Camera.main.ScreenToWorldPoint(pointerPos);
        startPosition = pointerPos; // mainTilemap.WorldToLocal(pointerPos);
        selectionBox.SetActive(true);
    }

    private void MainPointerDrag_performed(CallbackContext obj)
    {
        //Draw selection box
        Vector2 pointerPos = _pointerPosition.ReadValue<Vector2>();
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
        Vector2 pointerPos = _pointerPosition.ReadValue<Vector2>();
        pointerPos = Camera.main.ScreenToWorldPoint(pointerPos);
        Collider2D[] hits = Physics2D.OverlapAreaAll(startPosition, pointerPos);
        Debug.Log(string.Format("Start position: {0}, end position: {1}", startPosition, pointerPos));
        foreach (var hit in hits)
        {
            selectedObjects.Add(hit.gameObject);
            Debug.Log(hit.gameObject.name);
        }

        startPosition = default;
        selectionBox.SetActive(false);
    }
}
