using Assets.Scripts.Classes.Static;
using Assets.Scripts.Classes.UI.Progress;
using System;
using TMPro;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI loadingLabel;
    [SerializeField]
    private RectTransform loadingPanel;

    private string initialLoadingFormatString;

    private void Awake()
    {
        _ = loadingLabel ?? throw new ArgumentException(nameof(loadingLabel));
        _ = loadingPanel ?? throw new ArgumentException(nameof(loadingPanel));
        initialLoadingFormatString = loadingLabel.text.Clone() as string;
        loadingPanel.gameObject.SetActive(false);
    }

    public void StartGame()
    {
        loadingPanel.gameObject.SetActive(true);
        AsyncSceneLoader progressModal = new AsyncSceneLoaderOnUIText(loadingLabel);
        var coroutine = progressModal.LoadSceneAsync(SceneNames.ChooseMap);
        _ = StartCoroutine(coroutine);
    }
}
