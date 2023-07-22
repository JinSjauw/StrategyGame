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
    private Unit _playerUnit;

    private List<TileGridObject> _highlights;

    private void Awake()
    {
        /*_inputReader.BoxSelectionStartEvent += BoxSelectionStart;
        _inputReader.BoxSelectionStopEvent += BoxSelectionStop;*/
        _inputReader.MouseClickStop += MouseClick;
        _inputReader.ShootStart += MouseClick;
        _inputReader.MouseMoveStartEvent += MouseMoveStart;
        _inputReader.PlayerMoveEvent += InputReader_MoveUnit;
        _inputReader.PlayerClickEvent += InputReader_UnitExecuteAction;
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
        
        _playerUnit = _playerUnits[0];
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
        
        _playerUnit.Aim(_playerCamera.ScreenToWorldPoint(_mousePosition));
    }

    private void InputReader_UnitExecuteAction(object sender, ClickEventArgs e)
    {
        //Do shoot action;
        if (!_playerUnit.isExecuting)
        {
            _playerUnit.TakeAction(_playerCamera.ScreenToWorldPoint(e.m_Target), typeof(ShootAction));
            _playerUnit.ExecuteAction();
        }
    }
    
    private void InputReader_MoveUnit(object sender, MoveEventArgs e)
    {
        Vector2 gridWorldPosition = _levelGrid.GetWorldPositionOnGrid(e.m_Direction);
        Vector2 targetPosition = _levelGrid.GetWorldPositionOnGrid(_playerUnit.transform.position) + gridWorldPosition;

        _playerUnit.TakeAction(targetPosition, typeof(MoveAction));
        _playerUnit.ExecuteAction();
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
    
    /*public void BoxSelectionStart()
    {
        _isDragging = true;
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
    }*/

    public void MouseMoveStart()
    {
        if (!_isOverUI)
        {
            //Write specific functions to handle the Vector2 List for the previews
            //Get rid of the previous points
            
            //In combat check for type of action;
            //Replace with dictionary<Type , Action> for handling the previews per ability category?
            
            _mousePosition = Mouse.current.position.ReadValue();
            //_playerUnit.weaponSprite.transform.LookAt(_playerCamera.ScreenToWorldPoint(_mousePosition));
        }
    }
    
    public void MouseClick()
    {
        if (!_isOverUI)
        {
            //Check what state the current unit is in
            _playerUnit.CloseUI();
            Ray mouseRay = _playerCamera.ScreenPointToRay(_mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mouseRay.origin, mouseRay.direction);
            
            //Remove this.
            if (hit.collider)
            {
                //Remove this.
                if (hit.collider.TryGetComponent(out Unit selectedUnit) && !selectedUnit.isEnemy)
                {
                    if (_playerUnit != selectedUnit)
                    {
                        _playerUnit.CloseUI();
                        _playerUnit = selectedUnit;
                        //_currentUnit.SetAction(typeof(MoveAction));
                    }
                    else if (_playerUnit == selectedUnit)
                    {
                        _playerUnit.OpenUI();
                    }
                    
                    return;
                }

                //If already has an action selected
                //_playerUnit.ExecuteAction();
            }
        }
    }

    public Unit GetCurrentUnit()
    {
        return _playerUnit;
    }
}
