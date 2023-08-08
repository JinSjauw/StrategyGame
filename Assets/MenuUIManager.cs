using System;
using System.Collections;
using System.Collections.Generic;
using CustomInput;
using UnityEngine;

public class MenuUIManager : MonoBehaviour
{
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private Transform _inventory;
    
    private void Awake()
    {
        //Enable MENU Actions
        _inputReader.EnableMainMenuInput();
    }
    
    public void OpenInventory()
    {
        _inputReader.EnableMainMenuInput();
        _inventory.gameObject.SetActive(true);
    }
    
    public void CloseInventory()
    {
        _inputReader.DisableMainMenuInput();
        _inventory.gameObject.SetActive(false);
    }
}
