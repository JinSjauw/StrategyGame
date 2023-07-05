using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private List<Unit> _playerUnits;
    [SerializeField] private LevelGrid _levelGrid;
    private Pathfinding _pathfinding;
    private List<Vector2> _path;
    private Unit _currentUnit;
    
    private void Start()
    {
        _pathfinding = new Pathfinding(_levelGrid);
        _currentUnit = _playerUnits[0];
    }

    public void OnMouseClick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            GridPosition clickGridPosition = _levelGrid.GetGridPosition(mousePosition);

            if (_levelGrid.IsOnGrid(clickGridPosition))
            {
                Debug.Log(_currentUnit.transform.position);
                _path = _pathfinding.FindPath(_levelGrid.GetGridPosition(_currentUnit.transform.position), clickGridPosition);
                if (!_currentUnit.IsActive)
                {
                    _currentUnit.Move(_path);
                }
            }
        }
    }

    public Unit GetCurrentUnit()
    {
        return _currentUnit;
    }

    /*public void OnMouseMove(InputAction.CallbackContext context)
    {
        
    }*/
}
