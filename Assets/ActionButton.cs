using System.Collections;
using System.Collections.Generic;
using ActionSystem;
using UnityEngine;

public class ActionButton : MonoBehaviour
{
    private BaseAction _buttonAction;
    private Sprite _actionSprite;

    public void Initialize(BaseAction buttonAction, Sprite actionSprite)
    {
        _buttonAction = buttonAction;
        _actionSprite = actionSprite;
    }
    
    //Invoke ActionSelected Event
    public void OnClick()
    {
        Debug.Log("Selected Action");
    }
}
