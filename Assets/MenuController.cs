using System;
using System.Collections;
using System.Collections.Generic;
using CustomInput;
using Player;
using SceneManagement;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private SceneEventChannel _sceneEventChannel;
    [SerializeField] private PlayerEventChannel _playerEventChannel;
    [SerializeField] private InventoryController _inventoryController;

    private void Awake()
    {
        _playerEventChannel.EnterMainMenu();
    }

    // Start is called before the first frame update
    void Start()
    {
        _inventoryController.Initialize(_inputReader);    
        _playerEventChannel.EnterMainMenu();
    }

    public void Play()
    {
        _sceneEventChannel.RequestLoadScene("MainLevel");
    }
    
}
