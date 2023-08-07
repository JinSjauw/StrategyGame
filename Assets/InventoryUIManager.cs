using System;
using System.Collections.Generic;
using CustomInput;
using InventorySystem;
using InventorySystem.Containers;
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
    [SerializeField] private List<ItemContainer> _lootList;

    private LootContainer _openLootContainer;
    
    private void Awake()
    {
        _inventoryEvents.InventorySpawned += SetPlayerInventory;
        //_inventoryEvents.InventorySpawned += SetLootInventory;
        _inventoryEvents.OpenLootContainer += OpenLootGrid;
        
        _inputReader.OpenInventory += InputReader_OpenInventory;
        _inputReader.CloseInventory += InputReader_CloseInventory;
        
        _lootGrid.OnItemAdded += OnLootAdded;
        _lootGrid.OnItemMoved += OnLootMoved;
        
    }

    private void SetPlayerInventory(object sender, InventoryEventArgs e)
    {
        if (e.type != InventoryType.PlayerInventory) { return; }
        _playerInventory = e.inventory;
        
        _playerInventory.OnItemAdded += OnItemAdded;
        _playerInventory.OnItemMoved += OnItemMoved;
    }

    /*private void SetLootInventory(object sender, InventoryEventArgs e)
    {
        if (e.type != InventoryType.LootInventory) { return; }
        
        _lootGrid = e.inventory;
        _lootGrid.OnItemAdded += OnLootAdded;
        _lootGrid.OnItemMoved += OnLootMoved;
    }*/

    private void OnItemAdded(object sender, OnItemChangedEventArgs e)
    {
        _itemList.Add(e.item);
    }
    private void OnItemMoved(object sender, OnItemChangedEventArgs e)
    {
        _itemList.Remove(e.item);
    }
    
    private void OnLootAdded(object sender, OnItemChangedEventArgs e)
    {
        _lootList.Add(e.item);
    }
    private void OnLootMoved(object sender, OnItemChangedEventArgs e)
    {
        _lootList.Remove(e.item);
    }

    private void InputReader_OpenInventory()
    {
        _inventoryUI.gameObject.SetActive(true);
    }
    
    private void InputReader_CloseInventory()
    {
        _inventoryUI.gameObject.SetActive(false);
    }

    public void OpenLootGrid(object sender, LootContainer e)
    {
        _lootGrid.gameObject.SetActive(true);
        _openLootContainer = e;
        //Get list of items;
        //Need random Insert function
        _lootList = _openLootContainer.GetLootList();

        for (int i = 0; i < _lootList.Count; i++)
        {
            _lootList[i].gameObject.SetActive(true);
            _lootGrid.InsertItem(_lootList[i]);
        }
    }
    
    public void CloseLootGrid()
    {
        _lootGrid.gameObject.SetActive(false);
        //Put itemslist back into lootContainer;
        _openLootContainer.SetItemList(_lootList);
        _openLootContainer = null;
        
        for (int i = 0; i < _lootList.Count; i++)
        {
            _lootList[i].gameObject.SetActive(false);
        }
    }
}
