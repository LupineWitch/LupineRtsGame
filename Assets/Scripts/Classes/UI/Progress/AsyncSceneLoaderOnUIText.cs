using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Classes.UI.Progress
{
    public class AsyncSceneLoaderOnUIText : AsyncSceneLoader
    {
        private TextMeshProUGUI textBox;
        private string initialLoadingFormatString = "{0}%";

        public AsyncSceneLoaderOnUIText(TextMeshProUGUI textBox) : base()
        {
            this.textBox = textBox;
            initialLoadingFormatString = String.IsNullOrEmpty(textBox.text) ? initialLoadingFormatString : textBox.text;
        }

        public IEnumerator LoadSceneAsync(string sceneName)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            // Wait until the asynchronous scene fully loads
            while (!asyncLoad.isDone)
            {
                textBox.text = string.Format(initialLoadingFormatString, asyncLoad.progress * 100);
                yield return null;
            }
        }
    }
}
