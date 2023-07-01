using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Unit : MonoBehaviour
{
    //Actions

    private ActionState _actionState;
    private IAction _selectedAction;
    private Action _onActionComplete;

    public void TakeAction(IAction action, Action onActionComplete)
    {
        _actionState = ActionState.Started;
        _selectedAction = action;
        _onActionComplete = onActionComplete;
    }
    
    // Update is called once per frame
    private void Update()
    {
        if (_actionState != ActionState.Completed)
        {
            _actionState = _selectedAction.Execute();
        }else if (_actionState == ActionState.Completed)
        {
            _onActionComplete();
        }
    }
}
