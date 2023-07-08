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
    //Create the action buttons over here
    //Get Actions list from Unit Script

    public void CreateButtons(List<BaseAction> actionsList)
    {
        foreach (BaseAction action in actionsList)
        {
            
        }
    }
}
