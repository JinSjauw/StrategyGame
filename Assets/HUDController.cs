using System;
using Player;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    //Has event channels
    
    //References to the hud elements
    [SerializeField] private PlayerHUDEvents _playerHUD;

    [SerializeField] private Image weaponImage;
    [SerializeField] private Image pocketItemImage;
    [SerializeField] private Image healthBar;
    [SerializeField] private Transform ammoCounter;
    [SerializeField] private Transform ammoImage;
    
    private void Awake()
    {
        _playerHUD.OnHealthChanged += UpdateHealthBar;
        _playerHUD.OnPlayerShoot += UpdateAmmoCounter;
        _playerHUD.OnPlayerReload += UpdateAmmoCounter;
        _playerHUD.OnPocketItemChanged += UpdatePocketItemIcon;
        _playerHUD.OnWeaponSwitched += UpdateWeaponIcon;
    }

    private void OnDestroy()
    {
        _playerHUD.OnHealthChanged -= UpdateHealthBar;
        _playerHUD.OnPlayerShoot -= UpdateAmmoCounter;
        _playerHUD.OnPlayerReload -= UpdateAmmoCounter;
        _playerHUD.OnPocketItemChanged -= UpdatePocketItemIcon;
        _playerHUD.OnWeaponSwitched -= UpdateWeaponIcon;
    }

    private void UpdateWeaponIcon(object sender, Sprite e)
    {
        weaponImage.sprite = e;
    }

    private void UpdatePocketItemIcon(object sender, Sprite e)
    {
        pocketItemImage.sprite = e;
    }

    private void UpdateAmmoCounter(object sender, int e)
    {
        if (e < 0)
        {
            if (ammoCounter.childCount <= 0) return;
            Transform ammoToRemove = ammoCounter.transform.GetChild(0);
            Debug.Log($"children: {ammoCounter.childCount} {ammoToRemove}");
            if (ammoToRemove == null) return;
            Destroy(ammoToRemove.gameObject);
        }
        else if(e > 0)
        {
            for (int i = 0; i < e; i++)
            {
                Instantiate(ammoImage, ammoCounter).name += i;
            }
        }else if (e == 0)
        {
            foreach (Transform child in ammoCounter)
            {
                Destroy(child.gameObject);
            }
        }
    }

    private void UpdateHealthBar(object sender, int e)
    {
        Debug.Log(e / 100f);
        healthBar.fillAmount += e / 100f;
    }
}
