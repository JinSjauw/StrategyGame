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

        public event EventHandler<List<BaseItem>> SavePlayerInventory;
        public event EventHandler<List<BaseItem>> SavePlayerStash;
        public event EventHandler<EquipmentEventArgs> SaveEquipment;
        public event EventHandler<List<BaseItem>> SavePlayerPockets;

        public event EventHandler<ItemContainer> PocketItemSelected;
        

        public void OnPocketItemSelected(ItemContainer itemContainer)
        {
            PocketItemSelected?.Invoke(this, itemContainer);
        }
        
        
        public void OnEquipmentChanged(ItemContainer itemContainer, ItemType slotType, SlotID slotID)
        {
            BaseItem item = null;
            if (itemContainer != null)
            {
                item = itemContainer.GetItem();
            }
            Debug.Log($"item: {item} {slotType} {slotID}");
            EquipmentChanged?.Invoke(this, new EquipmentEventArgs(item, slotType, slotID));
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
        public void OnSavePlayerInventory(List<ItemContainer> list, List<ItemContainer> pockets)
        {
            List<BaseItem> itemsList = new List<BaseItem>();
            for (int i = 0; i < list.Count; i++)
            {
                BaseItem item = list[i].GetItem();
                itemsList.Add(item);
            }
            SavePlayerInventory?.Invoke(this, itemsList);
            
            itemsList = new List<BaseItem>();
            for (int i = 0; i < pockets.Count; i++)
            {
                BaseItem item = pockets[i].GetItem();
                itemsList.Add(item);
            }
            Debug.Log($"Pocket List: {itemsList.Count}");
            SavePlayerPockets?.Invoke(this, itemsList);
            
        }
        public void OnSavePlayerStash(List<ItemContainer> list)
        {
            List<BaseItem> itemsList = new List<BaseItem>();
            for (int i = 0; i < list.Count; i++)
            {
                BaseItem item = list[i].GetItem();
                itemsList.Add(item);
            }
            SavePlayerStash?.Invoke(this, itemsList);
        }
        public void OnSaveEquipment(ItemContainer itemContainer, ItemType slotType, SlotID slotID)
        {
            BaseItem itemToSend = null;
            if (itemContainer != null)
            {
                itemToSend = itemContainer.GetItem();
            }
            SaveEquipment?.Invoke(this, new EquipmentEventArgs(itemToSend, slotType, slotID));
        }
    }
    
    #region Event Args
    
        public class EquipmentEventArgs : EventArgs
        {
            public BaseItem item;
            public ItemType itemType;
            public SlotID slotID;

            public EquipmentEventArgs(BaseItem baseItem, ItemType type, SlotID id)
            {
                item = baseItem;
                itemType = type;
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