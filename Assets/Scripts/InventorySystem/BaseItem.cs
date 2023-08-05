using UnitSystem;
using UnityEngine;

namespace InventorySystem
{
    public abstract class BaseItem : ScriptableObject
    {
        [SerializeField] private Sprite itemSprite;
        [SerializeField] private int width;
        [SerializeField] private int height;

        public abstract void Use(Unit unit);
    }
}