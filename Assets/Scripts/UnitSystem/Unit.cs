using System;
using System.Collections.Generic;
using ActionSystem;
using UnityEngine;

public class Unit : MonoBehaviour
{
    //Unit Stats --> Pass onto the Action Instance
    [SerializeField] private UnitData _unitData;
    //Unit Loadout --> Pass onto the Action Instance
    //WeaponData field
    
    //Temp --> Action Collection
    [SerializeField] private MoveAction _moveAction;

    //Unit Events

    private event EventHandler<UnitMovedEventArgs> _onUnitMove;
    public EventHandler<UnitMovedEventArgs> OnUnitMove
    {
        get => _onUnitMove;
        set => _onUnitMove = value;
    }
    private ActionState _actionState;

    public ActionState ActionState
    {
        get { return _actionState; }
    }
    private bool _isExecuting;
    public bool isExecuting
    {
        get { return _isExecuting; }
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

    private void Start()
    {
        _onUnitMove?.Invoke(this, new UnitMovedEventArgs(this, transform.position, transform.position));
    }

    private void Update()
    {
        if (_isExecuting)
        {
            if (_actionState != ActionState.Completed)
            {
                //Temp --> selectedAction.Execute();
                _actionState = _moveAction.Execute();
            }
            else
            {
                _isExecuting = false;
            }
        }
    }

    public void Move(List<Vector2> path)
    {
        _isExecuting = true;
        _actionState = ActionState.Started;
        _moveAction.SetPath(path);
    }

    public UnitData GetUnitStats()
    {
        return _unitData;
    }
}
