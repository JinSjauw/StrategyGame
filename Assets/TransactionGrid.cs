using System;
using System.Collections.Generic;
using InventorySystem;
using InventorySystem.Grid;
using Items;
using UnityEngine;

public class TransactionGrid : MonoBehaviour
{
    [SerializeField] private InventoryGrid transactionGrid;
    private List<ItemContainer> _transactionItems;

    [SerializeField] private BaseItem currencyItem;
    [SerializeField] private Transform itemContainerPrefab;
    
    private void Awake()
    {
        _transactionItems = new List<ItemContainer>();
        transactionGrid.Initialize();
        transactionGrid.OnItemAdded += OnTransactionItemAdded;
        transactionGrid.OnItemMoved += OnTransactionItemMoved;
    }

    private void OnDestroy()
    {
        transactionGrid.OnItemAdded -= OnTransactionItemAdded;
        transactionGrid.OnItemMoved -= OnTransactionItemMoved;
    }

    private void OnTransactionItemMoved(object sender, OnItemChangedEventArgs e)
    {
        RemoveItem(_transactionItems, e.item);
    }

    private void OnTransactionItemAdded(object sender, OnItemChangedEventArgs e)
    {
        AddItem(_transactionItems, e.item);
    }
    
    private void AddItem(List<ItemContainer> addToList, ItemContainer itemContainer)
    {
        if (!addToList.Contains(itemContainer))
        {
            addToList.Add(itemContainer);
        }
    }
    private void RemoveItem(List<ItemContainer> removeFromList, ItemContainer itemContainer)
    {
        if (removeFromList.Contains(itemContainer))
        {
            removeFromList.Remove(itemContainer);
        }
    }

    public void CalculateDeal()
    {
        int totalValue = 0;
        for (int i = 0; i < _transactionItems.Count; i++)
        {
            totalValue += _transactionItems[i].GetValue();
            _transactionItems[i].ClearSlots();
            Destroy(_transactionItems[i].transform.parent.gameObject);
        }
        _transactionItems.Clear();

        ItemContainer currencyContainer = Instantiate(itemContainerPrefab).GetComponentInChildren<ItemContainer>();
        currencyContainer.Initialize(Instantiate(currencyItem));
        currencyContainer.AddAmount(totalValue);
        
        transactionGrid.InsertItem(currencyContainer);
    }
}
