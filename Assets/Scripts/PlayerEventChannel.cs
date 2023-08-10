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
        public event EventHandler<List<BaseItem>> SendPlayerPocketsEvent; 

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

        public void OnPlayerInventoryRequest(List<BaseItem> inventory, BaseItem[] equipment, List<BaseItem> pockets)
        {
            SendPlayerInventoryEvent?.Invoke(this, inventory);
            SendPlayerEquipmentEvent?.Invoke(this, equipment);
            SendPlayerPocketsEvent?.Invoke(this, pockets);
        }

        public void EnterMainMenu()
        {
            OnMainMenuRequest.Invoke();
        }
        
        public void OnMenuInventoryRequest(BaseItem[] equipment, List<BaseItem> inventory, List<BaseItem> stash, List<BaseItem> pockets)
        {
            SendPlayerEquipmentEvent?.Invoke(this, equipment);
            SendPlayerInventoryEvent?.Invoke(this, inventory);
            SendStashInventoryEvent?.Invoke(this, stash);
            SendPlayerPocketsEvent?.Invoke(this, pockets);
        }
    }
}