using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Weapon")]
public class Weapon : ScriptableObject
{
    private Sprite weaponSprite;

    //list or number of attachment slots;

    private float accuracy;
    private float rangeRadius;
    private float recoil;
    private float damage;
    private float firerate;
    private float maxAmmo;
}
