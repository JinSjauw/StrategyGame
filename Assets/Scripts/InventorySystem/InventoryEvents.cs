using System;
using System.Collections.Generic;
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

        public event EventHandler<List<ItemContainer>> SavePlayerInventory;
        public event EventHandler<List<ItemContainer>> SavePlayerStash;
        public event EventHandler<EquipmentEventArgs> SaveEquipment; 

        public void OnEquipmentChanged(ItemContainer itemContainer, ItemType slotType, SlotID slotID)
        {
            EquipmentChanged?.Invoke(this, new EquipmentEventArgs(itemContainer, slotType, slotID));
            OnSaveEquipment(itemContainer, slotType, slotID);
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
        public void OnSavePlayerInventory(List<ItemContainer> list)
        {
            SavePlayerInventory?.Invoke(this, list);
        }
        public void OnSavePlayerStash(List<ItemContainer> list)
        {
            SavePlayerStash?.Invoke(this, list);
        }
        public void OnSaveEquipment(ItemContainer itemContainer, ItemType slotType, SlotID slotID)
        {
            SaveEquipment?.Invoke(this, new EquipmentEventArgs(itemContainer, slotType, slotID));
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