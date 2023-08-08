using System;
using System.Collections;
using System.Collections.Generic;
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
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

            yield return asyncLoad;
            
            Debug.Log("Success!");
            if (_currentLoadedScene != "Initialization")
            {
                SceneManager.UnloadSceneAsync(_currentLoadedScene);
            }
            _currentLoadedScene = sceneName;
        }
    }

    
}