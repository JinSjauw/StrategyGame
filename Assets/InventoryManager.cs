using System.Collections.Generic;
using InventorySystem;
using InventorySystem.Grid;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private InventoryGrid mainInventory;
    [SerializeField] private InventoryGrid lootInventory;
    
    //List that holds all the items;
    [SerializeField] private List<ItemContainer> _itemList = new List<ItemContainer>();
    
    
    private void Awake()
    {
        mainInventory.OnItemAdded += OnItemAdded;
        mainInventory.OnItemMoved += OnItemMoved;
    }

    private void OnItemAdded(object sender, OnItemChangedEventArgs e)
    {
        _itemList.Add(e.item);
    }
    private void OnItemMoved(object sender, OnItemChangedEventArgs e)
    {
        _itemList.Remove(e.item);
    }
    public void ShowInventory(bool state)
    {
        gameObject.SetActive(state);
    }
}
