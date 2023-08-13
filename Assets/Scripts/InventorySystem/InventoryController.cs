using System;
using System.Collections.Generic;
using CustomInput;
using InventorySystem;
using InventorySystem.Containers;
using InventorySystem.Grid;
using InventorySystem.Items;
using Items;
using Player;
using UnitSystem;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

[RequireComponent(typeof(InventoryHighlighter))]
public class InventoryController : MonoBehaviour
{
    [SerializeField] private InventoryEvents _inventoryEvents;
    //TEST
    [SerializeField] private List<BaseItem> itemsList;
    [SerializeField] private Transform itemContainerPrefab;
    [SerializeField] private Transform itemWorldContainerPrefab;
    
    [SerializeField] private InventoryGrid _selectedInventoryGrid;
    [SerializeField] private InventoryGrid _playerStash;
    [SerializeField] private bool _spawnStartItems;
    [SerializeField] private bool _canDropItem;
    [SerializeField] private bool _isShopping;

    [SerializeField] private PlayerHUDEvents _playerHUD;
    
    private InputReader _inputReader;
    private ItemContainer _selectedItem;
    private RectTransform _selectedItemTransform;
    
    private InventoryHighlighter _inventoryHighlighter;
    private ItemContainer _containerToHighlight;
    private GridPosition _lastHighlightPosition;
    
    private GridPosition _previousPosition;
    private bool _isDragging;
    private bool _rotated;

    private ItemContainer _selectedPocketItem;
    private PlayerUnit _playerUnit;


    private void Start()
    {
        SpawnStartItems();
    }

    private void SpawnStartItems()
    {
        if (!_spawnStartItems) return;
        
        for (int i = 0; i < itemsList.Count; i++)
        {
            ItemContainer spawnedItem = Instantiate(itemContainerPrefab).GetComponentInChildren<ItemContainer>();
            BaseItem randomItemData = itemsList[i];
            randomItemData = Instantiate(randomItemData);
            spawnedItem.Initialize(randomItemData);

            if (spawnedItem.GetItemType() == ItemType.Ammo || spawnedItem.GetItemType() == ItemType.Miscellaneous)
            {
                spawnedItem.AddAmount(64);
            }

            if (_playerStash != null && _playerStash.InsertItem(spawnedItem))
            {
                _spawnStartItems = false;
                continue;
            }

            Destroy(spawnedItem.gameObject);
        }
    }
    
    private void Update()
    {
        if (_isDragging)
        {
            _selectedItemTransform.position = Mouse.current.position.ReadValue();
        }
    }
    private void OnDestroy()
    {
        _inputReader.InventoryMouseMoveEvent -= OnMouseMoveEvent;
        _inputReader.InventoryClickStartEvent -= OnMouseClickStart;
        _inputReader.InventoryClickEndEvent -= OnMouseClickEnd;
        _inputReader.RotateItem -= OnRotate;
        _inputReader.SpawnItem -= OnSpawnItem;
        _inventoryEvents.PocketItemSelected -= PocketSelectionChanged;
    }
    private void OnMouseMoveEvent(object sender, MouseEventArgs e)
    {
        if (_selectedInventoryGrid == null)
        {
            return;
        }
        GridPosition gridPosition = _selectedInventoryGrid.GetGridPosition(e.MousePosition);
        
        if (_lastHighlightPosition == gridPosition && !_rotated)
        {
            _rotated = false;
            return;
        }
        if (_selectedItem == null)
        {
            _containerToHighlight = _selectedInventoryGrid.GetContainer(gridPosition);
            if (_containerToHighlight != null)
            {
                _inventoryHighlighter.Show(true);
                _inventoryHighlighter.SetSize(_containerToHighlight);
                _inventoryHighlighter.SetPosition(_selectedInventoryGrid, _containerToHighlight);
            }
            else
            {
                _inventoryHighlighter.Show(false);
            }
        }
        else if(_containerToHighlight != null)
        {
            GridPosition offSetGridposition = GetOffsetMousePosition(e.MousePosition);
            _inventoryHighlighter.Show(_selectedInventoryGrid.IsOnGrid(offSetGridposition));
            _inventoryHighlighter.SetSize(_containerToHighlight);
            _inventoryHighlighter.SetPosition(_selectedInventoryGrid, _containerToHighlight, offSetGridposition);
        }
        _lastHighlightPosition = gridPosition;
    }
    private GridPosition GetOffsetMousePosition(Vector2 mousePosition)
    {
        mousePosition.x -= (_selectedItem.GetWidth() - 1) * InventoryGrid.TileSizeWidth / 2;
        mousePosition.y -= (_selectedItem.GetHeight() - 1) * InventoryGrid.TileSizeHeight / 2;

        GridPosition gridPosition = _selectedInventoryGrid.GetGridPosition(mousePosition);

        return gridPosition;
    }
    private void OnMouseClickStart(object sender, MouseEventArgs e)
    {
        if (_selectedInventoryGrid == null)
        {
            Debug.Log(" NO INVENTORY GRID");
            return;
        }

        GridPosition gridPosition = _selectedInventoryGrid.GetGridPosition(e.MousePosition);
        
        if (_selectedItem == null)
        {
            _selectedItem = _selectedInventoryGrid.PickupItem(gridPosition);
            
            if(_selectedItem == null) { return; }

            if (_isShopping)
            {
                //Get transaction window for that inventory grid;
                //Insert item in that grid
                
                //What if the player wants to return item to the respecitve grid?
                
                //You click on an object and it gets inserted in the window + added to a list;
                //you click on it and it gets removed
            }
            
            _selectedItemTransform = _selectedItem.containerRect;
            _isDragging = true;

            _previousPosition = GetOffsetMousePosition(e.MousePosition);
        }
    }
    private void OnMouseClickEnd(object sender, MouseEventArgs e)
    {
        if (_selectedInventoryGrid == null)
        {
            DropItem();
            Debug.Log(" NO INVENTORY GRID");
            return;
        }
        
        if (_selectedItem != null)
        {
            GridPosition gridPosition = GetOffsetMousePosition(e.MousePosition);
            
            if (_selectedInventoryGrid.PlaceItem(_selectedItem, gridPosition))
            {
                _selectedItem = null;
                _isDragging = false;
            }
            else if(_selectedInventoryGrid.PlaceItem(_selectedItem, _previousPosition))
            {
                _selectedItem = null;
                _isDragging = false;
                //put it back in the previous location
            }
        }
    }
    private void OnRotate()
    {
        if (_selectedItem == null) { return; }
        _selectedItem.Rotate();
    }
    // ONLY FOR TESTING
    private void OnSpawnItem()
    {
        ItemContainer spawnedItem = Instantiate(itemContainerPrefab).GetComponentInChildren<ItemContainer>();
        BaseItem randomItemData = itemsList[Random.Range(0, itemsList.Count)];
        randomItemData = Instantiate(randomItemData);
        spawnedItem.Initialize(randomItemData);

        if (_selectedInventoryGrid != null)
        {
            if (!_selectedInventoryGrid.InsertItem(spawnedItem))
            {
                Destroy(spawnedItem.gameObject);
                Debug.Log("NO SPAAACCEE");
            }
        }
    }

    private void DropItem()
    {
        if (!_canDropItem || _selectedItem == null) return;
        
        //Spawn WorldItem
        ItemWorldContainer itemWorldContainer = Instantiate(itemWorldContainerPrefab).GetComponent<ItemWorldContainer>();
        itemWorldContainer.Initialize(_selectedItem, _playerUnit.transform.position);
        _selectedItem = null;
        _isDragging = false;
        //Send it flying off to somewhere
        
    }
    
    #region Public

    public void Initialize(InputReader inputReader, PlayerUnit playerUnit)
    {
        _inputReader = inputReader;
        _playerUnit = playerUnit;
        
        _inventoryHighlighter = GetComponent<InventoryHighlighter>();

        _inputReader.InventoryMouseMoveEvent += OnMouseMoveEvent;
        _inputReader.InventoryClickStartEvent += OnMouseClickStart;
        _inputReader.InventoryClickEndEvent += OnMouseClickEnd;
        _inputReader.RotateItem += OnRotate;
        _inputReader.SpawnItem += OnSpawnItem;

        _inventoryEvents.PocketItemSelected += PocketSelectionChanged;
    }
    
    private void PocketSelectionChanged(object sender, ItemContainer e)
    {
        Debug.Log(e.GetItem().name);
        _selectedPocketItem = e;
        _playerHUD.RaisePocketItemChanged(e.GetItem().GetSprite());
    }

    public void PickUpWorldItem(ItemWorldContainer itemWorldContainer)
    {
        //InventoryEvents.PickedUpWorldItem
        //Send WorldItemContainer
        _inventoryEvents.OnPickUpWorld(itemWorldContainer);
    }

    public ItemContainer GetPocketItem()
    {
        return _selectedPocketItem;
    }
    
    public void ClearPocketItem()
    {
        _selectedPocketItem = null;
    }
    
    public void OpenLootContainer(LootContainer lootContainer)
    {
        _inventoryEvents.OnOpenLootContainer(lootContainer);
    }
    
    public void SetInventory(InventoryGrid inventoryGrid)
    {
        _selectedInventoryGrid = inventoryGrid;
    }
    public void ClearInventory()
    {
        _selectedInventoryGrid = null;
    }
    
    
    #endregion


   
}
