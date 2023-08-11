using System;
using System.Collections;
using System.Collections.Generic;
using ActionSystem;
using Items;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "Actions/ShootAction")]
public class ShootAction : BaseAction
{
    //Have weapons run their own shoot logic and return when they are done?
    private Weapon _weapon;
    private Vector2 _target;

    private void Shoot()
    {
        //SPAWN BULLET;
        //SFX & VFX
        
        //Aim the weapon at the target;
        
        RaycastHit2D hit = Physics2D.Raycast(_target, Vector3.forward);
        bool onTarget = false;
        if (hit.collider)
        {
            if (hit.collider.CompareTag("UnitHead"))
            {
                onTarget = true;
            }
        }

        _weapon.Shoot(onTarget);
        _onComplete();
    }

    public override void SetAction(Vector2 target)
    {
        //Retrieve weapon of unit;
        _weapon = holderUnit.weapon;
        _target = target;
    }

    public override void Preview()
    {
    }

    public override void StopPreview()
    {
        
    }

    public override void Execute()
    {
        Shoot();
    }
}
