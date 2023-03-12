using Assets.Scripts.Classes.Events;
using UnityEngine;

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

    public void Awake()
    {
        SpriteRenderer spriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
        SpriteWidth = spriteRenderer.bounds.size.x;
        SpriteHeigth = spriteRenderer.bounds.size.y;

        buildingCollider = gameObject.GetComponent<Collider2D>();
        this.BuildingProgressChanged += buildingProgressBar.RespondToUpdatedProgress;
    }

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

    void OnDestroy()
    {
        BuildingDestroy();
    }

}
