using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private List<Transform> _playerTransforms;
    [SerializeField] private LevelGrid _levelGrid;

    private Pathfinding _pathfinding;
    private List<Vector2> _path;
    private Transform _currentUnit;
    
    private void Start()
    {
        _pathfinding = new Pathfinding(_levelGrid);
    }
    
    private void Update()
    {
        //Moving
        
    }

    public void OnMouseClick(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.performed)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            GridPosition clickGridPosition = _levelGrid.GetGridPosition(mousePosition);

            if (_levelGrid.IsOnGrid(clickGridPosition))
            {
                Debug.Log(clickGridPosition);
                Transform unit = _playerTransforms[0];
                _path = _pathfinding.FindPath(_levelGrid.GetGridPosition(unit.position), clickGridPosition);
            }
        }
    }

    public void OnMouseMove(InputAction.CallbackContext callbackContext)
    {
        
    }
}
