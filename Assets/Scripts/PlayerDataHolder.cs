using System.Collections.Generic;
using InventorySystem.Grid;
using Items;
using UnityEngine;

namespace Player
{
    [CreateAssetMenu(menuName = "PlayerDataHolder")]
    public class PlayerDataHolder : ScriptableObject
    {
        //Inventory List
        //Stash List
        //Equipment Slots

        [SerializeField] private List<BaseItem> _inventoryList;
        [SerializeField] private List<BaseItem> _stashList;
        [SerializeField] private List<BaseItem> _pocketList;

        private BaseItem _weaponA;
        private BaseItem _weaponB;

        private BaseItem _helmet;
        private BaseItem _armor;

        [SerializeField] private BaseItem[] _equipment;
        
        public void Reset()
        {
            Debug.Log("Reset!");
            _inventoryList.Clear();
            _stashList.Clear();
            _pocketList.Clear();
            
            _weaponA = null;
            _weaponB = null;

            _helmet = null;
            _armor = null;

            _equipment = new BaseItem[4];
        }
        
        public void SaveInventory(List<BaseItem> inventory)
        {
            _inventoryList = inventory;
        }

        public void SaveStash(List<BaseItem> stash)
        {
            _stashList = stash;
        }

        public void SavePockets(List<BaseItem> pockets)
        {
            _pocketList = pockets;
        }
        
        public void SaveEquipment(BaseItem item, SlotID slotID)
        {
            switch (slotID)
            {
                case SlotID.WeaponA:
                    _weaponA = item;
                    _equipment[0] = item;
                    break;
                case SlotID.WeaponB:
                    _weaponB = item;
                    _equipment[1] = item;
                    break;
                case SlotID.Helmet:
                    _helmet = item;
                    _equipment[2] = item;
                    break;
                case SlotID.Armor:
                    _armor = item;
                    _equipment[3] = item;
                    break;
            }
        }

        public List<BaseItem> GetInventory()
        {
            return _inventoryList;
        }
        public List<BaseItem> GetStash()
        {
            return _stashList;
        }

        public BaseItem[] GetEquipment()
        {
            return _equipment;
        }
        
        public List<BaseItem> GetPockets()
        {
            return _pocketList;
        }
        
        public BaseItem GetEquipment(SlotID slotID)
        {
            switch (slotID)
            {
                case SlotID.WeaponA:
                    return _weaponA;
                case SlotID.WeaponB:
                    return _weaponB;
                case SlotID.Helmet:
                    return _helmet;
                case SlotID.Armor:
                    return _armor;
                default:
                    return null;
            }
        }
    }
}