using System;
using System.Collections.Generic;
using CustomInput;
using InventorySystem;
using InventorySystem.Grid;
using Items;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class InventoryController : MonoBehaviour
{
    [SerializeField] private InventoryGrid selectedInventoryGrid;
    
    //TEST
    [SerializeField] private List<BaseItem> itemsList;
    [SerializeField] private Transform itemContainerPrefab;
    
    private InputReader _inputReader;
    private ItemContainer _selectedItem;
    private RectTransform _selectedItemTransform;
    private GridPosition _previousPosition;
    private bool _isDragging;

    private void Update()
    {
        if (_isDragging)
        {
            _selectedItemTransform.position = Mouse.current.position.ReadValue();
        }
    }

    private GridPosition GetOffsetMousePosition(Vector2 mousePosition)
    {
        mousePosition.x -= (_selectedItem.GetWidth() - 1) * InventoryGrid.TileSizeWidth / 2;
        mousePosition.y -= (_selectedItem.GetHeight() - 1) * InventoryGrid.TileSizeHeight / 2;

        GridPosition gridPosition = selectedInventoryGrid.GetGridPosition(mousePosition);

        return gridPosition;
    }
    
    private void OnMouseClickStart(object sender, ClickEventArgs e)
    {
        if (selectedInventoryGrid == null)
        {
            Debug.Log(" NO INVENTORY GRID");
            return;
        }

        GridPosition gridPosition = selectedInventoryGrid.GetGridPosition(e.m_Target);

        if (_selectedItem == null)
        {
            _selectedItem = selectedInventoryGrid.PickupItem(gridPosition);
            
            if(_selectedItem == null) { return; }
            
            _selectedItemTransform = _selectedItem.containerRect;
            _isDragging = true;

            _previousPosition = GetOffsetMousePosition(e.m_Target);
        }
    }

    private void OnMouseClickEnd(object sender, ClickEventArgs e)
    {
        if (selectedInventoryGrid == null)
        {
            Debug.Log(" NO INVENTORY GRID");
            return;
        }
        
        if (_selectedItem != null)
        {
            GridPosition gridPosition = GetOffsetMousePosition(e.m_Target);
            
            if (selectedInventoryGrid.PlaceItem(_selectedItem, gridPosition))
            {
                _selectedItem = null;
                _isDragging = false;
            }
            else if(selectedInventoryGrid.PlaceItem(_selectedItem, _previousPosition))
            {
                _selectedItem = null;
                _isDragging = false;
                //put it back in the previous location
            }
        }
    }

    private void OnRotate()
    {
        
    }
    // ONLY FOR TESTING
    private void OnSpawnItem()
    {
        ItemContainer spawnedItem = Instantiate(itemContainerPrefab).GetComponent<ItemContainer>();
        BaseItem randomItemData = itemsList[Random.Range(0, itemsList.Count)];
        spawnedItem.Initialize(randomItemData);

        if (selectedInventoryGrid != null)
        {
            if (!selectedInventoryGrid.PlaceItem(spawnedItem, new GridPosition(Random.Range(0, 4), Random.Range(0, 4))))
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
        
        _inputReader.InventoryClickStartEvent += OnMouseClickStart;
        _inputReader.InventoryClickEndEvent += OnMouseClickEnd;
        _inputReader.RotateItem += OnRotate;
        _inputReader.SpawnItem += OnSpawnItem;
    }

    public void SetInventory(InventoryGrid inventoryGrid)
    {
        selectedInventoryGrid = inventoryGrid;
    }

    public void ClearInventory()
    {
        selectedInventoryGrid = null;
    }
    
    #endregion
    
   
    
}
