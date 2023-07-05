using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMovedEventArgs : EventArgs
{
    public Unit unit { get; set; }
    public Vector2 originPosition { get; set; }
    public Vector2 targetPosition { get; set; }

    public UnitMovedEventArgs(Unit senderUnit, Vector2 origin, Vector2 target)
    {
        unit = senderUnit;
        originPosition = origin;
        targetPosition = target;
    }
}

[Serializable]
public class UnitData 
{
    public float health;
    public float moveSpeed;
}
