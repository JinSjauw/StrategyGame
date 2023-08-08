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
    [SerializeField] private List<ItemContainer> _inventoryList;
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

    private void OnItemAdded(object sender, OnItemChangedEventArgs e)
    {
        Debug.Log($"Added this Item {e.item.GetItem().name}");
        AddItem(_inventoryList, e.item);
    }
    private void OnItemMoved(object sender, OnItemChangedEventArgs e)
    {
        RemoveItem(_inventoryList, e.item);
    }
    
    private void OnLootAdded(object sender, OnItemChangedEventArgs e)
    {
        AddItem(_lootList, e.item);
    }
    private void OnLootMoved(object sender, OnItemChangedEventArgs e)
    {
        RemoveItem(_lootList, e.item);
    }

    private void AddItem(List<ItemContainer> addToList, ItemContainer itemContainer)
    {
        if (!addToList.Contains(itemContainer))
        {
            addToList.Add(itemContainer);
        }
    }
    
    private void RemoveItem(List<ItemContainer> removeFromList, ItemContainer itemContainer)
    {
        if (removeFromList.Contains(itemContainer))
        {
            removeFromList.Remove(itemContainer);
        }
    }
    
    private void InputReader_OpenInventory()
    {
        _inventoryUI.gameObject.SetActive(true);
    }
    
    private void InputReader_CloseInventory()
    {
        _inventoryUI.gameObject.SetActive(false);
        CloseLootGrid();
    }

    public void OpenLootGrid(object sender, LootContainer e)
    {
        _lootGrid.transform.parent.gameObject.SetActive(true);
        InputReader_OpenInventory();
        _inputReader.EnableInventoryInput();
        
        _openLootContainer = e;
        List<ItemContainer> containerList = _openLootContainer.GetLootList();

        for (int i = 0; i < containerList.Count; i++)
        {
            ItemContainer item = containerList[i];
            if (!e.IsOpened())
            {
                _lootGrid.InsertItem(item);
            }
            else
            {
                _lootGrid.PlaceItem(item, item.GetGridposition());
            }
            item.gameObject.SetActive(true);
        }
        if (!e.IsOpened())
        {
            e.SetOpen();
        }
    }
    
    public void CloseLootGrid()
    {
        _lootGrid.transform.parent.gameObject.SetActive(false);
        //Put itemslist back into lootContainer;
        if (_openLootContainer == null) { return; }

        foreach (ItemContainer item in _lootList)
        {
            item.gameObject.SetActive(false);
            item.ClearSlots();
        }
        
        _openLootContainer.SetItemList(_lootList);
        _openLootContainer = null;
        _lootList = new List<ItemContainer>();
        _lootGrid.ClearGrid();
    }
}
