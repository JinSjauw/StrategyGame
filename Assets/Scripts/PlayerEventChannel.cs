using System;
using System.Collections.Generic;
using Items;
using UnityEngine;
using UnityEngine.Events;

namespace Player
{
    [CreateAssetMenu(menuName = "Player Event Channel")]
    public class PlayerEventChannel : ScriptableObject
    {
        public event EventHandler<Weapon> SendPlayerDataEvent;
        public event EventHandler<List<BaseItem>> SendPlayerInventoryEvent;
        public event EventHandler<BaseItem[]> SendPlayerEquipmentEvent;
        public event EventHandler<List<BaseItem>> SendStashInventoryEvent; 
        
        public event UnityAction OnMainMenuRequest = delegate {  };
        public event UnityAction PlayerSpawnRequest = delegate {  };
        public event UnityAction PlayerSpawnedEvent = delegate {  };
        
        public void RequestPlayerSpawn()
        {
            PlayerSpawnRequest.Invoke();
        }

        public void PlayerSpawned()
        {
            PlayerSpawnedEvent.Invoke();
        }
        
        public void OnPlayerSpawnRequest(Weapon weapon)
        {
            SendPlayerDataEvent?.Invoke(this, weapon);
        }

        public void OnPlayerInventoryRequest(List<BaseItem> inventory, BaseItem[] equipment)
        {
            SendPlayerInventoryEvent?.Invoke(this, inventory);
            SendPlayerEquipmentEvent?.Invoke(this, equipment);
        }

        public void EnterMainMenu()
        {
            OnMainMenuRequest.Invoke();
        }
        
        public void OnMenuInventoryRequest(BaseItem[] equipment, List<BaseItem> inventory, List<BaseItem> stash)
        {
            SendPlayerEquipmentEvent?.Invoke(this, equipment);
            SendPlayerInventoryEvent?.Invoke(this, inventory);
            SendStashInventoryEvent?.Invoke(this, stash);
        }
    }
}