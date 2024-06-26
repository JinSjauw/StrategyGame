using System;
using CustomInput;
using InventorySystem;
using InventorySystem.Grid;
using Items;
using UnitSystem;
using UnityEngine;

public class LoadoutSystem : MonoBehaviour
{
    [SerializeField] private InventoryEvents _inventoryEvents;
    private InputReader _inputReader;
    
    [SerializeField] private Weapon[] _equippedWeapons = new Weapon[2];
    private int _equippedIndex;
    
    //private Helmet equippedHelmet;
    private Armor _equippedArmor;
    
    private PlayerUnit _playerUnit;

    private Action _OnChange;
    
    private void OnDestroy()
    {
        _inventoryEvents.EquipmentChanged -= UpdateEquipment;
        _inputReader.PlayerScrollEvent -= SwitchWeapons;
    }

    private void UpdateEquipment(object sender, EquipmentEventArgs e)
    {
        Debug.Log($"Updating Equipment {e.item} {e.itemType} {e.slotID}");
        SetEquipment(e.item, e.itemType, e.slotID);
    }

    public void SetEquipment(BaseItem item, ItemType itemType, SlotID slotID)
    {
        Debug.Log($"Setting Equip, Weapon: {item} : ItemType: {itemType} SlotID: {slotID}");
        if (itemType == ItemType.Empty)
        {
            item = null;
        }
        
        switch (slotID)
        {
            case SlotID.WeaponA:
                EquipWeapon(item, 0);
                break;
            case SlotID.WeaponB:
                EquipWeapon(item, 1);
                break;
            case SlotID.Armor:
                break;
            case SlotID.Helmet:
                break;
        }
    }

    private void EquipWeapon(BaseItem toEquip, int equipIndex)
    {
        Debug.Log($"ToEquip {toEquip}");
        Weapon weaponToEquip = toEquip as Weapon;
        if (weaponToEquip != null)
        {
            _equippedWeapons[equipIndex] = weaponToEquip;
        }
        else if(weaponToEquip == null)
        {
            _equippedWeapons[equipIndex] = null;
            if (equipIndex == _equippedIndex)
            {
                _playerUnit.UnEquip();
            }
            Debug.Log("Not A WEAPON!??");
        }
        
        //Update PlayerUnit
        if (_equippedIndex == equipIndex)
        {
            _playerUnit.EquipWeapon(weaponToEquip);
            _OnChange();
        }
    }

    //Listen to input event (Mouse Scroll up/down)
    private void SwitchWeapons(object sender, int equippedIndex)
    {
        Weapon weaponToEquip = _equippedWeapons[equippedIndex];
        if (weaponToEquip != null)
        {
            _playerUnit.EquipWeapon(weaponToEquip);
            _OnChange();
        }
    }

    public void Initialize(PlayerUnit playerUnit, InputReader inputReader, Action onWeaponChanged)
    {
        _playerUnit = playerUnit;
        _inputReader = inputReader;
        _OnChange = onWeaponChanged;

        _inventoryEvents.EquipmentChanged += UpdateEquipment;
        _inputReader.PlayerScrollEvent += SwitchWeapons;
    }
}
