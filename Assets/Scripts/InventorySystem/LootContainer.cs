using System.Collections.Generic;
using Items;
using Unity.VisualScripting;
using UnityEngine;

namespace InventorySystem.Containers
{
    public class LootContainer : MonoBehaviour
    {
        [SerializeField] private int width;
        [SerializeField] private int height;

        [SerializeField] private int maxLootAmount;

        [SerializeField] private ItemContainer itemContainerPrefab;
        [SerializeField] private bool _isOpened;
        [SerializeField] private List<BaseItem> itemPool;
        [SerializeField] private List<ItemContainer> itemList = new List<ItemContainer>();
        
        private void Generateloot()
        {
            int amount = Random.Range(1, maxLootAmount);

            for (int i = 0; i < amount; i++)
            {
                ItemContainer spawnedItem = Instantiate(itemContainerPrefab).GetComponent<ItemContainer>();
                BaseItem randomItemData = itemPool[Random.Range(0, itemPool.Count)];
                randomItemData = Instantiate(randomItemData);
                spawnedItem.Initialize(randomItemData);
                
                itemList.Add(spawnedItem);
            }
        }
        
        public Vector2 GetSize()
        {
            return new Vector2(width, height);
        }
        public List<ItemContainer> GetLootList()
        {
            if (!_isOpened)
            {
                Generateloot();
            }
            return itemList;
        }

        public void SetItemList(List<ItemContainer> newItemList)
        {
            itemList = newItemList;
        }

        public void SetOpen()
        {
            _isOpened = true;
        }
        
        public bool IsOpened()
        {
            return _isOpened;
        }

    }
}

