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

    [SerializeField] private InputReader _inputReader;

    [SerializeField] private bool _inCombat;
    
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

    private void Awake()
    {
        _inputReader.BoxSelectionStartEvent += BoxSelectionStart;
        _inputReader.BoxSelectionStopEvent += BoxSelectionStop;
        _inputReader.MouseClickStop += MouseClick;
        _inputReader.ShootStart += MouseClick;
        _inputReader.MouseMoveStartEvent += MouseMoveStart;
    }

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
        if (_highlights.Count > 0)
        {
            Vector2 positionToRemove = new Vector2(e.targetPosition.x, e.targetPosition.y);
            int index = _highlights.FindIndex(t => (Vector2)t.m_WorldPosition == positionToRemove);
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

    private void ShowPathPreview()
    {
        foreach (TileGridObject highlight in _highlights)
        {
            highlight.m_TileHighlight.gameObject.SetActive(false);
        }
        _highlights.Clear();
        
        _mousePosition = Mouse.current.position.ReadValue();
        Vector2 mouseWorldPosition = _playerCamera.ScreenToWorldPoint(_mousePosition);
        TileGridObject targetTile = _levelGrid.GetTileGridObject(mouseWorldPosition);

        if (_currentUnit.GetActionType() != typeof(MoveAction))
        {
            return;
        }
        
        if (_levelGrid.IsOnGrid(_levelGrid.GetGridPosition(mouseWorldPosition)) && 
            Vector2.Distance(mouseWorldPosition, _lastMousePosition) > _levelGrid.GetCellSize() && targetTile.isWalkable )
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
                if (actionType == typeof(MoveAction) && i == 0)
                {
                    continue;
                }
                
                Vector2 point = previewPoints[i];
                TileGridObject highlight = _levelGrid.GetTileGridObject(point);
                if(!_highlights.Contains(highlight))
                {
                    _highlights.Add(highlight);
                    highlight.m_TileHighlight.gameObject.SetActive(true);
                }
            }
        }
    }
    public void BoxSelectionStart()
    {
        _isDragging = true;
        _unitsFollowing = false;
        _selectedUnits.Clear();
        _startPoint = _playerCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
    }
    public void BoxSelectionStop()
    {
        _isDragging = false;
        _selectedUnits = _selectionBox.GetSelection();
        _selectionBox.Clear();

        if (_selectedUnits.Count > 0)
        { 
            _currentUnit = _selectedUnits[0];   
        }
    }
    public void MouseMoveStart()
    {
        if (!_isOverUI && !_currentUnit.isExecuting)
        {
            //Write specific functions to handle the Vector2 List for the previews
            //Get rid of the previous points
            
            //In combat check for type of action;
            //Replace with dictionary<Type , Action> for handling the previews per ability category?
            
            ShowPathPreview();
        }
    }
    
    public void MouseClick()
    {
        if (!_isOverUI)
        {
            //Check what state the current unit is in
            _currentUnit.CloseUI();
            Ray mouseRay = _playerCamera.ScreenPointToRay(_mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mouseRay.origin, mouseRay.direction);
            
            if (hit.collider)
            {
                if (_inCombat)
                {
                    ExecuteAction();
                    return;
                }
                
                if (hit.collider.TryGetComponent(out Unit selectedUnit) && !selectedUnit.isEnemy)
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
                if (!_levelGrid.isTileWalkable(hit.point) && !_inCombat)
                {
                    _mouseOnTileVisual.gameObject.SetActive(false);
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
