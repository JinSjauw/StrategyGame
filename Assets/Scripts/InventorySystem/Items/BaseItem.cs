using System;
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

    public enum ItemID
    {
        SingleOnly = 0,
        
        SmallAmmo = 1,
        MediumAmmo = 2,
        LargeAmmo = 3,
        
        Currency = 4,
        Bandage = 5,
        HealthKit = 6,
    }
    
    public abstract class BaseItem : ScriptableObject
    {
        [SerializeField] private int width;
        [SerializeField] private int height;
        [SerializeField] private ItemID _itemID;
        [SerializeField] private int value;
        private GridPosition _itemPosition;
        private int _amount = 1;
        private bool _isRotated = false;
        private bool _isInstanced = false;
        
        private void Awake()
        {
            _isInstanced = true;
        }
        public virtual int GetWidth() { return width; }
        public virtual int GetHeight() { return height; }
        public virtual int GetValue() { return value; }
        public virtual ItemID GetItemID() { return _itemID; }
        public virtual GridPosition GetItemPosition() { return _itemPosition; }
        public virtual bool IsRotated() { return _isRotated; }
        public virtual void Rotate(bool state) { _isRotated = state; }
        public virtual int GetAmount() { return _amount; }
        public virtual void SetAmount(int amount) { _amount = amount; }
        public virtual void SetItemPosition(GridPosition position) { _itemPosition = position; }
        public abstract ItemType GetItemType();
        public abstract Sprite GetSprite();

    }
}