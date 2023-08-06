using Items;
using UnityEngine;

namespace InventorySystem
{
    public class InventorySlot
    {
        public GridPosition m_GridPosition;
        public Vector2 m_WorldPosition;

        private ItemContainer _itemContainer;

        private bool _isOccupied;
        //Item;

        public InventorySlot(GridPosition gridPosition, Vector2 worldPosition)
        {
            m_GridPosition = gridPosition;
            m_WorldPosition = worldPosition;
        }

        public ItemContainer GetItemContainer()
        {
            return _itemContainer;
        }
        
        public void OccupySlot(ItemContainer itemContainer)
        {
            _itemContainer = itemContainer;
            _isOccupied = true;
        }

        public void ClearSlot()
        {
            if (_itemContainer != null)
            {
                Debug.Log("Cleared: " + m_GridPosition + " From: " + _itemContainer.GetItem().name);
                _itemContainer = null;
            }
           
            _isOccupied = false;
        }
        
        public bool isOccupied()
        {
            return _isOccupied;
        }
    }
}