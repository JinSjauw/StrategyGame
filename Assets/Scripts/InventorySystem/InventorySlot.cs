using Items;
using UnityEngine;

namespace InventorySystem
{
    public class InventorySlot
    {
        public GridPosition m_GridPosition;
        public Vector2 m_WorldPosition;

        private BaseItem _baseItem;
        
        //Item;

        public InventorySlot(GridPosition gridPosition, Vector2 worldPosition)
        {
            m_GridPosition = gridPosition;
            m_WorldPosition = worldPosition;
        }
    }
}