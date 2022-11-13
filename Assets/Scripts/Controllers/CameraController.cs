using System;
using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class CameraController : MonoBehaviour
{
    private CameraControls _cameraControls;
    private InputAction _moveCameraAction;

    private Vector3 _lastPosition;
    private Vector3 _targetPosition;
    private Vector3 _cameraVelocity;

    [SerializeField]
    private float maxSpeed = 1f;
    private float _speed;
    [SerializeField]
    private float acceleration = 10f;
    [SerializeField]
    private float dampening = 15f;

    private void Awake()
    {
        _cameraControls = new CameraControls();
        _moveCameraAction = _cameraControls.DefaultCameraControls.MoveCamera;
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
        GetMovementStatus();
        UpdateVelocity();
        UpdateCameraPosition();
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

    private void GetMovementStatus()
    {
        Vector3 moveDirection = _moveCameraAction.ReadValue<Vector2>().normalized;

        if (moveDirection.sqrMagnitude > 0.1f)
            _targetPosition += moveDirection;
    }

}
