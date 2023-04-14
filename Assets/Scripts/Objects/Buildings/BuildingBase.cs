using Assets.Scripts.Classes.Events;
using Assets.Scripts.Classes.Helpers;
using Assets.Scripts.Classes.Static;
using Assets.Scripts.Classes.TileOverlays;
using Assets.Scripts.Managers;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum CellConstructionSuitability
{
    Available = 0,
    Affected = 1,
    Unavailable = 2
}

public class BuildingBase : EntityBase
{
    public float BuildProgress 
    { 
        get => buildProgress;
        set
        {
            ProgressEventArgs buildingEventArgs = new ProgressEventArgs(value);
            this.BuildingProgressChanged?.Invoke(this, buildingEventArgs);
            buildProgress = value;
        }
    }

    public event ProgressEvent BuildingProgressChanged;

    public int BuildingLayer { get; protected set; } = 2;
    public float SpriteWidth { get; private set; }
    public float SpriteHeigth { get; private set; }
    public BoundsInt OccupiedBounds { get; private set; }
    public Vector2Int BuildingSize { get => buildingSize; protected set => buildingSize = value; }
    public Vector3Int TilePosition { get => tilePosition; protected set => tilePosition = value; }
    public Collider2D Collider { get => buildingCollider; }

    ///On Created - get nodes to disconnect from pathing grid
    ///On Destroyed - get nodes to try to reconnect to pathing grid
    public event BuildingCreatedEvent Created;
    public event BuildingStateChanged StateChanged;
    public event BuildingDestroyedEvent Destroyed;

    [SerializeField]
    private Vector2Int buildingSize;
    [SerializeField]
    protected ProgressBar buildingProgressBar;

    private Collider2D buildingCollider;
    private Vector3Int tilePosition;
    private float buildProgress = 0f;
    private SpriteRenderer spriteRenderer;

    public void Initialize(int builidngLayer, BoundsInt occupiedBounds, Vector3Int tilePosition)
    {
        BuildingLayer = builidngLayer;
        OccupiedBounds = occupiedBounds;
        this.TilePosition = tilePosition;
        this.Created.Invoke(this, new BuildingEventArgs(occupiedBounds = OccupiedBounds));
    }

    public void BuildingDestroy()
    {
        this.Destroyed?.Invoke(this, new BuildingEventArgs(this.OccupiedBounds));
    }
  
    public virtual CellConstructionSuitability IsCellAvailableForBuilding(Vector3Int cell, AvailableBuildingSpaceManager buildingsManager)
    {
        bool canBePlaced = !buildingsManager.IsCellOccupiedByBuilding(cell);
        if (!buildingsManager.GetTilemap.HasTile(cell) || buildingsManager.GetTilemap.GetTopTilePosition(cell).z != 2)
            canBePlaced = false;

        return (CellConstructionSuitability)(canBePlaced ? 0 : 2);
    }

    protected override void Awake()
    {
        base.Awake();
        this.spriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
        if(this.spriteRenderer != null)
        {
            SpriteWidth = this.spriteRenderer.bounds.size.x;
            SpriteHeigth = this.spriteRenderer.bounds.size.y;
        }else //Calculate bounds from multiple sprites used by building
        {
            Bounds calculatedBounds = new Bounds(transform.position, Vector3.zero);
            foreach(SpriteRenderer renderer in GetComponentsInChildren<SpriteRenderer>())
                calculatedBounds.Encapsulate(renderer.bounds);

            SpriteWidth = calculatedBounds.size.x;
            SpriteHeigth = calculatedBounds.size.y;
        }

        buildingCollider = gameObject.GetComponent<Collider2D>();
        this.BuildingProgressChanged += buildingProgressBar.RespondToUpdatedProgress;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        BuildingDestroy();
    }

    public bool ValidatePlacement(Vector3Int topCell, AvailableBuildingSpaceManager buildingsManager)
    {
        Vector2Int size = BuildingSize;
        Vector3Int bottomLeftCorner = new Vector3Int(topCell.x - (size.x.GetEvenInteger() / 2), topCell.y - (size.y.GetEvenInteger() / 2), topCell.z);
        BoundsInt newBounds = new BoundsInt(bottomLeftCorner, new Vector3Int(size.x, size.y, 1));
        foreach (var pos in newBounds.allPositionsWithin)
            if (IsCellAvailableForBuilding(pos, buildingsManager) == CellConstructionSuitability.Unavailable)
                return false;

        return true;
    }

    public virtual void ShowSprite(bool show) => this.spriteRenderer.enabled = show;
}
