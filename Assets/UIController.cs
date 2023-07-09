using System;
using System.Collections;
using System.Collections.Generic;
using ActionSystem;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private Transform actionButtonLeft;
    [SerializeField] private Transform actionButtonDown;
    [SerializeField] private Transform actionButtonPrefab;

    private List<ActionButton> _actionButtons;
    
    public void CreateButtons(List<BaseAction> actionsList, Action<BaseAction> onButtonClick)
    {
        _actionButtons = new List<ActionButton>();
        foreach (BaseAction action in actionsList)
        {
            Transform buttonTransform = Instantiate(actionButtonPrefab, actionButtonLeft);
            if (buttonTransform.TryGetComponent(out ActionButton button))
            {
                button.Initialize(action, onButtonClick);
                _actionButtons.Add(button);
            }
        }
    }

    /*public void SubscribeButtons(Action<BaseAction> onButtonClick)
    {
        foreach (ActionButton button in _actionButtons)
        {
            
        }
    }*/
}
