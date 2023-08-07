using System;
using System.Collections.Generic;
using CustomInput;
using InventorySystem;
using InventorySystem.Grid;
using UnityEngine;

public class InventoryUIManager : MonoBehaviour
{
    [SerializeField] private InventoryGrid _playerInventory;
    [SerializeField] private InventoryGrid _lootGrid;
    [SerializeField] private InventoryGrid _stashGrid;
    
    [SerializeField] private Transform _inventoryUI;
    
    [SerializeField] private InventoryEvents _inventoryEvents;
    [SerializeField] private InputReader _inputReader;
    
    //List that holds all the items;
    [SerializeField] private List<ItemContainer> _itemList;
    
    
    private void Awake()
    {
        _inventoryEvents.InventorySpawned += SetPlayerInventory;
        _inputReader.OpenInventory += InputReader_OpenInventory;
        _inputReader.CloseInventory += InputReader_CloseInventory;
    }

    private void SetPlayerInventory(object sender, InventoryEventArgs e)
    {
        _playerInventory = e.inventory;
        
        _playerInventory.OnItemAdded += OnItemAdded;
        _playerInventory.OnItemMoved += OnItemMoved;
    }

    private void OnItemAdded(object sender, OnItemChangedEventArgs e)
    {
        _itemList.Add(e.item);
    }
    private void OnItemMoved(object sender, OnItemChangedEventArgs e)
    {
        _itemList.Remove(e.item);
    }

    private void InputReader_OpenInventory()
    {
        _inventoryUI.gameObject.SetActive(true);
    }
    
    private void InputReader_CloseInventory()
    {
        _inventoryUI.gameObject.SetActive(false);
    }
}
