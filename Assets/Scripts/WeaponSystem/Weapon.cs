using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using InventorySystem;
using SoundManagement;
using UnityEngine;
using Random = UnityEngine.Random;


namespace Items
{
    [CreateAssetMenu(menuName = "Items/Weapon")]
    public class Weapon : BaseItem
    {
        [Header("Weapon Variables")]
        [SerializeField] private Sprite weaponSprite;
        [SerializeField] private Transform _weaponTransform;
        //list or number of attachment slots; Should be an array? with maxAttachments as size initializer
        private List<string> _attachments;
        [SerializeField] private float barrelLength;
        [SerializeField] private int ammoCapacity;
        
        //Test -- Projectile Prefab, maybe not a test. 
        [Header("Projectile Prefab --> Loads BulletConfig")]
        [SerializeField] private Transform projectile;
        
        [Header("Configuration")]
        [SerializeField] private ShootConfig _shootConfig;
        [SerializeField] private VFXConfig _vfxConfig;
        [SerializeField] private SFXConfig _sfxConfig;
        [SerializeField] private SFXEventChannel _sfxChannel;
        
        [Header("ReloadSystem Test")]
        //[SerializeField] private Bullet a;
        [SerializeField] private Bullet bulletType;
        //[SerializeField] private Bullet c;
        [SerializeField] private List<Bullet> _bulletsToLoad;

        private Vector3 _muzzlePosition;
        private Stack<Bullet> _loadedBullets;
        
        private Action _onShootAction;
        private bool _isReloading;
        private bool _isLoaded;
        private bool _infinite;
        
        public float Accuracy { get => _shootConfig.accuracy; }
        public float Recoil { get => _shootConfig.recoil; }
        public int AmmoCount { get => _loadedBullets.Count; }
        public int BulletAmount { get => _bulletsToLoad.Count; } 
        public int AmmoCapacity { get => ammoCapacity; }
        public float ReloadTimer { get; set; }
        public bool isLoaded { get => _isLoaded; }
        
        private void Awake()
        {
            _loadedBullets = new Stack<Bullet>();
            _bulletsToLoad = new List<Bullet>();
        }
        
        //PLACEHOLDER METHOD
        private void SimulateBulletStack()
        {
            _bulletsToLoad.Clear();
            _bulletsToLoad.Add(bulletType.Copy());

            /*for (int i = 0; i < 1; i++)
            {
                //_bulletsToLoad.Add(a.Copy());
                _bulletsToLoad.Add(bulletType.Copy());
                //_bulletsToLoad.Add(c.Copy());
            }*/
        }

        public Weapon Equip(SpriteRenderer weaponRenderer, Action OnShoot, bool infinite = false)
        {
            _weaponTransform = weaponRenderer.transform;
            weaponRenderer.sprite = weaponSprite;
            _onShootAction = OnShoot;
            _infinite = infinite;
            _isLoaded = true;
            
            if (_infinite)
            {
                SimulateBulletStack();
            }
            
            return this;
        }

        public SFXConfig GetSFXConfig()
        {
            return _sfxConfig;
        }
        
        public void Aim()
        {
            //WeaponSpecific Minigame?
        }
        
        public void Shoot(bool ignore = false)
        {
            _muzzlePosition = _weaponTransform.position + _weaponTransform.right * barrelLength;
            
            //Ignore adjacent cover colliders
            RaycastHit2D coverHit = Physics2D.Raycast(_muzzlePosition, _weaponTransform.right, 1.5f,LayerMask.GetMask("Cover"));

            //Fire rate?

            if (_loadedBullets.Count > 0)
            {
                BulletProjectile bulletProjectile = Instantiate(projectile, _muzzlePosition, Quaternion.identity).GetComponent<BulletProjectile>();
                bulletProjectile.SetBullet(_loadedBullets.Pop());

                if (coverHit.collider)
                {
                    bulletProjectile.IgnoreCollider(coverHit.collider);
                }
                
                float accuracySpread = _shootConfig.accuracy / 2;
                Quaternion spreadAngle = Quaternion.AngleAxis(Random.Range(-accuracySpread, accuracySpread), Vector3.forward);
                bulletProjectile.Fire(spreadAngle * _weaponTransform.right, ignore);
                
                //_sfxChannel.RequestSFX(_sfxConfig.GetShootClip(), _muzzlePosition);
                
                _onShootAction();   
            }
            else
            {
                Debug.Log("Gun is Empty!");
            }
        }

        public void Eject()
        {
            //Play sound here aswell
            //_sfxChannel.RequestSFX(_sfxConfig.GetEjectClip(), _muzzlePosition);
            _isLoaded = false;
            _loadedBullets.Clear(); //Empty magazine. Return/Destroy all objects
            Debug.Log(_loadedBullets.Count);
        }
        
        public void Load()
        {
            //Play sound here aswell
            if (_bulletsToLoad.Count <= 0)
            {
                Debug.Log("NO AMMO TO LOAD");
                return;
            }
            
            _isLoaded = true;
            for (int i = 0; i < ammoCapacity; i++)
            {
                if (_bulletsToLoad.Count <= 0)
                {
                    return;
                }

                Bullet bullet = _bulletsToLoad.First();
                if (!_infinite)
                {
                    _bulletsToLoad.Remove(bullet);
                }
                
                _loadedBullets.Push(bullet);
            }
            
            //Debug.Log(_loadedBullets.Count);
            //_sfxChannel.RequestSFX(_sfxConfig.GetLoadClip(), _muzzlePosition);
        }
        //Coroutine and call an action when it is done loading
        //Load bullet
        public IEnumerator ReloadRoutine()
        {
            while (ReloadTimer <= 1f)
            {
                ReloadTimer += Time.deltaTime / _shootConfig.reloadTime;
                yield return null;
            }
        }
        public override Sprite GetSprite()
        {
            return weaponSprite;
        }
        public override ItemType GetItemType()
        {
            return ItemType.Weapon;
        }

        public void GiveBullets(List<Bullet> bullets)
        {
            Debug.Log($"Bullets Give {bullets.Count}");
            _bulletsToLoad = bullets;
        }
    }
}

