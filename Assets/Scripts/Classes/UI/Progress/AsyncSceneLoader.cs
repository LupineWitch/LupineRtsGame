using System.Collections;

namespace Assets.Scripts.Classes.UI.Progress
{
    public interface AsyncSceneLoader
    {
        public IEnumerator LoadSceneAsync(string sceneName);
    }
}
