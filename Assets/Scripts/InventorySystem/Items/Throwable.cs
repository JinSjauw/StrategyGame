using InventorySystem;
using UnityEngine;


namespace Items
{
    [CreateAssetMenu(menuName = "Items/Throwable")]
    public class Throwable : BaseItem
    {
        [SerializeField] private Sprite sprite;
        public float fuseTimer;
        public int turnTimer;
        public float radius;
        public int damage;
        public float throwVelocity = 2.5f;
        //ThrowAble config
        
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

