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
    /// <summary>
    /// World's space occupied by building.
    /// </summary>
    public virtual Vector2[] OccupiedWorldPositions { get; protected set; }
    public int BuildingLayer { get; protected set; } = 2;
    public float SpriteWidth { get; private set; }
    public float SpriteHeigth { get; private set; }
    public Bounds WorldBounds { get; private set; }
    public Vector2Int BuildingSize { get => buildingSize; protected set => buildingSize = value; }


    ///On Created - get nodes to disconnect from pathing grid
    ///On Destroyed - get nodes to try to reconnect to pathing grid
    public event BuildingCreatedEvent OnCreated;
    public event BuildingStateChanged OnStateChanged;
    public event BuildingDestroyedEvent OnDestroyed;

    [SerializeField]
    private Vector2Int buildingSize;
    private Collider2D buildingCollider;
    private Vector3[] occupiedPositions = default;
    private float tileSize;

    public void Awake()
    {
        SpriteRenderer spriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();

        WorldBounds = spriteRenderer.bounds;
        SpriteWidth = WorldBounds.size.x;
        SpriteHeigth = WorldBounds.size.y;

        buildingCollider = gameObject.GetComponent<Collider2D>();
    }
    
    public IEnumerable<Vector3> GetOccupiedCells()
    {
        if(occupiedPositions == default)
            occupiedPositions = CalculateOccupiedCells();

        return occupiedPositions;
    }

    private Vector3[] CalculateOccupiedCells()
    {
        List<Vector3> occupiedPositions = new List<Vector3>();
        foreach (Vector2 pos in OccupiedWorldPositions)
        {
            var distanceToCollider = Vector2.Distance(buildingCollider.ClosestPoint(pos), pos);
            if (buildingCollider.OverlapPoint(pos) || distanceToCollider <= tileSize)
                occupiedPositions.Add(new Vector3(pos.x, pos.y, BuildingLayer ));
        }

        return occupiedPositions.ToArray();
    }

    public void Initialize(float cellSize, int builidngLayer, Vector2[] newBounds)
    {
        BuildingLayer = builidngLayer;
        tileSize = cellSize;
        OccupiedWorldPositions = newBounds;
        this.OnCreated.Invoke(this, new BuildingEventArgs() { OccupiedWorldPositions = GetOccupiedCells() });
    }
}
