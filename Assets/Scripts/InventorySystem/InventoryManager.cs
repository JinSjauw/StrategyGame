using System.Collections.Generic;
using CustomInput;
using InventorySystem;
using InventorySystem.Containers;
using InventorySystem.Grid;
using InventorySystem.Items;
using Items;
using Player;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [Header("Inventory Grids")]
    [SerializeField] private InventoryGrid _playerInventory;
    [SerializeField] private InventoryGrid _containerGrid;

    [Header("EquipmentSlots")]
    [SerializeField] private InventoryGrid _weaponASlot;
    [SerializeField] private InventoryGrid _weaponBSlot;
    [SerializeField] private InventoryGrid _helmetSlot;
    [SerializeField] private InventoryGrid _armorSlot;
    [SerializeField] private InventoryGrid _pocketSlots;

    [Header("Inventory Item UI")]
    [SerializeField] private Transform _inventoryUI;
    [SerializeField] private Transform itemContainerPrefab;
    
    [Header("Event Channels")]
    [SerializeField] private PlayerEventChannel _playerEventChannel;
    [SerializeField] private InventoryEvents _inventoryEvents;
    [SerializeField] private InputReader _inputReader;

    [Header("MONAEY")]
    [SerializeField] private float totalCurrency;
    
    //List that holds all the items;
    [Header("Inventory Item Lists")]
    [SerializeField] private List<ItemContainer> _inventoryList;
    [SerializeField] private List<ItemContainer> _pocketList;
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
        _pocketSlots.Initialize();
        
        _inputReader.OpenInventory += InputReader_OpenInventory;
        _inputReader.CloseInventory += InputReader_CloseInventory;
        _inputReader.PocketSelectionChanged += InputReader_PocketSelectionChanged;

        _inventoryEvents.OpenLootContainer += OpenLootGrid;
        _inventoryEvents.PickedUpWorldItem += InsertWorldItem;
        _inventoryEvents.RequestAmmo += SendAmmo;
        
        _playerEventChannel.SendPlayerInventoryEvent += LoadPlayerInventory;
        _playerEventChannel.SendPlayerEquipmentEvent += LoadPlayerEquipment;
        _playerEventChannel.SendStashInventoryEvent += LoadPlayerStash;
        _playerEventChannel.SendPlayerPocketsEvent += LoadPlayerPockets;
    }

    private void CalculateTotalCurrency()
    {
        for (int i = 0; i < _containerList.Count; i++)
        {
            //If itemType = misc/currency
            ItemContainer itemContainer = _containerList[i];
            
            if(itemContainer.GetItemType() != ItemType.Miscellaneous && itemContainer.GetItem().GetItemID() != ItemID.Currency) continue;
            totalCurrency += _containerList[i].GetValue();
        }
    }
    
    private void CalculateNewCurrency(ItemContainer itemContainer, bool subtract = false)
    {
        if (_containerGrid.GetInventoryType() != InventoryType.PlayerStash) return;
        
        if(itemContainer.GetItemType() != ItemType.Miscellaneous && itemContainer.GetItem().GetItemID() != ItemID.Currency) return;

        int amount = itemContainer.GetValue();
        
        //if item type is misc
        if (subtract)
        {
            totalCurrency -= amount;
        }
        else
        {
            totalCurrency += amount;
        }
    }
    #region Inventory Management

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
            //Check here for throwables like grenades;
            ItemContainer itemContainer = Instantiate(itemContainerPrefab).GetComponentInChildren<ItemContainer>();
            itemContainer.Initialize(item);
            _playerInventory.PlaceItem(itemContainer, itemContainer.GetGridposition());
        }
    }
    private void LoadPlayerStash(object sender, List<BaseItem> stash)
    {
        CalculateTotalCurrency();
        //Debug.Log("Player Data Inv" + inventory.Count + " :" + this);
        foreach (BaseItem item in stash)
        {
            //Instantiate Item Container
            ItemContainer itemContainer = Instantiate(itemContainerPrefab).GetComponentInChildren<ItemContainer>();
            itemContainer.Initialize(item);
            _containerGrid.PlaceItem(itemContainer, itemContainer.GetGridposition());
        }
    }
    private void LoadPlayerPockets(object sender, List<BaseItem> pockets)
    {
        foreach (BaseItem item in pockets)
        {
            //Instantiate Item Container
            ItemContainer itemContainer = Instantiate(itemContainerPrefab).GetComponentInChildren<ItemContainer>();
            itemContainer.Initialize(item);
            _pocketSlots.PlaceItem(itemContainer, itemContainer.GetGridposition());
        }
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
    private void OnContainerItemAdded(object sender, OnItemChangedEventArgs e)
    {
        CalculateNewCurrency(e.item);
        AddItem(_containerList, e.item);
    }
    private void OnContainerMoved(object sender, OnItemChangedEventArgs e)
    {
        CalculateNewCurrency(e.item, true);
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
    
    private void InsertWorldItem(object sender, ItemWorldContainer e)
    {
        if (_playerInventory.InsertItem(e.GetItemContainer()))
        {
            Debug.Log($"Inserted {e.GetItemContainer().GetItem().name} !!");
            Destroy(e.gameObject);
        }   
    }

    #endregion
    
    private void InputReader_OpenInventory()
    {
        _inventoryUI.gameObject.SetActive(true);
    }
    
    private void InputReader_CloseInventory()
    {
        _inventoryUI.gameObject.SetActive(false);
        CloseLootGrid();
    }

    private void InputReader_PocketSelectionChanged(object sender, GridPosition e)
    {
        Debug.Log("Pocket SelectionChanged");
        //ItemContainer pocketItem = _pocketList.Find(item => item.GetGridposition() == e);
        ItemContainer pocketItem = _pocketSlots.GetContainer(e);
        
        if (pocketItem == null) return;
        
        _inventoryEvents.OnPocketItemSelected(pocketItem);
    }
    private void SendAmmo(int amount, ItemID bulletType)
    {
        List<Bullet> bulletsList = new List<Bullet>();
        
        List<ItemContainer> ammoContainers = _inventoryList.FindAll(item => item.GetItemType() == ItemType.Ammo);
        for (int i = 0; i < ammoContainers.Count; i++)
        {
            if (bulletsList.Count == amount) break;

            Bullet bullet = ammoContainers[i].GetItem() as Bullet;
            
            if(bullet == null || bullet.GetItemID() != bulletType) continue;
            
            int difference = bullet.GetAmount() - amount;
            //Debug.Log($"Bullet Amount: {bullet.GetAmount()}");
            if (difference > 0)
            {
                for (int bulletsToAdd = 0; bulletsToAdd < amount; bulletsToAdd++)
                {
                    bulletsList.Add(bullet.Copy());
                    if (bulletsList.Count == amount) break;
                }
                bullet.SetAmount(difference);
                ammoContainers[i].GetAmount();
                //Debug.Log($"NEW Bullet Amount: {bullet.GetAmount()}");
            }
            else if (difference < 0)
            {
                int amountToAdd = amount - Mathf.Abs(difference);
                //.Log($"Amount To Add {amountToAdd} {bullet.GetAmount()}");
                for (int bulletsToAdd = 0; bulletsToAdd < amountToAdd; bulletsToAdd++)
                {
                    bulletsList.Add(bullet.Copy());
                    if (bulletsList.Count == amount) break;
                }
                //Debug.Log($"BULLET REMOVED & DESTROYED");
                ammoContainers[i].ClearSlots();
                _inventoryList.Remove(ammoContainers[i]);
                Destroy(ammoContainers[i].transform.parent.gameObject);
            }
        }
        //Debug.Log($"Bullets To Send: {bulletsList.Count}");
        _inventoryEvents.OnSendAmmo(bulletsList);
    }
    
    private void OnSaveInventory()
    {
        Debug.Log("Sending Inventories!");
        
        for (int x = 0; x < _pocketSlots.GetWidth(); x++)
        {
            for (int y = 0; y < _pocketSlots.GetHeight(); y++)
            {
                ItemContainer itemContainer = _pocketSlots.GetContainer(new GridPosition(x, y));
                
                if (itemContainer == null) continue;
                
                _pocketList.Add(itemContainer);
            }
        }
        
        _inventoryEvents.OnSavePlayerInventory(_inventoryList, _pocketList);

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
        _playerEventChannel.SendPlayerPocketsEvent -= LoadPlayerPockets;
        
        //_inventoryEvents.InventorySpawned -= SetPlayerInventory;
        _inventoryEvents.OpenLootContainer -= OpenLootGrid;
        _inventoryEvents.PickedUpWorldItem -= InsertWorldItem;
        _inventoryEvents.RequestAmmo -= SendAmmo;

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
            item.gameObject.SetActive(true);
            if (!e.IsOpened())
            {
                _containerGrid.InsertItem(item);
            }
            else
            {
                _containerGrid.PlaceItem(item, item.GetGridposition());
            }
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
