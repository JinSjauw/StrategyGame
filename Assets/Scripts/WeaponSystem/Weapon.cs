using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.PlayerLoop;

[CreateAssetMenu(menuName = "Items/Weapon")]
public class Weapon : ScriptableObject
{
    [SerializeField] private Sprite weaponSprite;
    //list or number of attachment slots; Should be an array? with maxAttachments as size initializer
    private List<string> _attachments;
    [SerializeField] private float barrelLength;
    [SerializeField] private float rangeRadius;
    [SerializeField] private float ammoCapacity;
    
    //Test
    [SerializeField] private Transform projectile;
    
    [SerializeField] private ShootConfig _shootConfig;
    [SerializeField] private VFXConfig _vfxConfig;
    [SerializeField] private SFXConfig _sfxConfig;

    private Transform _weaponTransform;
    private Vector3 _muzzlePosition;
    private Stack<Bullet> _loadedBullets;

    public Weapon Equip(Transform weaponTransform)
    {
        //Init the gun variables
        Weapon weaponCopy = Instantiate(this);
        weaponCopy._weaponTransform = weaponTransform;

        return weaponCopy;
    }
    
    public void Shoot(bool ignore = false)
    {
        //Spawn bullet at 
        //_shootConfig.Shoot();
        //Spawn projectile
        //Point gun towards target
        //Find out the muzzle position of weapon
        _muzzlePosition = _weaponTransform.position + _weaponTransform.right * barrelLength;
        
        //Spawn bullet at muzzlePosition;
        Bullet bullet = Instantiate(projectile, _muzzlePosition, Quaternion.identity).GetComponent<Bullet>();
        bullet.Fire(_weaponTransform.right, ignore);

        //Debug.DrawLine(_muzzlePosition, _weaponTransform.right * 100, Color.red);
    }

    //Load bullets
    public void Load()
    {
        //LoadConfig SO?
    }

    public Sprite GetSprite()
    {
        return weaponSprite;
    }
}
