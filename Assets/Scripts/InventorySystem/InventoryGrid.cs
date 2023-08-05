using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InventorySystem.Grid
{
    [RequireComponent(typeof(InventoryGridInteract))]
    public class InventoryGrid : MonoBehaviour
    {
        private const int TileSizeWidth = 64;
        private const int TileSizeHeight = 64;

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

        public InventorySlot GetInventorySlot(GridPosition gridPosition) => _inventoryGrid.GetInventorySlot(gridPosition);
    }
}

