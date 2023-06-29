using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private List<Transform> playerTransforms;
    [SerializeField] private LevelGrid _levelGrid;
    
    public void OnMouseClick(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.performed)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            GridPosition tileGridPosition = _levelGrid.GetGridPosition(mousePosition);
            Debug.Log(tileGridPosition);
        }
    }
    
}
