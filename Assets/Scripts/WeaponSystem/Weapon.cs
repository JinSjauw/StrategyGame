using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;


namespace Items
{
    [CreateAssetMenu(menuName = "Items/Weapon")]
    public class Weapon : BaseItem
    {
        [SerializeField] private Sprite weaponSprite;
        //list or number of attachment slots; Should be an array? with maxAttachments as size initializer
        private List<string> _attachments;
        [SerializeField] private float barrelLength;
        [SerializeField] private int ammoCapacity;
        
        //Test
        [SerializeField] private Transform projectile;
        
        [SerializeField] private ShootConfig _shootConfig;
        [SerializeField] private VFXConfig _vfxConfig;
        [SerializeField] private SFXConfig _sfxConfig;

        [Header("ReloadSystem Test")]
        [SerializeField] private Bullet a;
        [SerializeField] private Bullet b;
        [SerializeField] private Bullet c;
        
        [SerializeField] private List<Bullet> _bulletsToLoad;

        [SerializeField] private Transform _weaponTransform;
        private Vector3 _muzzlePosition;
        private Stack<Bullet> _loadedBullets;
        
        private Action _onShootAction;
        private bool _isReloading;

        public float Accuracy { get => _shootConfig.accuracy; }
        public float Recoil { get => _shootConfig.recoil; }
        public int AmmoCount { get => _loadedBullets.Count; }
        public int BulletAmount { get => _bulletsToLoad.Count; } 
        public int AmmoCapacity { get => ammoCapacity; }
        public float ReloadTimer { get; set; }
        
        private void Awake()
        {
            _loadedBullets = new Stack<Bullet>();
            SimulateBulletStack();
        }

        //PLACEHOLDER METHOD
        private void SimulateBulletStack()
        {
            _bulletsToLoad.Clear();
            for (int i = 0; i < 10; i++)
            {
                _bulletsToLoad.Add(a.Copy());
                _bulletsToLoad.Add(b.Copy());
                _bulletsToLoad.Add(c.Copy());
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

                Bullet bullet = _bulletsToLoad.First();
                _bulletsToLoad.Remove(bullet);
                _loadedBullets.Push(bullet);
            }
            
            Debug.Log(_loadedBullets.Count);
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
    }
}

