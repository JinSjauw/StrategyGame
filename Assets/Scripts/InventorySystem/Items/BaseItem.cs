using UnitSystem;
using UnityEngine;

namespace InventorySystem
{
    public enum ItemType
    {
        Ammo = 0,
        Consumables = 1,
        Grenades = 2,
        Armor = 3,
        Weapon = 4,
        Misc = 5,
    }
    
    public abstract class BaseItem : ScriptableObject
    {
        [SerializeField] private Sprite itemSprite;
        [SerializeField] private int width;
        [SerializeField] private int height;

        [SerializeField] protected ItemType itemType;
        [SerializeField] protected ScriptableObject itemData;
        
        //Item configs?
        //Healing Amount
        //Durability Restore?
        //Damage?

        public virtual void Initialize(){}
        public abstract void Use(Unit unit);
    }
}