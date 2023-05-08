using System.Collections.Generic;
using System.IO;
using Assets.Scripts.Classes.DPL;
using Assets.Scripts.Classes.Models.Level;
using Assets.Scripts.Classes.Models.Level.Map;
using Assets.Scripts.Classes.Static;
using Assets.Scripts.Classes.UI.Progress;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadMapViewManager : MonoBehaviour
{
    public string MapsDirectory = @"Maps";
    public int SelectedMapIndex { get { return selectedIndex; } }

    [SerializeField]
    private TextMeshProUGUI MapSizeLabel;
    [SerializeField]
    private TextMeshProUGUI MapNameLabel;
    [SerializeField]
    private TextMeshProUGUI MapDiffLabel;
    [SerializeField]
    private GameObject MapScrollViewContent;
    [SerializeField]
    private GameObject mapButtonPrefab;
    [SerializeField]
    private GameObject loadingModal;
    [SerializeField]
    private TextMeshProUGUI loadingProgressLabel;
    [SerializeField]
    private LoadMapPersistenceData loadedMapData;

    private List<MapModel> AvailableMaps;
    private int selectedIndex = -1;
    private string MapSizeBaseText;
    private string MapNameBaseText;
    private string MapDiffBaseText;

    private void Awake()
    {
        MapSizeBaseText = MapSizeLabel.text;
        MapNameBaseText = MapNameLabel.text;
        MapDiffBaseText = MapDiffLabel.text;

        MapDiffLabel.text = string.Format(MapDiffBaseText, string.Empty);
        MapNameLabel.text = string.Format(MapNameBaseText, string.Empty);
        MapSizeLabel.text = string.Format(MapSizeBaseText, string.Empty);

        loadingModal.SetActive(false);

        IEnumerable<string> filesEnumerator = Directory.EnumerateFiles(MapsDirectory, "*.json", SearchOption.AllDirectories);
        var settings = new JsonSerializerSettings
        {
            Converters = new[]
            {
                new TypeJsonConverter()
            },
        };

        AvailableMaps = new List<MapModel>();
        int mapIndex = 0;
        foreach (string mapFile in filesEnumerator)
        {
            MapModel loadedMap = JsonConvert.DeserializeObject<MapModel>(File.ReadAllText(mapFile), settings);
            AvailableMaps.Add(loadedMap);
            GameObject InstiatiedMapButton = Instantiate(mapButtonPrefab, Vector3.zero, Quaternion.identity, MapScrollViewContent.transform);
            Button ButtonComp = InstiatiedMapButton.GetComponent<Button>();
            int i = mapIndex;
            ButtonComp.onClick.AddListener(() => SelectMap(i, loadedMap));
            TextMeshProUGUI textField = InstiatiedMapButton.GetComponentInChildren<TextMeshProUGUI>();
            textField.text = loadedMap.Name;
            mapIndex++;
        }
    }

    public void SelectMap(int index, MapModel model)
    {
        MapNameLabel.text = string.Format(MapNameBaseText, model.Name);
        MapDiffLabel.text = string.Format(MapDiffBaseText, model.Difficult);
        MapSizeLabel.text = string.Format(MapSizeBaseText, model.MapSize.size);
        selectedIndex = index;
    }

    public void StartGameOnTheMap()
    {
        //TODO:Add proper invalid selection handling here
        if (selectedIndex < 0)
            return;

        loadedMapData.LoadedMapModel = AvailableMaps[selectedIndex];
        loadingModal.SetActive(true);
        AsyncSceneLoader progressModal = new AsyncSceneLoaderOnUIText(loadingProgressLabel);
        StartCoroutine(progressModal.LoadSceneAsync(SceneNames.MainScene));
    }


}
