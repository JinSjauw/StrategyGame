using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SceneManagement
{
    [CreateAssetMenu(menuName = "Events/Load Scene Event Channel")]
    public class SceneEventChannel : ScriptableObject
    {
        public event EventHandler<String> OnLoadSceneRequest; 

        public void RequestLoadScene(string sceneName)
        {
            OnLoadSceneRequest?.Invoke(this, sceneName);
        }
    }
}


