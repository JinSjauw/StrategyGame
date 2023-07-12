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
    
    public void CreateButtons(Dictionary<Type, BaseAction> actionsList, Action<BaseAction> onButtonClick)
    {
        _actionButtons = new List<ActionButton>();
        foreach (var action in actionsList)
        {
            Transform buttonTransform = Instantiate(actionButtonPrefab, actionButtonLeft);
            if (buttonTransform.TryGetComponent(out ActionButton button))
            {
                button.Initialize(action.Value, onButtonClick);
                _actionButtons.Add(button);
            }
        }
    }
}
