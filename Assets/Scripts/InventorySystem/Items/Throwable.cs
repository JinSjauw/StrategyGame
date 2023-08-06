using InventorySystem;
using UnityEngine;


namespace Items
{
    [CreateAssetMenu(menuName = "Items/Throwable")]
    public class Throwable : BaseItem
    {
        [SerializeField] private Sprite sprite;
    
        public override ItemType GetItemType()
        {
            return ItemType.Throwable;
        }

        public override Sprite GetSprite()
        {
            return sprite;
        }
    }
}

