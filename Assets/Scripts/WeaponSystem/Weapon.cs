using UnityEngine;

[CreateAssetMenu(menuName = "Items/Weapon")]
public abstract class Weapon : ScriptableObject
{
    [SerializeField] private Sprite weaponSprite;
    //list or number of attachment slots;
    [SerializeField] private float accuracy;
    [SerializeField] private float rangeRadius;
    [SerializeField] private float recoil;
    [SerializeField] private float damage;
    [SerializeField] private float firerate;
    [SerializeField] private float ammoCapacity;

    protected abstract void Shoot();
    public abstract bool Execute();
}
