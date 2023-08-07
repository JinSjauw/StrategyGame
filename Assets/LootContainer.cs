using System.Collections.Generic;
using Items;
using UnityEngine;

namespace InventorySystem.Containers
{
    public class LootContainer : MonoBehaviour
    {
        [SerializeField] private int width;
        [SerializeField] private int height;

        [SerializeField] private List<BaseItem> itemPool;

        public Vector2 GetSize()
        {
            return new Vector2(width, height);
        }

        public List<BaseItem> ItemPool()
        {
            return itemPool;
        }
    }
}

