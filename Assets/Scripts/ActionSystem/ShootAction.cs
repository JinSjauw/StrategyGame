using System;
using System.Collections;
using System.Collections.Generic;
using ActionSystem;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "Actions/ShootAction")]
public class ShootAction : BaseAction
{
    //Have weapons run their own shoot logic and return when they are done?
    private Weapon _weapon;
    private Vector2 _target;
    private int _shootCounter = 0;

    private void Shoot()
    {
        //SPAWN BULLET;
        //SFX & VFX
        
        //Aim the weapon at the target;

        RaycastHit2D hit = Physics2D.Raycast(_target, Vector3.forward);
        bool onTarget = false;
        if (hit.collider)
        {
            /*Debug.Log("Shot!" + _shootCounter);
            Debug.Log(hit.collider.name);
            Debug.Log(hit.collider.tag);*/

            if (hit.collider.CompareTag("UnitHead"))
            {
                onTarget = true;
            }
        }

        _weapon.Shoot(onTarget);
        _shootCounter++;
        _onComplete();
    }
    
    public override void Initialize(Unit unit, Action onComplete)
    {
        base.Initialize(unit, onComplete);
        /*inputReader.ShootStart += OnShoot;
        inputReader.AimMove += OnAimMove;*/
    }

    public override void UnsetAction()
    {
        //inputReader.EnableGameplay();
    }

    public override List<Vector2> SetAction(Vector2 target)
    {
        //Retrieve weapon of unit;
        _weapon = holderUnit.weapon;
        _target = target;
        //If mouse "target" is over player head collider you can shoot through all half cover colliders.
        
        //Have the weapon angle set here.
        
        //Return list of Units in Range
        //Return a overlapCircle 
        
        return new List<Vector2>();
    }

    public override void Execute()
    {
        //Start listening to the input only when this gets called for the 1st time

        /*if (_shootCounter > 3)
        {
            inputReader.EnableGameplay();
            _onComplete();
        }*/
        
        Shoot();

        //Have actions cost points. When certain point threshold is met the turn advances.
        
        //If shot x many times or timer has ran out invoke _onComplete
        //Run the weapon Logic until it finishes
    }
}
