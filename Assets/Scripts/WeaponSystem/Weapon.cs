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
        
        //Get random angle
        //Simulate recoil spread?
        
        //Spawn bullet at muzzlePosition;
        Bullet bullet = Instantiate(projectile, _muzzlePosition, Quaternion.identity).GetComponent<Bullet>();
        float accuracySpread = _shootConfig.accuracy / 2;
        Quaternion rotation = Quaternion.AngleAxis(Random.Range(-accuracySpread, accuracySpread), Vector3.forward);
        /*Quaternion rotationA = Quaternion.AngleAxis(accuracySpread, Vector3.forward);
        Quaternion rotationB = Quaternion.AngleAxis(-accuracySpread, Vector3.forward);
        Debug.DrawRay(_weaponTransform.position, rotationA * _weaponTransform.right * 10, Color.yellow);
        Debug.DrawRay(_weaponTransform.position, rotationB * _weaponTransform.right * 10, Color.cyan);*/
        
        bullet.Fire(rotation * _weaponTransform.right, ignore);

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
