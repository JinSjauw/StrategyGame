using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private List<Unit> _playerUnits;
    [SerializeField] private LevelGrid _levelGrid;
    [SerializeField] private Camera _playerCamera;
    
    //Mouse
    [SerializeField] private Transform _mouseOnTileVisual;
    private Vector2 _mousePosition;
    private Vector2 _lastMousePosition;
    
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

    private List<TileGridObject> _highlights;
    
    private void Start()
    {
        _pathfinding = new Pathfinding(_levelGrid);
        
        foreach (Unit unit in _playerUnits)
        {
            unit.Initialize(_pathfinding);
            unit.OnUnitMove += _levelGrid.Unit_OnUnitMoved;
            unit.OnUnitMove += Unit_OnUnitMoved;
        }
        
        _currentUnit = _playerUnits[0];
        _selectedUnits = new List<Unit>();
        _highlights = new List<TileGridObject>();
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

    private void Unit_OnUnitMoved(object sender, UnitMovedEventArgs e)
    {
        Debug.Log(e.unit.gameObject.name + " MOVED" + " Count: " + _highlights.Count);
        if (_highlights.Count > 0)
        {
            Vector2 positionToRemove = new Vector2(e.targetPosition.x, e.targetPosition.y);
            int index = _highlights.FindIndex(t => (Vector2)t.m_WorldPosition == positionToRemove);

            Debug.Log(positionToRemove + " Index: " + index);
            
            if (index == -1)
            {
                return;
            }

            for (int i = 0; i < index; i++)
            {
                TileGridObject highlightToRemove = _highlights[i];
                highlightToRemove.m_TileHighlight.gameObject.SetActive(false);
            }
        }
    }
    
    private void ExecuteAction()
    {
        if (!_currentUnit.isExecuting && _currentUnit.GetActionType() != null)
        {
            _currentUnit.StartAction();
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
                selectedUnit.SetAction(typeof(MoveAction), targetPosition);
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

    public void OnMouseMove(InputAction.CallbackContext context)
    {
        if (context.started && !_isOverUI && !_currentUnit.isExecuting)
        {
            //Get rid of the previous points
            foreach (TileGridObject highlight in _highlights)
            {
                highlight.m_TileHighlight.gameObject.SetActive(false);
            }
            _highlights.Clear();
            
            _mousePosition = Mouse.current.position.ReadValue();
            Vector2 mouseWorldPosition = _playerCamera.ScreenToWorldPoint(_mousePosition);
            if (Vector2.Distance(mouseWorldPosition, _lastMousePosition) > _levelGrid.GetCellSize() &&
                _levelGrid.GetTileGridObject(mouseWorldPosition).isWalkable)
            {
                _mouseOnTileVisual.position = _levelGrid.GetWorldPositionOnGrid(mouseWorldPosition);
                _mouseOnTileVisual.gameObject.SetActive(true);
                MoveUnit(mouseWorldPosition, _currentUnit);
                Type actionType = null;
                List<Vector2> previewPoints = new List<Vector2>();
                previewPoints = _currentUnit.PreviewAction(out actionType);
                //Spawn some prefabs on all cells
                for (int i = 0; i < previewPoints.Count; i++)
                {
                    Vector2 point = previewPoints[i];
                    TileGridObject highlight = _levelGrid.GetTileGridObject(point);
                    Debug.Log(point + " TGOBJ: " + highlight + " Index: " + i);
                    if(!_highlights.Contains(highlight))
                    {
                        _highlights.Add(highlight);
                        highlight.m_TileHighlight.gameObject.SetActive(true);
                    }
                }
            }
            //In combat check for type of action;
        }
    }
    
    public void OnMouseClick(InputAction.CallbackContext context)
    {
        if (context.canceled && !_isOverUI)
        {
            //Check what state the current unit is in
            _currentUnit.CloseUI();

            //_mousePosition = Mouse.current.position.ReadValue();
            Ray mouseRay = _playerCamera.ScreenPointToRay(_mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mouseRay.origin, mouseRay.direction);
            if (hit.collider)
            {
                if (hit.collider.TryGetComponent(out Unit selectedUnit))
                {
                    if (_currentUnit != selectedUnit)
                    {
                        _currentUnit.CloseUI();
                        _currentUnit = selectedUnit;
                        _currentUnit.SetAction(typeof(MoveAction), hit.point);
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
                        ExecuteAction();
                        _unitsFollowing = true;
                    }
                    else
                    {
                        _currentUnit.isFollowing = false;
                        ExecuteAction();
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
