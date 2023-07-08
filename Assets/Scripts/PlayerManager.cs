using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private List<Unit> _playerUnits;
    [SerializeField] private LevelGrid _levelGrid;
    [SerializeField] private Camera _playerCamera;
    
    //Selection Box
    [SerializeField] private SelectionBox _selectionBox;
    private Vector2 _startPoint;
    private Vector2 _endPoint;
    private bool _isDragging;

    //Pathfinding
    private Pathfinding _pathfinding;
    private List<Vector2> _path;
    
    //Units
    private Unit _currentUnit;
    private List<Unit> _selectedUnits;
    private bool _unitsFollowing;
    private Unit _lastUnit;
    
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
                    _lastUnit = _currentUnit;
                }
                FollowUnit(unit, _lastUnit);
                _lastUnit = unit;
            }            
        }
    }
    
    private void MoveUnit(Vector2 targetPosition, Unit selectedUnit)
    {
        GridPosition targetGridPosition = _levelGrid.GetGridPosition(targetPosition);
        GridPosition originPosition = _levelGrid.GetGridPosition(selectedUnit.transform.position);

        if (!_levelGrid.IsOnGrid(targetGridPosition))
        {
            Debug.Log("TargetPos is not on the Grid! : " + targetGridPosition);
            return;
        }
        
        if (targetGridPosition == originPosition)
        {
            return;
        }
        
        TileGridObject tileGridObject = _levelGrid.GetTileGridObject(targetGridPosition);
        if (!tileGridObject.isOccupied && tileGridObject.isWalkable)
        {
            List<Vector2> path = _pathfinding.FindPath(originPosition, targetGridPosition, true);
            if (!selectedUnit.isExecuting)
            {
                selectedUnit.Move(path);
            }
        }
    }

    private void FollowUnit(Unit follower, Unit unitToFollow)
    {
        GridPosition targetGridPosition = _levelGrid.GetGridPosition(unitToFollow.transform.position);
        GridPosition originPosition = _levelGrid.GetGridPosition(follower.transform.position);

        if (targetGridPosition.Distance(originPosition) <= 1.5)
        {
            return;
        }
        
        if (_levelGrid.IsOnGrid(targetGridPosition))
        {
            List<Vector2> path = _pathfinding.FindPath(originPosition, targetGridPosition, false);
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
        //Need to see what state the unit is in
        //To determine whether to showcase potential targets for action
        //Or maybe character is stunned
        
        if (context.performed)
        {
            //Check what state the current unit is in
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Ray mouseRay = Camera.main.ScreenPointToRay(mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mouseRay.origin, mouseRay.direction);
            if (hit.collider)
            {
                if (hit.collider.TryGetComponent<Unit>(out Unit selectedUnit))
                {
                    if (_currentUnit != selectedUnit)
                    {
                        _currentUnit.CloseUI();
                        _currentUnit = selectedUnit;
                    }
                    else if (_currentUnit == selectedUnit)
                    {
                        Debug.Log("Open UnitMenu: " + _currentUnit.name);
                        _currentUnit.OpenUI();
                    }
                    _selectedUnits.Clear();
                    return;
                }
                
                _currentUnit.CloseUI();
                if (_selectedUnits.Count > 1)
                {
                    MoveUnit(hit.point, _currentUnit);
                    _unitsFollowing = true;
                }
                else
                {
                    MoveUnit(hit.point, _currentUnit);
                    _unitsFollowing = false;
                }
            }
        }
    }

    public Unit GetCurrentUnit()
    {
        return _currentUnit;
    }
}
