using System.Collections.Generic;
using InventorySystem;
using InventorySystem.Grid;
using Items;
using UnityEngine;

namespace Player
{
    public class PlayerDataManager : MonoBehaviour
    {
        [SerializeField] private InventoryEvents _inventory;
        [SerializeField] private PlayerDataHolder _playerData;
        [SerializeField] private PlayerEventChannel _playerEventChannel;

        private void Awake()
        {
            _playerData.Reset();
            
            _inventory.SaveEquipment += SaveEquipment;
            _inventory.SavePlayerInventory += SaveInventory;
            _inventory.SavePlayerStash += SaveStash;

            _playerEventChannel.PlayerSpawnRequest += OnPlayerSpawnRequest;
            _playerEventChannel.PlayerSpawnedEvent += OnPlayerSpawned;

            _playerEventChannel.OnMainMenuRequest += OnMainMenuEnter;
        }

        private void OnPlayerSpawned()
        {
            _playerEventChannel.OnPlayerInventoryRequest(_playerData.GetInventory(), _playerData.GetEquipment());
        }

        private void OnMainMenuEnter()
        {
            //_playerEventChannel.OnPlayerInventoryRequest();
            _playerEventChannel.OnMenuInventoryRequest(_playerData.GetEquipment(), _playerData.GetInventory(), _playerData.GetStash());
        }

        private void SaveEquipment(object sender, EquipmentEventArgs e)
        {
            _playerData.SaveEquipment(e.item, e.slotID);
        }
        private void SaveInventory(object sender, List<BaseItem> e)
        {
            Debug.Log("saving inventories in MainMenu");
            _playerData.SaveInventory(e);
        }
        private void SaveStash(object sender, List<BaseItem> e)
        {
            _playerData.SaveStash(e);
        }
        
        private void OnPlayerSpawnRequest()
        {
            _playerEventChannel.OnPlayerSpawnRequest(_playerData.GetEquipment(SlotID.WeaponA) as Weapon);
        }
    }

}
