using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneManagement
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] private SceneEventChannel _sceneEventChannel;

        private string _currentLoadedScene = "Initialization";
        
        // Start is called before the first frame update
        private void Awake()
        {
            _sceneEventChannel.OnLoadSceneRequest += LoadScene;
            StartCoroutine(LoadSceneAsync("MainMenu"));
        }

        private void LoadScene(object sender, string e)
        {
            StartCoroutine(LoadSceneAsync(e));
        }

        IEnumerator LoadSceneAsync(string sceneName)
        {
            if (_currentLoadedScene != "Initialization")
            {
                AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(_currentLoadedScene);
                yield return asyncUnload;
            }

            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            
            yield return asyncLoad;

            SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
            
            Debug.Log("Success");
            _currentLoadedScene = sceneName;
            
        }
    }

    
}