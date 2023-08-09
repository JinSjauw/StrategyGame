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
        
        public void OnPlayerSpawnRequest(List<BaseItem> inventory, Weapon weapon, BaseItem[] equipment)
        {
            SendPlayerDataEvent?.Invoke(this, weapon);
        }

        public void OnPlayerInventoryRequest(List<BaseItem> inventory, Weapon weapon, BaseItem[] equipment)
        {
            SendPlayerInventoryEvent?.Invoke(this, inventory);
            SendPlayerEquipmentEvent?.Invoke(this, equipment);
        }
    }
}