using System;
using System.Collections.Generic;
using ActionSystem;
using UnityEngine;

[Serializable]
public class UnitStats
{
    public float health;
    public float moveSpeed;
}

public class Unit : MonoBehaviour
{
    //Unit Stats --> Pass onto the Action Instance
    [SerializeField] private UnitStats _unitStats;
    //Unit Loadout --> Pass onto the Action Instance
    //WeaponData field
    
    //Temp --> Action Collection
    [SerializeField] private MoveAction _moveAction;

    private bool _isActive;
    private ActionState _actionState;
    public bool IsActive
    {
        get { return _isActive; }
    }

    private SpriteRenderer _sprite;

    public SpriteRenderer Sprite
    {
        get { return _sprite;  }
    }

    private void Awake()
    {
        _sprite = GetComponentInChildren<SpriteRenderer>();
        
        _moveAction = Instantiate(_moveAction);
        _moveAction.Initialize(this);
    }

    private void Update()
    {
        if (_isActive)
        {
            if (_actionState != ActionState.Completed)
            {
                _actionState = _moveAction.Execute();
            }
            else
            {
                _isActive = false;
            }
        }
    }

    public void Move(List<Vector2> path)
    {
        Debug.Log("Moving!");
        _isActive = true;
        _actionState = ActionState.Started;
        _moveAction.SetPath(path);
    }

    public UnitStats GetUnitStats()
    {
        return _unitStats;
    }
}
