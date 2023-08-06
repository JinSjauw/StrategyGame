using System.Collections.Generic;
using UnityEngine;

namespace InventorySystem.Grid
{
    [RequireComponent(typeof(InventoryGridInteract))]
    public class InventoryGrid : MonoBehaviour
    {
        public const int TileSizeWidth = 64;
        public const int TileSizeHeight = 64;

        [SerializeField] private int _width;
        [SerializeField] private int _height;
        
        private GridSystem<InventorySlot> _inventoryGrid;

        private RectTransform _gridRect;
        private void Awake()
        {
            _gridRect = GetComponent<RectTransform>();
            Init(_width, _height);
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
            itemContainer.containerRect.SetParent(_gridRect);

            int itemWidth = itemContainer.GetWidth();
            int itemHeight = itemContainer.GetHeight();

            if (!_inventoryGrid.FitsOnGrid(gridPosition, itemWidth, itemHeight))
            {
                Debug.Log("DOESNT FIT");
                return false;
            }

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

            for (int i = 0; i < slots.Count; i++)
            {
                itemContainer.OccupySlot(slots[i]);
            }
            
            Vector2 itemPosition = new Vector2();
            itemPosition.x = gridPosition.x * TileSizeWidth + TileSizeWidth * itemContainer.GetWidth() / 2;
            itemPosition.y = gridPosition.y * TileSizeHeight + TileSizeHeight * itemContainer.GetHeight() / 2;

            itemContainer.containerRect.localPosition = itemPosition;

            return true;
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

            return itemContainer;
        }
        
        public InventorySlot GetInventorySlot(GridPosition gridPosition) => _inventoryGrid.GetInventorySlot(gridPosition);

        
    }
}

