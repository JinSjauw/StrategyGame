using System;
using System.Collections.Generic;
using ActionSystem;
using UnityEngine;

public class Unit : MonoBehaviour
{
    //Temp
    [SerializeField] private MoveAction _moveAction;
    private bool _isActive;
    private ActionState _actionState;
    public bool IsActive
    {
        get { return _isActive; }
    }

    private void Awake()
    {
        _moveAction = ScriptableObject.CreateInstance<MoveAction>();
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
}
