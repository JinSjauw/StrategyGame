using System;
using UnitSystem;
using UnityEngine;

namespace InventorySystem
{
    [CreateAssetMenu(menuName = "Inventory/Items/EquippableItem")]
    public class EquippableItem : BaseItem
    {
        [SerializeField] private Weapon _weapon;
        //[SerializeField] private Armor _armor;

        public override void Initialize()
        {
            Debug.Log("WOWOW:" + itemData.name);
            Weapon newWeapon = itemData as Weapon;
            _weapon = newWeapon;
            Debug.Log("Weapon: " + newWeapon.name + " " + newWeapon.Recoil);
        }

        public override void Use(Unit unit)
        {
            if (itemType == ItemType.Weapon)
            {
                //If it is drag and dropped
                //Switch current item with this one || or add back randomly into inventory
                //unit.weapon = this.weapon;
            }

            if (itemType == ItemType.Armor)
            {
                //Switch current item with this one, Armor should be the same size (only Chest pieces)
                //unit.armor = this.armor;
            }
        }
    }
}