using Assets.Scripts.Helpers;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class CameraController : MonoBehaviour
{
    private BasicControls basicControls;
    private InputAction moveCameraAction;
    private Camera parentCamera;

    private Vector3 lastPosition;
    private Vector3 targetPosition;
    private Vector3 cameraVelocity;

    private TopCellSelector mouseGridhelper;

    [SerializeField]
    private float maxSpeed = 1f;
    private float currentSpeed;
    [SerializeField]
    private float acceleration = 10f;
    [SerializeField]
    private float dampening = 15f;

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
        parentCamera = this.gameObject.GetComponent<Camera>();
        if (parentCamera == null)
            throw new NullReferenceException(String.Format("{0} script couldn't awake, gameobject has no camera component", nameof(CameraController)));

        basicControls = new BasicControls();
        moveCameraAction = basicControls.DefaultCameraControls.MoveCamera;
        mouseGridhelper = new TopCellSelector(mainTilemap);
    }

    private void OnEnable()
    {
        basicControls.Enable();
    }

    private void OnDisable()
    {
        basicControls.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        currentSpeed = 0f;
        targetPosition = Vector3.zero;
        lastPosition = this.transform.position;
        cameraVelocity = Vector3.zero;
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
        float scrollValue = basicControls.DefaultCameraControls.MouseScroll.ReadValue<float>();

        if (scrollValue != 0)
            parentCamera.orthographicSize = Math.Clamp(parentCamera.orthographicSize + scrollValue * zoomSpeed * Time.deltaTime, minZoom, maxZoom);
    }

    private void UpdateCameraPosition()
    {
        if (targetPosition.sqrMagnitude > 0.1f)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, maxSpeed, Time.deltaTime * acceleration);
            transform.position += targetPosition * currentSpeed * Time.deltaTime;
        }
        else
        {
            cameraVelocity = Vector3.Lerp(cameraVelocity, Vector3.zero, Time.deltaTime * dampening);
            transform.position += cameraVelocity * Time.deltaTime;
        }

        targetPosition = Vector3.zero;
    }

    private void UpdateVelocity()
    {
        cameraVelocity = (this.transform.position - lastPosition) / Time.deltaTime;
        lastPosition = this.transform.position;
    }

    private void SetTargetPosition()
    {
        Vector3 moveDirection = moveCameraAction.ReadValue<Vector2>().normalized;

        if (moveDirection.sqrMagnitude > 0.1f)
            targetPosition += moveDirection;
    }

}
