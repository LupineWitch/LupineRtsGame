using Assets.Scripts.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ReferenceManager : MonoBehaviour
{
    public MapManager MapManager => mapManager;
    public AvailableBuildingSpaceManager BuildingSpaceManager => buildingSpaceManager;
    public Tilemap MainTilemap => mainTilemap;
    public GameObject SelectionBox => selectionBox;
    public GameObject UnitsContainer => unitsContainer;
    public GameObject ResourceReadout => resourceReadout;
    public GameObject BuildingsContainer => buildingsContainer;
    public GameObject FactionContainer => factionContainer;
    public GameObject AmbientContainer => ambientContainer;
    [SerializeField]
    private MapManager mapManager;
    [SerializeField]
    private AvailableBuildingSpaceManager buildingSpaceManager;
    [SerializeField]
    private Tilemap mainTilemap;
    [SerializeField]
    private GameObject selectionBox;
    [SerializeField]
    private GameObject unitsContainer;
    [SerializeField]
    private GameObject resourceReadout;
    [SerializeField]
    private GameObject buildingsContainer;
    [SerializeField]
    private GameObject factionContainer;
    [SerializeField]
    private GameObject ambientContainer;
}
