using System;
using System.Collections.Generic;
using Items;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.TestTools;

namespace InventorySystem.Grid
{
    public enum InventoryType
    {
        PlayerInventory = 0,
        PlayerStash = 1,
        LootInventory = 2,
        EquipmentSlot = 3,
        Hotbar = 4,
    }

    public enum SlotID
    {
        WeaponA = 0,
        WeaponB = 1,
        Helmet = 2,
        Armor = 3,
        
        //Maybe ID's for each pocket cell?
    }
    
    [RequireComponent(typeof(InventoryGridInteract))]
    public class InventoryGrid : MonoBehaviour
    {
        public const int TileSizeWidth = 64;
        public const int TileSizeHeight = 64;

        [SerializeField] private InventoryEvents _inventoryEvents;
        
        [SerializeField] private InventoryType _inventoryType;
        [SerializeField] private ItemType _acceptableItemType;
        [SerializeField] private SlotID _slotID;
        
        [SerializeField] private int _width;
        [SerializeField] private int _height;
        
        private GridSystem<InventorySlot> _inventoryGrid;
        private RectTransform _gridRect;

        public event EventHandler<OnItemChangedEventArgs> OnItemAdded;
        public event EventHandler<OnItemChangedEventArgs> OnItemMoved;

        private void Awake()
        {
            _gridRect = GetComponent<RectTransform>();
            Init(_width, _height);

            if (_inventoryType == InventoryType.PlayerInventory)
            {
                _inventoryEvents.OnPlayerInventorySpawned(this, _inventoryType);
            }
        }
        
        private void Init(int width, int height)
        {
            Vector2 size = new Vector2(width * TileSizeWidth, height * TileSizeHeight);
            _gridRect.sizeDelta = size;
            _inventoryGrid = new GridSystem<InventorySlot>(width, height, TileSizeWidth, 
                (GridPosition gridPosition, Vector3 worldPosition) => CreateInventorySlot(gridPosition, worldPosition));
        }
        
        private InventorySlot CreateInventorySlot(GridPosition gridPosition, Vector3 worldPosition)
        {
            return new InventorySlot(gridPosition, worldPosition);
        }
        
        private Vector2 _worldPosition = Vector2.zero;
        private GridPosition _gridPosition = new GridPosition();
        
        public RectTransform GetGridRect()
        {
            return _gridRect;
        }

        public InventoryType GetInventoryType()
        {
            return _inventoryType;
        }
        
        public Vector2 CalculateContainerPosition(ItemContainer itemContainer, GridPosition gridPosition)
        {
            Vector2 containerPosition = new Vector2();
            containerPosition.x = gridPosition.x * TileSizeWidth + TileSizeWidth * itemContainer.GetWidth() / 2;
            containerPosition.y = gridPosition.y * TileSizeHeight + TileSizeHeight * itemContainer.GetHeight() / 2;

            return containerPosition;
        }
        
        public GridPosition GetGridPosition(Vector2 mousePosition)
        {
            _worldPosition = new Vector2(
                mousePosition.x - _gridRect.transform.position.x,
                mousePosition.y - _gridRect.transform.position.y);

            _gridPosition.x = (int)_worldPosition.x / TileSizeWidth;
            _gridPosition.y = (int)_worldPosition.y / TileSizeHeight;
            
            return _gridPosition;
        }

        public bool PlaceItem(ItemContainer itemContainer, GridPosition gridPosition)
        {
            int itemWidth = itemContainer.GetWidth();
            int itemHeight = itemContainer.GetHeight();

            if (!_inventoryGrid.FitsOnGrid(gridPosition, itemWidth, itemHeight) && _inventoryType != InventoryType.EquipmentSlot)
            {
                Debug.Log("DOESNT FIT");
                return false;
            }
            
            if (_inventoryType == InventoryType.EquipmentSlot && _acceptableItemType == itemContainer.GetItemType())
            {
                gridPosition = new GridPosition(0, 0);
            }
            else if(_inventoryType == InventoryType.EquipmentSlot)
            {
                return false;
            }
            
            itemContainer.containerRect.SetParent(_gridRect);
            
            List<InventorySlot> slots = new List<InventorySlot>();

            for (int x = 0; x < itemWidth; x++)
            {
                for (int y = 0; y < itemHeight; y++)
                {
                    GridPosition slotPosition = new GridPosition(gridPosition.x + x, gridPosition.y + y);
                    InventorySlot slot = GetInventorySlot(slotPosition);
                    
                    if (slot.isOccupied() && slot.GetItemContainer() != itemContainer)
                    {
                        Debug.Log("Cell: " + slot.m_GridPosition + " Occupied By: " + slot.GetItemContainer().GetItem().name);
                        return false;
                    }
                    
                    slots.Add(slot);
                }
            }
            
            itemContainer.SetGridPosition(gridPosition);
            
            for (int i = 0; i < slots.Count; i++)
            {
                itemContainer.OccupySlot(slots[i]);
            }
            
            if (_inventoryType == InventoryType.EquipmentSlot && _acceptableItemType == itemContainer.GetItemType())
            {
                _inventoryEvents.OnEquipmentChanged(itemContainer, _acceptableItemType, _slotID);
            }
            
            itemContainer.containerRect.localPosition = CalculateContainerPosition(itemContainer, gridPosition);
            itemContainer.ShowBackground(true);
            OnItemAdded?.Invoke(this, new OnItemChangedEventArgs(itemContainer));
            
            return true;
        }

        public bool InsertItem(ItemContainer itemContainer)
        {
            GridPosition gridPosition = FindSpaceForObject(itemContainer);

            if (gridPosition.x == -1) { return false; }

            PlaceItem(itemContainer, new GridPosition(gridPosition.x, gridPosition.y));

            return true;
        }

        public GridPosition FindSpaceForObject(ItemContainer itemToInsert)
        {
            for (int x = 0; x < _width - itemToInsert.GetWidth() + 1; x++)
            {
                for (int y = 0; y < _height - itemToInsert.GetHeight() + 1; y++)
                {
                    if (_inventoryGrid.CheckOverlap(
                            new GridPosition(x, y),
                            itemToInsert.GetWidth() + 1,
                            itemToInsert.GetHeight() + 1))
                    {
                        return new GridPosition(x, y);
                    }
                }
            }
            return new GridPosition(-1, -1);
        }
        
        public ItemContainer PickupItem(GridPosition gridPosition)
        {   
            if(!_inventoryGrid.IsOnGrid(gridPosition)){ return null; }
            
            ItemContainer itemContainer = GetInventorySlot(gridPosition).GetItemContainer();
            
            if(itemContainer == null){ return null; }
            
            int itemWidth = itemContainer.GetWidth();
            int itemHeight = itemContainer.GetHeight();

            itemContainer.containerRect.SetParent(itemContainer.transform.root);
            for (int x = 0; x < itemWidth; x++)
            {
                for (int y = 0; y < itemHeight; y++)
                {
                    GridPosition slotPosition = new GridPosition(gridPosition.x + x, gridPosition.y + y);
                    Debug.Log(slotPosition);
                    if (!_inventoryGrid.IsOnGrid(slotPosition))
                    {
                        Debug.Log(slotPosition + " NOT ON GRID!");
                        continue;
                    }
                    itemContainer.ClearSlots();
                }
            }
            if (_inventoryType == InventoryType.EquipmentSlot)
            {
                _inventoryEvents.OnEquipmentChanged(null, ItemType.Empty, _slotID);
            }
            
            itemContainer.ShowBackground(false);
            OnItemMoved?.Invoke(this, new OnItemChangedEventArgs(itemContainer));
            
            return itemContainer;
        }
        
        public ItemContainer GetContainer(GridPosition gridPosition)
        {
            InventorySlot inventorySlot = GetInventorySlot(gridPosition);

            if (inventorySlot != null)
            {
                return GetInventorySlot(gridPosition).GetItemContainer();
            }

            return null;
        }

        public bool IsOnGrid(GridPosition gridPosition) => _inventoryGrid.IsOnGrid(gridPosition);
        public InventorySlot GetInventorySlot(GridPosition gridPosition) => _inventoryGrid.GetInventorySlot(gridPosition);
    }
    
}

