using System;
using InventorySystem.Grid;
using Items;
using UnityEngine;

namespace InventorySystem
{
    [CreateAssetMenu(menuName = "Events/InventoryEventReader")]
    public class InventoryEvents : ScriptableObject
    {
        public event EventHandler<InventoryEventArgs> InventorySpawned;
        public event EventHandler<InventoryControllerEventArgs> ControllerSpawned;
        public event EventHandler<EquipmentEventArgs> EquipmentChanged;

        public void OnEquipmentChanged(ItemContainer itemContainer, ItemType slotType)
        {
            EquipmentChanged?.Invoke(this, new EquipmentEventArgs(itemContainer, slotType));
        }
        public void OnPlayerInventorySpawned(InventoryGrid inventory, InventoryType type)
        {
            InventorySpawned?.Invoke(inventory, new InventoryEventArgs(inventory, type));
        }
        public void OnPlayerStashSpawned(InventoryGrid inventory, InventoryType type)
        {
            InventorySpawned?.Invoke(inventory, new InventoryEventArgs(inventory, type));
        }
        public void OnInventoryControllerSpawned(InventoryController controller)
        {
            Debug.Log(controller.name + " In InventoryEvents");
            ControllerSpawned?.Invoke(this, new InventoryControllerEventArgs(controller));
        }
    }

    public class EquipmentEventArgs : EventArgs
    {
        public ItemContainer itemContainer;
        public ItemType slotType;

        public EquipmentEventArgs(ItemContainer item, ItemType type)
        {
            itemContainer = item;
            slotType = type;
        }
    }
    
    public class InventoryControllerEventArgs : EventArgs
    {
        public InventoryController controller;

        public InventoryControllerEventArgs(InventoryController inventoryController)
        {
            controller = inventoryController;
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
}