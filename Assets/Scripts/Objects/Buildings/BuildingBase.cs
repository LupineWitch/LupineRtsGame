using Assets.Scripts.Classes.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.Burst;
using System;
using UnityEngine.Tilemaps;

public class BuildingBase : MonoBehaviour
{
    public float BuildProgress 
    { 
        get => buildProgress;
        set
        {
            BuildingEventArgs buildingEventArgs = new BuildingEventArgs(buildProgress: value);
            this.StateChanged?.Invoke(this, buildingEventArgs);
            buildProgress = value;
        }
    }

    public int BuildingLayer { get; protected set; } = 2;
    public float SpriteWidth { get; private set; }
    public float SpriteHeigth { get; private set; }
    public BoundsInt OccupiedBounds { get; private set; }
    public Vector2Int BuildingSize { get => buildingSize; protected set => buildingSize = value; }
    public Collider2D Collider { get => buildingCollider; }

    ///On Created - get nodes to disconnect from pathing grid
    ///On Destroyed - get nodes to try to reconnect to pathing grid
    public event BuildingCreatedEvent Created;
    public event BuildingStateChanged StateChanged;
    public event BuildingDestroyedEvent Destroyed;

    [SerializeField]
    private Vector2Int buildingSize;
    [SerializeField]
    private ProgressBar buildingProgressBar;

    private Collider2D buildingCollider;
    private float buildProgress = 0f;

    public void Awake()
    {
        SpriteRenderer spriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
        SpriteWidth = spriteRenderer.bounds.size.x;
        SpriteHeigth = spriteRenderer.bounds.size.y;

        buildingCollider = gameObject.GetComponent<Collider2D>();
        this.StateChanged += buildingProgressBar.RespondToUpdatedProgress;
    }

    public void Initialize(int builidngLayer, BoundsInt occupiedBounds)
    {
        BuildingLayer = builidngLayer;
        OccupiedBounds = occupiedBounds;
        this.Created.Invoke(this, new BuildingEventArgs(occupiedBounds = OccupiedBounds));
    }

    public void BuildingDestroy()
    {
        this.Destroyed?.Invoke(this, new BuildingEventArgs(this.OccupiedBounds));
    }

    public void OnDestroy()
    {
        BuildingDestroy();
    }
}
