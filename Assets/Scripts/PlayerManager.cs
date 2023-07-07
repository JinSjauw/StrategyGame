using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private List<Unit> _playerUnits;
    [SerializeField] private LevelGrid _levelGrid;

    //Selection Box
    [SerializeField] private SelectionBox _selectionBox;
    private Vector2 _startPoint;
    private Vector2 _endPoint;
    private bool _isDragging;

    private Pathfinding _pathfinding;
    private List<Vector2> _path;
    
    private Unit _currentUnit;
    private List<Unit> _selectedUnits;
    private Vector2 _targetPosition;
    private bool _unitsFollowing;
    Unit lastUnit;
    
    private void Awake()
    {
        //On Awaking Subscribe levelGrid to all unitMoved events;
        foreach (Unit unit in _playerUnits)
        {
            unit.OnUnitMove += _levelGrid.Unit_OnUnitMoved;
        }
    }

    private void Start()
    {
        _pathfinding = new Pathfinding(_levelGrid);
        _currentUnit = _playerUnits[0];
        _selectedUnits = new List<Unit>();
    }

    private void Update()
    {
        if (_isDragging)
        {
            //DrawBoxSelection();
            _endPoint = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            _selectionBox.DrawSelectionBox(_startPoint, _endPoint);
        }

        if (_unitsFollowing)
        {
            for(int i = 1; i < _selectedUnits.Count; i++)
            {
                Unit unit = _selectedUnits[i];
                
                if (i == 1)
                {
                    lastUnit = _currentUnit;
                }
                
                FollowUnit(unit, lastUnit);
                
                lastUnit = unit;
            }            
        }
    }
    
    private void MoveUnit(Vector2 targetPosition, Unit selectedUnit)
    {
        GridPosition clickGridPosition = _levelGrid.GetGridPosition(targetPosition);
        TileGridObject tileGridObject = _levelGrid.GetTileGridObject(clickGridPosition);
        if (_levelGrid.IsOnGrid(clickGridPosition) && !tileGridObject.isOccupied && tileGridObject.isWalkable)
        {
            List<Vector2> path = _pathfinding.FindPath(_levelGrid.GetGridPosition(selectedUnit.transform.position), clickGridPosition, true);
            if (!selectedUnit.isExecuting)
            {
                selectedUnit.Move(path);
            }
        }
    }

    private void FollowUnit(Unit follower, Unit unitToFollow)
    {
        GridPosition followPosition = _levelGrid.GetGridPosition(unitToFollow.transform.position);
        if (_levelGrid.IsOnGrid(followPosition))
        {
            List<Vector2> path = _pathfinding.FindPath(_levelGrid.GetGridPosition(follower.transform.position), followPosition, false);
            if (!follower.isExecuting)
            {
                if (path.Count > 0)
                {
                    path.RemoveAt(path.Count - 1);
                    follower.Move(path);   
                }
            }
        }
    }

    //Save startpoint
    //Check for difference between current mouseposition and startpoint
    public void OnBoxSelection(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _isDragging = true;
            _unitsFollowing = false;
            _selectedUnits.Clear();
            _startPoint = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        }

        if (context.canceled)
        {
            _isDragging = false;
            _selectedUnits = _selectionBox.GetSelection();
            _selectionBox.Clear();

            if (_selectedUnits.Count > 0)
            { 
                _currentUnit = _selectedUnits[0];   
            }
        }
    }
    
    public void OnMouseClick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            
            RaycastHit2D hit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(mousePosition));
            if (hit.collider)
            {
                if (hit.collider.TryGetComponent<Unit>(out Unit selectedUnit))
                {
                    _currentUnit = selectedUnit;
                    _selectedUnits.Clear();
                }
            }
            
            //Trying to move units;
            _targetPosition = Camera.main.ScreenToWorldPoint(mousePosition);;
            if (_selectedUnits.Count > 1)
            {
                MoveUnit(_targetPosition, _currentUnit);
                _unitsFollowing = true;
            }
            else
            {
                _unitsFollowing = false;
                MoveUnit(_targetPosition, _currentUnit);
            }
        }
    }

    public Unit GetCurrentUnit()
    {
        return _currentUnit;
    }
}
