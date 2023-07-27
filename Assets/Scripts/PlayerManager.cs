using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    //[SerializeField] private List<Unit> _playerUnits;
    [SerializeField] private LevelGrid _levelGrid;
    [SerializeField] private Camera _playerCamera;
    [SerializeField] private Transform _crosshairPrefab;
    [SerializeField] private CrosshairController _crosshairController;
    
    [SerializeField] private InputReader _inputReader;
    
    //Mouse
    [SerializeField] private Transform _mouseOnTileVisual;
    private Vector2 _mousePosition;
    private Vector2 _mouseWorldPosition;
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
    [SerializeField] private Unit _playerUnit;

    private List<TileGridObject> _highlights;

    private void Awake()
    {
        /*_inputReader.BoxSelectionStartEvent += BoxSelectionStart;
        _inputReader.BoxSelectionStopEvent += BoxSelectionStop;*/
        if (_crosshairController == null)
        {
            _crosshairController = Instantiate(_crosshairPrefab).GetComponent<CrosshairController>();
            _crosshairController.Initialize(_playerUnit);
        }
        
        _inputReader.MouseClickStop += MouseClick;
        _inputReader.ShootStart += MouseClick;
        _inputReader.MouseMoveStartEvent += MouseMoveStart;
        _inputReader.PlayerMoveEvent += InputReader_MoveUnit;
        _inputReader.PlayerClickEvent += InputReader_UnitExecuteAction;
        _inputReader.ReloadStart += InputReader_Reload;
    }

    private void Start()
    {
        _pathfinding = new Pathfinding(_levelGrid);
        _playerUnit.Initialize(_pathfinding);
        _playerUnit.OnUnitMove += _levelGrid.Unit_OnUnitMoved;
        _playerUnit.OnUnitMove += Unit_OnUnitMoved;
        _playerUnit.OnUnitShoot += Unit_OnUnitShoot;

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
        _playerUnit.Aim(_crosshairController.transform.position);
    }
    
    private void InputReader_UnitExecuteAction(object sender, ClickEventArgs e)
    {
        //Do shoot action;
        if (!_playerUnit.isExecuting)
        {
            //Return CrosshairController.RandomSpreadPoint()
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

    private void InputReader_Reload()
    {
        if (!_playerUnit.isExecuting)
        {
            Debug.Log("Reloaded! ");
            _playerUnit.Reload();
        }
    }
    
    private void Unit_OnUnitShoot(object sender, EventArgs e)
    {
        _crosshairController.Shoot(_mouseWorldPosition);
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
    
    public void MouseMoveStart()
    {
        if (!_isOverUI)
        {
            //Write specific functions to handle the Vector2 List for the previews
            //Get rid of the previous points
            
            //In combat check for type of action;
            //Replace with dictionary<Type , Action> for handling the previews per ability category?
            
            _mousePosition = Mouse.current.position.ReadValue();
            _mouseWorldPosition = _playerCamera.ScreenToWorldPoint(_mousePosition);
            _crosshairController.GetMousePosition(_mouseWorldPosition);
            _playerUnit.FlipSprite(_mouseWorldPosition);
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
        }
    }

    public Unit GetCurrentUnit()
    {
        return _playerUnit;
    }
}
