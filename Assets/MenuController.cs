using System.Collections;
using System.Collections.Generic;
using CustomInput;
using SceneManagement;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private SceneEventChannel _sceneEventChannel;
    [SerializeField] private InventoryController _inventoryController;
    
    
    // Start is called before the first frame update
    void Start()
    {
        _inventoryController.Initialize(_inputReader);    
    }

    public void Play()
    {
        _sceneEventChannel.RequestLoadScene("MainLevel");
    }
    
}
