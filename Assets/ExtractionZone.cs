using System;
using SceneManagement;
using UnitSystem;
using UnityEngine;

public class ExtractionZone : MonoBehaviour
{
    [SerializeField] private SceneEventChannel _sceneEventChannel;

    private bool _activated;
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log("Collision " + col.name);
        if (col.transform.parent.GetComponent<PlayerUnit>() && !_activated)
        {
            _activated = true;
            Debug.Log("To the menu!!");
            _sceneEventChannel.RequestLoadScene("MainMenu");
        }
    }
}
