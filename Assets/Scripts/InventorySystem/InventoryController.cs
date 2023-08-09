using System;
using System.Collections.Generic;
using CustomInput;
using InventorySystem;
using InventorySystem.Containers;
using InventorySystem.Grid;
using Items;
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

    [SerializeField] private InventoryGrid _selectedInventoryGrid;

    private InputReader _inputReader;
    private ItemContainer _selectedItem;
    private RectTransform _selectedItemTransform;
    
    private InventoryHighlighter _inventoryHighlighter;
    private ItemContainer _containerToHighlight;
    private GridPosition _lastHighlightPosition;
    
    private GridPosition _previousPosition;
    private bool _isDragging;
    private bool _rotated;
    
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
            
            _selectedItemTransform = _selectedItem.containerRect;
            _isDragging = true;

            _previousPosition = GetOffsetMousePosition(e.MousePosition);
        }
    }

    private void OnMouseClickEnd(object sender, MouseEventArgs e)
    {
        if (_selectedInventoryGrid == null)
        {
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
        ItemContainer spawnedItem = Instantiate(itemContainerPrefab).GetComponent<ItemContainer>();
        BaseItem randomItemData = itemsList[Random.Range(0, itemsList.Count)];
        randomItemData = Instantiate(randomItemData);
        spawnedItem.Initialize(randomItemData);

        if (_selectedInventoryGrid != null)
        {
            if (!_selectedInventoryGrid.PlaceItem(spawnedItem, new GridPosition(Random.Range(0, 4), Random.Range(0, 4))))
            {
                Destroy(spawnedItem.gameObject);
                Debug.Log("Oops no space!");
            }
        }
    }
    
    #region Public

    public void Initialize(InputReader inputReader)
    {
        _inputReader = inputReader;
        _inventoryHighlighter = GetComponent<InventoryHighlighter>();

        _inputReader.InventoryMouseMoveEvent += OnMouseMoveEvent;
        _inputReader.InventoryClickStartEvent += OnMouseClickStart;
        _inputReader.InventoryClickEndEvent += OnMouseClickEnd;
        _inputReader.RotateItem += OnRotate;
        _inputReader.SpawnItem += OnSpawnItem;
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
