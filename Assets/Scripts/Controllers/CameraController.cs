using Assets.Scripts.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using static UnityEngine.InputSystem.InputAction;

public class CameraController : MonoBehaviour
{
    private CameraControls _cameraControls;
    private InputAction _moveCameraAction;
    private Camera _parentCamera;

    private Vector3 _lastPosition;
    private Vector3 _targetPosition;
    private Vector3 _cameraVelocity;

    private MouseGridHelper _mouseGridhelper;

    [SerializeField]
    private float maxSpeed = 1f;
    private float _speed;
    [SerializeField]
    private float acceleration = 10f;
    [SerializeField]
    private float dampening = 15f;

    private float zoomVelocity = 0f;
    [SerializeField]
    private float zoomSpeed = 1f;
    [SerializeField]
    private float minZoom = 1f;
    [SerializeField]
    private float maxZoom = 10f;

    [SerializeField]
    private Tilemap mainTilemap;

    private void Awake()
    {
        _parentCamera = this.gameObject.GetComponent<Camera>();
        if (_parentCamera == null)
            throw new NullReferenceException(String.Format("{0} script couldn't awake, gameobject has no camera component", nameof(CameraController)));

        _cameraControls = new CameraControls();
        _moveCameraAction = _cameraControls.DefaultCameraControls.MoveCamera;
        _cameraControls.DefaultCameraControls.MouseClickMain.performed += OnMouseClickMain;

        _mouseGridhelper = new MouseGridHelper(mainTilemap);
    }

    private void OnMouseClickMain(CallbackContext context)
    {
        Vector2 mousePos = _cameraControls.DefaultCameraControls.MousePosition.ReadValue<Vector2>();
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        Vector3Int cellPos = _mouseGridhelper.GetTopCell(mousePos);
        _ = _mouseGridhelper.GetTopCell2(mousePos);

        Debug.LogFormat("clicked cell position: {0}",cellPos);
    }

    private void OnEnable()
    {
        _cameraControls.Enable();
    }

    private void OnDisable()
    {
        _cameraControls.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        _speed = 0f;
        _targetPosition = Vector3.zero;
        _lastPosition = this.transform.position;
        _cameraVelocity = Vector3.zero;
        //_moveCameraAction.performed += OnMoveCamera;
    }

    // Update is called once per frame
    void Update()
    {
        SetTargetPosition();
        UpdateVelocity();
        UpdateCameraPosition();
        ApplyScroll();
    }

    private void ApplyScroll()
    {
        float scrollValue = _cameraControls.DefaultCameraControls.MouseScroll.ReadValue<float>();

        if (scrollValue != 0)
            _parentCamera.orthographicSize = Math.Clamp(_parentCamera.orthographicSize + scrollValue * zoomSpeed * Time.deltaTime, minZoom, maxZoom);
    }

    private void UpdateCameraPosition()
    {
        if(_targetPosition.sqrMagnitude > 0.1f)
        {
            _speed = Mathf.Lerp(_speed, maxSpeed, Time.deltaTime * acceleration);
            transform.position += _targetPosition * _speed * Time.deltaTime;
        }
        else
        {
            _cameraVelocity = Vector3.Lerp(_cameraVelocity, Vector3.zero, Time.deltaTime * dampening);
            transform.position += _cameraVelocity * Time.deltaTime;
        }

        _targetPosition = Vector3.zero;
    }

    private void UpdateVelocity()
    {
        _cameraVelocity = (this.transform.position - _lastPosition) / Time.deltaTime;
        _lastPosition = this.transform.position;
    }    

    private void SetTargetPosition()
    {
        Vector3 moveDirection = _moveCameraAction.ReadValue<Vector2>().normalized;

        if (moveDirection.sqrMagnitude > 0.1f)
            _targetPosition += moveDirection;
    }

}
