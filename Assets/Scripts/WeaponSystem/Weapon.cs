using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "Items/Weapon")]
public class Weapon : ScriptableObject
{
    [SerializeField] private Sprite weaponSprite;
    //list or number of attachment slots; Should be an array? with maxAttachments as size initializer
    private List<string> _attachments;
    [SerializeField] private float barrelLength;
    [SerializeField] private float ammoCapacity;
    
    //Test
    [SerializeField] private Transform projectile;
    
    [SerializeField] private ShootConfig _shootConfig;
    [SerializeField] private VFXConfig _vfxConfig;
    [SerializeField] private SFXConfig _sfxConfig;

    [Header("ReloadSystem Test")]
    [SerializeField] private BulletConfig configA;
    [SerializeField] private BulletConfig configB;
    [SerializeField] private BulletConfig configC;
    
    [SerializeField] private List<BulletConfig> _bulletsToLoad;

    private Transform _weaponTransform;
    private Vector3 _muzzlePosition;
    private Stack<BulletConfig> _loadedBullets;
    
    private Action _onShootAction;
    private bool _isReloading;

    public float Accuracy { get => _shootConfig.accuracy; }
    public float Recoil { get => _shootConfig.recoil; }
    public int AmmoCount { get => _loadedBullets.Count; }
    public int BulletAmount { get => _bulletsToLoad.Count; } 
    public float ReloadTimer { get; set; }
    
    private void Awake()
    {
        _loadedBullets = new Stack<BulletConfig>();
        SimulateBulletStack();
    }

    //PLACEHOLDER METHOD
    private void SimulateBulletStack()
    {
        _bulletsToLoad.Clear();
        for (int i = 0; i < 10; i++)
        {
            _bulletsToLoad.Add(configA.Copy());
            _bulletsToLoad.Add(configB.Copy());
            _bulletsToLoad.Add(configC.Copy());
        }
    }

    public Weapon Equip(SpriteRenderer weaponRenderer, Action OnShoot)
    {
        //Init the gun variables
        Weapon weaponCopy = Instantiate(this);
        weaponCopy._weaponTransform = weaponRenderer.transform;
        
        ShootConfig shootConfigCopy = Instantiate(_shootConfig);
        weaponCopy._shootConfig = shootConfigCopy;

        weaponRenderer.sprite = weaponSprite;
        weaponCopy._onShootAction = OnShoot;
        
        return weaponCopy;
    }

    public void Aim()
    {
        //WeaponSpecific Minigame?
    }
    
    public void Shoot(bool ignore = false)
    {
        _muzzlePosition = _weaponTransform.position + _weaponTransform.right * barrelLength;
        
        //Replace with object pool

        //Fire rate?

        if (_loadedBullets.Count > 0)
        {
            Bullet bullet = Instantiate(projectile, _muzzlePosition, Quaternion.identity).GetComponent<Bullet>();
            bullet.SetBullet(_loadedBullets.Pop());
            
            float accuracySpread = _shootConfig.accuracy / 2;
            Quaternion spreadAngle = Quaternion.AngleAxis(Random.Range(-accuracySpread, accuracySpread), Vector3.forward);
            bullet.Fire(spreadAngle * _weaponTransform.right, ignore);
            
            _onShootAction();   
        }
        else
        {
            Debug.Log("Gun is Empty!");
        }
    }

    public void Eject()
    {
        _loadedBullets.Clear(); //Empty magazine. Return/Destroy all objects
        Debug.Log(_loadedBullets.Count);
    }
    
    public void Load()
    {
        if (_bulletsToLoad.Count <= 0)
        {
            Debug.Log("NO AMMO TO LOAD");
            return;
        }
        
        for (int i = 0; i < ammoCapacity; i++)
        {
            if (_bulletsToLoad.Count <= 0)
            {
                return;
            }

            BulletConfig bulletConfig = _bulletsToLoad.First();
            _bulletsToLoad.Remove(bulletConfig);
            _loadedBullets.Push(bulletConfig);
        }
        
        Debug.Log(_loadedBullets.Count);
    }
    //Coroutine and call an action when it is done loading
    //Load bullets
    
    public IEnumerator ReloadRoutine()
    {
        while (ReloadTimer <= 1f)
        {
            ReloadTimer += Time.deltaTime / _shootConfig.reloadTime;
            yield return null;
        }
    }

    public Sprite GetSprite()
    {
        return weaponSprite;
    }
}
