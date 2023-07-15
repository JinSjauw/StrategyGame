using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Weapon")]
public class Weapon : ScriptableObject
{
    [SerializeField] private Sprite weaponSprite;
    //list or number of attachment slots; Should be an array? with maxAttachments as size initializer
    private List<string> _attachments;
    [SerializeField] private float rangeRadius;
    [SerializeField] private float ammoCapacity;
    
    [SerializeField] private ShootConfig _shootConfig;
    [SerializeField] private VFXConfig _vfxConfig;
    [SerializeField] private SFXConfig _sfxConfig;
    
    public void Shoot(Vector2 target)
    {
        //_shootConfig.Shoot();
        //Spawn projectile
        //Point gun towards target
    }

    public Sprite GetSprite()
    {
        return weaponSprite;
    }
}
