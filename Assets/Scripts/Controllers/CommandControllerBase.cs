using Assets.Scripts.Commandables;
using Assets.Scripts.Faction;
using Assets.Scripts.Helpers;
using Assets.Scripts.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using static UnityEngine.InputSystem.InputAction;


public abstract class CommandControllerBase : MonoBehaviour, ICommander
{
    public GameObject UnitsContainer => unitsContainer;

    public MapManager MapManager => this.mapManager;
    public BuildingManager BuildingsManager => this.buildingManager;
    public AvailableBuildingSpaceManager BuildingSpaceManager => this.buildSpaceManager;
    public BaseFaction Faction { get => faction; set => faction = value; }

    protected ITopCellSelector topCellSelector;

    [SerializeField]
    protected Tilemap mainTilemap;
    [SerializeField]
    protected GameObject unitsContainer;
    [SerializeField]
    protected MapManager mapManager;
    [SerializeField]
    protected AvailableBuildingSpaceManager buildSpaceManager;
    [SerializeField]
    protected BuildingManager buildingManager;

    protected IDeputy CurrentSelectionRepresentative;
    protected List<ISelectable> selectedObjects;
    
    [SerializeField]
    private BaseFaction faction;

    public TopCellResult GetTopCellResult(Vector2 inputValue) => topCellSelector.GetTopCell(inputValue);

    protected virtual void Awake()
    {
        var sceneRoot = SceneManager.GetActiveScene().GetRootGameObjects().First(obj => obj.TryGetComponent<ReferenceManager>(out _));
        //Resolve null field from global reference manager
        var refManager = sceneRoot.GetComponent<ReferenceManager>();
        if (unitsContainer == null)
            unitsContainer = refManager.UnitsContainer;
        if (mainTilemap == null)
            mainTilemap = refManager.MainTilemap;
        if (mapManager == null)
            mapManager = refManager.MapManager;
        if (buildSpaceManager == null)
            buildSpaceManager = refManager.BuildingSpaceManager;
        if(faction == null)
            faction = gameObject.GetComponentInParent<BaseFaction>();

        if(mainTilemap == null)
            throw new ArgumentNullException(nameof(mainTilemap) + "field is null");

        topCellSelector = new TopCellSelector(mainTilemap);
    }

    protected abstract void Update();

    protected abstract void SendCommandForSelectedEntities(CallbackContext obj);
}
