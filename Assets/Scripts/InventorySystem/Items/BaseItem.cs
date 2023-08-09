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
    
    public abstract class BaseItem : ScriptableObject
    {
        [SerializeField] private int width;
        [SerializeField] private int height;
        private GridPosition _itemPosition;
        private bool _isRotated = false;
        private bool _isInstanced = false;
        
        private void Awake()
        {
            _isInstanced = true;
        }
        //Remove copy function from weapon
        public virtual bool IsInstanced() { return _isInstanced; }
        public virtual int GetWidth() { return width; }
        public virtual int GetHeight() { return height; }
        public virtual GridPosition GetItemPosition() { return _itemPosition; }
        public virtual bool IsRotated() { return _isRotated; }
        public virtual void Rotate(bool state) { _isRotated = state; }
        public virtual void SetItemPosition(GridPosition position) { _itemPosition = position; }
        public abstract ItemType GetItemType();
        public abstract Sprite GetSprite();

    }
}