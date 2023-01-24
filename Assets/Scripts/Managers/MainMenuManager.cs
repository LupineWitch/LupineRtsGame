using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

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

    public void StartGame(string mainSceneName)
    {
        loadingPanel.gameObject.SetActive(true);
        _ = StartCoroutine(LoadSceneAsync(mainSceneName));
    }

    IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            loadingLabel.text = string.Format(initialLoadingFormatString, asyncLoad.progress * 100);
            yield return null;
        }
    }
}
