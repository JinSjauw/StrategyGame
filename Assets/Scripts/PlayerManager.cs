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
        
        //On Awaking Subscribe levelGrid to all unitMoved events;
        foreach (Unit unit in _playerUnits)
        {
            unit.OnUnitMove += _levelGrid.Unit_OnUnitMoved;
        }
    }

    public void OnMouseClick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            RaycastHit2D hit =
                Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()));

            if (hit.collider)
            {
                Debug.Log(hit.collider.name);

                if (hit.collider.TryGetComponent<Unit>(out Unit selectedUnit))
                {
                    _currentUnit = selectedUnit;
                }
                
                return;
            }

            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            GridPosition clickGridPosition = _levelGrid.GetGridPosition(mousePosition);
            
            if (_levelGrid.IsOnGrid(clickGridPosition) && !_levelGrid.GetTileGridObject(clickGridPosition).isOccupied)
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
}
