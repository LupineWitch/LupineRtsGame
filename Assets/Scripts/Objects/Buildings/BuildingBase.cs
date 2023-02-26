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
    public int BuildingLayer { get; protected set; } = 2;
    public float SpriteWidth { get; private set; }
    public float SpriteHeigth { get; private set; }
    public BoundsInt OccupiedBounds { get; private set; }
    public Vector2Int BuildingSize { get => buildingSize; protected set => buildingSize = value; }
    public Collider2D Collider { get => buildingCollider; }

    ///On Created - get nodes to disconnect from pathing grid
    ///On Destroyed - get nodes to try to reconnect to pathing grid
    public event BuildingCreatedEvent OnCreated;
    public event BuildingStateChanged OnStateChanged;
    public event BuildingDestroyedEvent OnDestroyed;

    [SerializeField]
    private Vector2Int buildingSize;
    private Collider2D buildingCollider;

    public void Awake()
    {
        SpriteRenderer spriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
        SpriteWidth = spriteRenderer.bounds.size.x;
        SpriteHeigth = spriteRenderer.bounds.size.y;

        buildingCollider = gameObject.GetComponent<Collider2D>();
    }

    public void Initialize(int builidngLayer, BoundsInt occupiedBounds)
    {
        BuildingLayer = builidngLayer;
        OccupiedBounds = occupiedBounds;
        this.OnCreated.Invoke(this, new BuildingEventArgs() { OccupiedBounds = OccupiedBounds });
    }

    public void BuildingDestroy()
    {
        this.OnDestroyed?.Invoke(this, new BuildingEventArgs() { OccupiedBounds = this.OccupiedBounds });
    }

    public void OnDestroy()
    {
        BuildingDestroy();
    }
}
