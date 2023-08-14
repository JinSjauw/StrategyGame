using System;
using System.Collections;
using System.Collections.Generic;
using InventorySystem;
using InventorySystem.Grid;
using Items;
using Unity.VisualScripting;
using UnityEngine;

public class ShopController : MonoBehaviour
{
    [SerializeField] private InventoryGrid shopGrid;
    [SerializeField] private List<BaseItem> _stock;
    [SerializeField] private Transform _itemContainerPrefab;
    private bool hasSpawned = false;
    
    private void Awake()
    {
        shopGrid.Initialize();
    }

    private void Start()
    {
        SpawnStartItems();
    }

    private void SpawnStartItems()
    {
        if (hasSpawned) return;
        
        for (int i = 0; i < _stock.Count; i++)
        {
            ItemContainer spawnedItem = Instantiate(_itemContainerPrefab).GetComponentInChildren<ItemContainer>();
            BaseItem randomItemData = _stock[i];
            randomItemData = Instantiate(randomItemData);
            spawnedItem.Initialize(randomItemData);

            if (spawnedItem.GetItemType() == ItemType.Ammo)
            {
                spawnedItem.AddAmount(64);
            }

            if (shopGrid != null && shopGrid.InsertItem(spawnedItem))
            {
                hasSpawned = false;
                continue;
            }

            Destroy(spawnedItem.gameObject);
        }
        
    }
}
