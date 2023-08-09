using System;
using System.Collections.Generic;
using CustomInput;
using InventorySystem;
using InventorySystem.Containers;
using InventorySystem.Grid;
using Items;
using Player;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private InventoryGrid _playerInventory;
    [SerializeField] private InventoryGrid _containerGrid;

    [SerializeField] private InventoryGrid _weaponASlot;
    [SerializeField] private InventoryGrid _weaponBSlot;
    [SerializeField] private InventoryGrid _helmetSlot;
    [SerializeField] private InventoryGrid _armorSlot;

    [SerializeField] private Transform _inventoryUI;
    [SerializeField] private Transform itemContainerPrefab;
    
    [SerializeField] private PlayerEventChannel _playerEventChannel;
    [SerializeField] private InventoryEvents _inventoryEvents;
    [SerializeField] private InputReader _inputReader;
    
    //List that holds all the items;
    [SerializeField] private List<ItemContainer> _inventoryList;
    [SerializeField] private List<ItemContainer> _containerList;
    
    private LootContainer _openLootContainer;
   
    private void Awake()
    {
        _playerInventory.OnItemAdded += OnItemAdded;
        _playerInventory.OnItemMoved += OnItemMoved;
        
        _containerGrid.OnItemAdded += OnContainerItemAdded;
        _containerGrid.OnItemMoved += OnContainerMoved;
        
        _playerInventory.Initialize();
        _containerGrid.Initialize();
        _weaponASlot.Initialize();
        _weaponBSlot.Initialize();
        _helmetSlot.Initialize();
        _armorSlot.Initialize();
        
        _inputReader.OpenInventory += InputReader_OpenInventory;
        _inputReader.CloseInventory += InputReader_CloseInventory;
        
        _inventoryEvents.OpenLootContainer += OpenLootGrid;
        
        _playerEventChannel.SendPlayerInventoryEvent += LoadPlayerInventory;
        _playerEventChannel.SendPlayerEquipmentEvent += LoadPlayerEquipment;
        _playerEventChannel.SendStashInventoryEvent += LoadPlayerStash;
    }

    private void LoadPlayerEquipment(object sender, BaseItem[] equipment)
    {
        for (int i = 0; i < equipment.Length; i++)
        {
            if(equipment[i] == null) continue;

            ItemContainer itemContainer = Instantiate(itemContainerPrefab).GetComponentInChildren<ItemContainer>();
            itemContainer.Initialize(equipment[i]);
            Debug.Log(itemContainer.GetItem().name);
            InventoryGrid inventoryGrid;
            switch (i)
            {
                case 0:
                    inventoryGrid = _weaponASlot;
                    break;
                case 1:
                    inventoryGrid = _weaponBSlot;
                    break;
                case 2:
                    inventoryGrid = _helmetSlot;
                    break;
                case 3:
                    inventoryGrid = _armorSlot;
                    break;
                default:
                    inventoryGrid = _weaponASlot;
                    break;
            }
            inventoryGrid.PlaceItem(itemContainer, itemContainer.GetGridposition());
        }
    }

    private void LoadPlayerInventory(object sender, List<BaseItem> inventory)
    {
        Debug.Log("Player Data Inv" + inventory.Count + " :" + this);
        
        foreach (BaseItem item in inventory)
        {
            //Instantiate Item Container
            ItemContainer itemContainer = Instantiate(itemContainerPrefab).GetComponentInChildren<ItemContainer>();
            itemContainer.Initialize(item);
            _playerInventory.PlaceItem(itemContainer, itemContainer.GetGridposition());
        }
    }
    
    private void LoadPlayerStash(object sender, List<BaseItem> stash)
    {
        //Debug.Log("Player Data Inv" + inventory.Count + " :" + this);
        foreach (BaseItem item in stash)
        {
            //Instantiate Item Container
            ItemContainer itemContainer = Instantiate(itemContainerPrefab).GetComponentInChildren<ItemContainer>();
            itemContainer.Initialize(item);
            _containerGrid.PlaceItem(itemContainer, itemContainer.GetGridposition());
        }
    }

    /*private void SetPlayerInventory(object sender, InventoryEventArgs e)
    {
        if (e.type != InventoryType.PlayerInventory) { return; }
        _playerInventory = e.inventory;
        
        _playerInventory.OnItemAdded += OnItemAdded;
        _playerInventory.OnItemMoved += OnItemMoved;
    }*/

    private void OnItemAdded(object sender, OnItemChangedEventArgs e)
    {
        Debug.Log($"Added this Item {e.item.GetItem().name}");
        AddItem(_inventoryList, e.item);
    }
    private void OnItemMoved(object sender, OnItemChangedEventArgs e)
    {
        RemoveItem(_inventoryList, e.item);
    }
    
    private void OnContainerItemAdded(object sender, OnItemChangedEventArgs e)
    {
        AddItem(_containerList, e.item);
    }
    private void OnContainerMoved(object sender, OnItemChangedEventArgs e)
    {
        RemoveItem(_containerList, e.item);
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

    private void OnSaveInventory()
    {
        Debug.Log("Sending Inventories!");
        
        _inventoryEvents.OnSavePlayerInventory(_inventoryList);
        
        if (_containerGrid.GetInventoryType() == InventoryType.PlayerStash)
        {
            _inventoryEvents.OnSavePlayerStash(_containerList);
        }
    }

    private void OnDestroy()
    {
        OnSaveInventory();
        _playerEventChannel.SendPlayerInventoryEvent -= LoadPlayerInventory;
        _playerEventChannel.SendStashInventoryEvent -= LoadPlayerStash;
        _playerEventChannel.SendPlayerEquipmentEvent -= LoadPlayerEquipment;

        
        //_inventoryEvents.InventorySpawned -= SetPlayerInventory;
        _inventoryEvents.OpenLootContainer -= OpenLootGrid;
        
        _inputReader.OpenInventory -= InputReader_OpenInventory;
        _inputReader.CloseInventory -= InputReader_CloseInventory;
        
        _containerGrid.OnItemAdded -= OnContainerItemAdded;
        _containerGrid.OnItemMoved -= OnContainerMoved;
    }

    public void OpenLootGrid(object sender, LootContainer e)
    {
        _containerGrid.transform.parent.gameObject.SetActive(true);
        InputReader_OpenInventory();
        _inputReader.EnableInventoryInput();
        
        _openLootContainer = e;
        List<ItemContainer> containerList = _openLootContainer.GetLootList();

        for (int i = 0; i < containerList.Count; i++)
        {
            ItemContainer item = containerList[i];
            if (!e.IsOpened())
            {
                _containerGrid.InsertItem(item);
            }
            else
            {
                _containerGrid.PlaceItem(item, item.GetGridposition());
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
        if (_containerGrid.GetInventoryType() == InventoryType.PlayerStash)
        {
            return;
        }
        _containerGrid.transform.parent.gameObject.SetActive(false);
        //Put itemslist back into lootContainer;
        if (_openLootContainer == null) { return; }

        foreach (ItemContainer item in _containerList)
        {
            item.gameObject.SetActive(false);
            item.ClearSlots();
        }
        
        _openLootContainer.SetItemList(_containerList);
        _openLootContainer = null;
        _containerList = new List<ItemContainer>();
        _containerGrid.ClearGrid();
    }
}
