using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
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

    private bool _isOverUI;
    
    //Pathfinding
    private Pathfinding _pathfinding;
    private List<Vector2> _path;
    
    //Units
    private Unit _currentUnit;
    private List<Unit> _selectedUnits;
    private bool _unitsFollowing;
    private Unit _lastUnit;
    
    private void Start()
    {
        _pathfinding = new Pathfinding(_levelGrid);
        
        foreach (Unit unit in _playerUnits)
        {
            unit.Initialize(_pathfinding);
            unit.OnUnitMove += _levelGrid.Unit_OnUnitMoved;
        }
        
        _currentUnit = _playerUnits[0];
        _selectedUnits = new List<Unit>();
    }

    private void Update()
    {
        //Detect if pointer is over UI
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            _isOverUI = EventSystem.current.IsPointerOverGameObject(PointerInputModule.kMouseLeftId);
        }
        
        if (_isDragging)
        {
            _endPoint = _playerCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
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
            if (!selectedUnit.isExecuting)
            {
                //selectedUnit.Move(path);
                selectedUnit.SetAction(typeof(MoveAction), targetPosition);
                selectedUnit.StartAction();
            }
        }
    }

    private void FollowUnit(Unit follower, Unit unitToFollow)
    {
        GridPosition targetGridPosition = _levelGrid.GetGridPosition(unitToFollow.transform.position);
        GridPosition originPosition = _levelGrid.GetGridPosition(follower.transform.position);

        if (targetGridPosition.Distance(originPosition) <= 1.1)
        {
            return;
        }
        
        if (_levelGrid.IsOnGrid(targetGridPosition) && !follower.isExecuting)
        {
            follower.isFollowing = true;
            follower.SetAction(typeof(MoveAction), unitToFollow.transform.position);
            follower.StartAction();
        }
    }
    
    public void OnBoxSelection(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _isDragging = true;
            _unitsFollowing = false;
            _selectedUnits.Clear();
            _startPoint = _playerCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
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
        if (context.canceled && !_isOverUI)
        {
            //Check what state the current unit is in
            _currentUnit.CloseUI();
            
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Ray mouseRay = _playerCamera.ScreenPointToRay(mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mouseRay.origin, mouseRay.direction);
            if (hit.collider)
            {
                if (hit.collider.TryGetComponent(out Unit selectedUnit))
                {
                    if (_currentUnit != selectedUnit)
                    {
                        _currentUnit.CloseUI();
                        _currentUnit = selectedUnit;
                    }
                    else if (_currentUnit == selectedUnit)
                    {
                        _currentUnit.OpenUI();
                    }
                    _selectedUnits.Clear();
                    return;
                }

                if (hit.collider.GetComponent<Tilemap>())
                {
                    //If not in combat
                    if (_selectedUnits.Count > 1)
                    {
                        MoveUnit(hit.point, _currentUnit);
                        _unitsFollowing = true;
                    }
                    else
                    {
                        _currentUnit.isFollowing = false;
                        MoveUnit(hit.point, _currentUnit);
                        _unitsFollowing = false;
                    }    
                }
            }
        }
    }
    public Unit GetCurrentUnit()
    {
        return _currentUnit;
    }
}
