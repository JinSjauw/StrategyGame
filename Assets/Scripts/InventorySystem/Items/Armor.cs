using InventorySystem;
using UnityEngine;

namespace Items
{
    [CreateAssetMenu(menuName = "Items/Armor")]
    public class Armor : BaseItem
    {
        [SerializeField] private Sprite iconSprite;
        [SerializeField] private Sprite worldSprite;
        [SerializeField] private ItemType armorType;
        [SerializeField] private float damageReduction;

        public float GetDamageReduction()
        {
            return damageReduction;
        }

        public Sprite GetWorldSprite()
        {
            return worldSprite;
        }
        
        public override ItemType GetItemType()
        {
            return armorType;
        }

        public override Sprite GetSprite()
        {
            return iconSprite;
        }
    }

}
