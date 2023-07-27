using System;
using System.Collections.Generic;
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

    private Action OnShootAction;

    private void Awake()
    {
        _loadedBullets = new Stack<BulletConfig>();
        SimulateBulletStack();
    }

    //PLACEHOLDER METHOD
    private void SimulateBulletStack()
    {
        for (int i = 0; i < 10; i++)
        {
            _bulletsToLoad.Add(configA.Copy());
            _bulletsToLoad.Add(configB.Copy());
            _bulletsToLoad.Add(configC.Copy());
        }
    }

    public Weapon Equip(Transform weaponTransform, Action OnShoot)
    {
        //Init the gun variables
        Weapon weaponCopy = Instantiate(this);
        weaponCopy._weaponTransform = weaponTransform;
        
        ShootConfig shootConfigCopy = Instantiate(_shootConfig);
        weaponCopy._shootConfig = shootConfigCopy;

        weaponCopy.OnShootAction = OnShoot;
        
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
        //Minigame?

        if (_loadedBullets.Count > 0)
        {
            Bullet bullet = Instantiate(projectile, _muzzlePosition, Quaternion.identity).GetComponent<Bullet>();
            bullet.SetBullet(_loadedBullets.Pop());
            float accuracySpread = _shootConfig.accuracy / 2;
            Quaternion rotation = Quaternion.AngleAxis(Random.Range(-accuracySpread, accuracySpread), Vector3.forward);
            bullet.Fire(rotation * _weaponTransform.right, ignore);
            OnShootAction();   
        }
        else
        {
            Debug.Log("Gun is Empty!");
        }
    }
    
    //Load bullets
    public void Load()
    {
        //LoadConfig SO? Lets have a two part reload
        //Should Call an IEnumerator - Load timer + Active reload minigame
        //Placeholder
        if (_loadedBullets.Count > 0)
        {
            _loadedBullets.Clear(); //Empty magazine. Return/Destroy all objects
            Debug.Log(_loadedBullets.Count);
            return;
        }

        if (_bulletsToLoad.Count <= 0)
        {
            Debug.Log("NO AMMO TO LOAD");
            return;
        }

        for (int i = 0; i < ammoCapacity; i++)
        {
            BulletConfig bulletConfig = _bulletsToLoad[i];
            _bulletsToLoad.RemoveAt(i);
            _loadedBullets.Push(bulletConfig);
        }
        Debug.Log(_loadedBullets.Count);
    }

    public Sprite GetSprite()
    {
        return weaponSprite;
    }
}
