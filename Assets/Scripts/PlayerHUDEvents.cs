using System;
using UnityEngine;
using UnityEngine.Events;

namespace Player
{
    [CreateAssetMenu(menuName = "Events/PlayerHUDEvents")]
    public class PlayerHUDEvents : ScriptableObject
    {
        public event EventHandler<int> OnHealthChanged;
        public event EventHandler<int> OnPlayerShoot;
        public event EventHandler<int> OnPlayerReload;
        public event EventHandler<Sprite> OnPocketItemChanged;
        public event EventHandler<Sprite> OnWeaponSwitched;

        public void RaiseHealthChanged(int health)
        {
            OnHealthChanged?.Invoke(this, health);
        }

        public void RaisePlayerShoot(int amount)
        {
            OnPlayerShoot?.Invoke(this, amount);
        }

        public void RaisePlayerReload(int amount)
        {
            OnPlayerReload?.Invoke(this, amount);
        }

        public void RaisePocketItemChanged(Sprite sprite)
        {
            OnPocketItemChanged?.Invoke(this, sprite);
        }

        public void RaiseWeaponSwitched(Sprite sprite)
        {
            OnWeaponSwitched?.Invoke(this, sprite);
        }
    }
}