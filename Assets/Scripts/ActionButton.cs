using System;
using System.Collections;
using System.Collections.Generic;
using ActionSystem;
using UnityEngine;

public class ActionButton : MonoBehaviour
{
    private BaseAction _buttonAction;
    private Sprite _actionSprite;
    private Action<BaseAction> _onClickAction;

    public void Initialize(BaseAction buttonAction, Action<BaseAction> onClickAction)
    {
        _buttonAction = buttonAction;
        _onClickAction = onClickAction;
        //_actionSprite = actionSprite;
    }
    
    //Invoke ActionSelected Event
    public void OnClick()
    {
        _onClickAction(_buttonAction);
    }
}
