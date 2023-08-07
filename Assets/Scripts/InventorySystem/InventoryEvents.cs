using System;
using InventorySystem.Containers;
using InventorySystem.Grid;
using Items;
using UnityEngine;

namespace InventorySystem
{
    [CreateAssetMenu(menuName = "Events/InventoryEventReader")]
    public class InventoryEvents : ScriptableObject
    {
        public event EventHandler<InventoryEventArgs> InventorySpawned;
        public event EventHandler<EquipmentEventArgs> EquipmentChanged;
        public event EventHandler<LootContainer> OpenLootContainer;

        public void OnEquipmentChanged(ItemContainer itemContainer, ItemType slotType, SlotID slotID)
        {
            EquipmentChanged?.Invoke(this, new EquipmentEventArgs(itemContainer, slotType, slotID));
        }
        public void OnPlayerInventorySpawned(InventoryGrid inventory, InventoryType type)
        {
            InventorySpawned?.Invoke(inventory, new InventoryEventArgs(inventory, type));
        }
        public void OnPlayerStashSpawned(InventoryGrid inventory, InventoryType type)
        {
            InventorySpawned?.Invoke(inventory, new InventoryEventArgs(inventory, type));
        }
        public void OnOpenLootContainer(LootContainer lootContainer)
        {
            OpenLootContainer?.Invoke(this, lootContainer);
        }
    }
    
    #region Event Args
    
        public class EquipmentEventArgs : EventArgs
        {
            public ItemContainer itemContainer;
            public ItemType slotType;
            public SlotID slotID;

            public EquipmentEventArgs(ItemContainer item, ItemType type, SlotID id)
            {
                itemContainer = item;
                slotType = type;
                slotID = id;
            }
        }
        public class InventoryEventArgs : EventArgs
        {
            public InventoryGrid inventory;
            public InventoryType type;
            
            public InventoryEventArgs(InventoryGrid spawnedInventory, InventoryType inventoryType)
            {
                inventory = spawnedInventory;
                type = inventoryType;
            }
        }
        
    #endregion
}