using UnitSystem;
using UnityEngine;

namespace Items
{
    public enum ItemType
    {
        Ammo = 0,
        Consumables = 1,
        Throwable = 2,
        Helmet = 3,
        Armor = 4,
        Weapon = 5,
        Misc = 6,
        Empty = 7,
    }
    
    public abstract class BaseItem : ScriptableObject
    {
        [SerializeField] private int width;
        [SerializeField] private int height;

        public virtual int GetWidth() { return width; }
        public virtual int GetHeight() { return height; }
        public abstract ItemType GetItemType();
        public abstract Sprite GetSprite();
        
    }
}