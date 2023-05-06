using Assets.Scripts.Classes.Factories.Building;
using Assets.Scripts.Commandables;
using Assets.Scripts.Faction;
using Assets.Scripts.Managers;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StartingConditionsManager : MonoBehaviour
{
    public Dictionary<string, int> StartingResources = null;
    public BuildingBase StartingBuildingPrefab;
    public CommandControllerBase CommanderOwner;
    public GameObject BuildingsParent;
    public PlayerResourceManager PlayerResourceManager;

    //Hack to enter dictionary values from editor - writing custom serialisable dictionary is not worth the effort
    [SerializeField]
    private List<string> resourceIdNames = new();
    [SerializeField]
    private List<int> amounts = new();

    public void InitialiseStartingPositions()
    {
        foreach (var resourceIdNameValuePair in StartingResources ?? MergeResourceListsIntoDictionary())
            PlayerResourceManager.ChangeResourceLevel(resourceIdNameValuePair.Key, resourceIdNameValuePair.Value);

        var buildingFactory = new PrefabbedBuildingFactory();
        var cellPosition = CommanderOwner.MapManager.MainTilemap.WorldToCell(this.transform.position);
        var instancedBuilding = buildingFactory.CreateAndPlaceBuildingBasedOnPrefab(StartingBuildingPrefab, cellPosition, BuildingsParent, CommanderOwner.MapManager);
        instancedBuilding.ChangeOwner(CommanderOwner);
    }

    public Dictionary<string,int> MergeResourceListsIntoDictionary()
    {
        if (resourceIdNames.Count != amounts.Count)
        {
            Debug.LogError($"{nameof(resourceIdNames.Count)} is not equal to: {nameof(amounts.Count)}");
            return null;
        }

        Dictionary<string, int> dictMergedFromLists = new();

        for ( int i = 0; i < amounts.Count; i++ )
            dictMergedFromLists.Add(resourceIdNames[i], amounts[i]);

        return dictMergedFromLists;
    }

    protected virtual void Start()
    {
        InitialiseStartingPositions();
    }
}
