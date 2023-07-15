using System;
using System.Collections;
using System.Collections.Generic;
using ActionSystem;
using UnityEngine;

public class ShootAction : BaseAction
{

    //Have weapons run their own shoot logic and return when they are done?
    private Weapon _weapon;
    
    public override List<Vector2> SetAction(Vector2 target, Action onComplete)
    {
        //Retrieve weapon of unit;
        _weapon = holderUnit.weapon;
        
        
        //Return list of Units in Range
        
        
        return new List<Vector2>();
    }

    public override void Execute()
    {
        //If shot x many times or timer has ran out invoke _onActionComplete
    }
}
