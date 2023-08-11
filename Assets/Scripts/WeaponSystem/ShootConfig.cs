using System.Collections;
using System.Collections.Generic;
using Items;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Weapons/ShootConfig")]
public class ShootConfig : ScriptableObject
{
    public float accuracy;
    public float recoil;
    public float damage;
    public float firerate;
    public float reloadTime;
    public ItemID ammoType;
}
