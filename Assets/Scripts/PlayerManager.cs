using System;
using System.Collections.Generic;
using CustomInput;
using UnitSystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    [Header("Scene Objects")]
    [SerializeField] private LevelGrid _levelGrid;
    [SerializeField] private Camera _playerCamera;
    [SerializeField] private Transform _crosshairPrefab;

    [Header("Scriptable Objects")]
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private TurnEventsHandler _turnEventsHandler;
    
    //Units
    [Header("Player Unit")]
    [SerializeField] private PlayerUnit playerPlayerUnit;
    
    //Mouse
    [SerializeField] private Transform _mouseOnTileVisual;
    private Vector2 _mousePosition;
    private Vector2 _mouseWorldPosition;
    private Vector2 _lastMouseWorldPosition;
    
    //Selection Box
    [SerializeField] private SelectionBox _selectionBox;
    private CrosshairController _crosshairController;
    
    private Vector2 _startPoint;
    private Vector2 _endPoint;
    private bool _isDragging;
    private bool _isOverUI;
    
    private bool _isReloading;
    private bool _isAiming;
    
    [SerializeField] private float _maxTurnTime;
    private float _turnTimer;
    //Pathfinding
    private Pathfinding _pathfinding;
    private List<Vector2> _path;
    private Type _actionType;
    private List<TileGridObject> _highlights;
    
    private void Awake()
    {
        _actionType = typeof(MoveAction);
        if (_crosshairController == null)
        {
            _crosshairController = Instantiate(_crosshairPrefab).GetComponent<CrosshairController>();
            _crosshairController.Initialize(playerPlayerUnit);
        }

        _turnTimer = _maxTurnTime;
        _inputReader.MouseClickStop += MouseClick;
        _inputReader.ShootStart += MouseClick;
        _inputReader.MouseMoveStartEvent += MouseMoveStart;
        _inputReader.PlayerMoveEvent += InputReader_MoveUnit;
        _inputReader.PlayerClickEvent += InputReader_UnitExecuteAction;
        
        _inputReader.ReloadStart += InputReader_Reload;
        _inputReader.AimStart += InputReader_Aim;
        _inputReader.AimStop += InputReader_AimStop;
    }

    private void Start()
    {
        playerPlayerUnit.Initialize(_levelGrid);
        playerPlayerUnit.OnUnitMove += _levelGrid.Unit_OnUnitMoved;
        playerPlayerUnit.OnUnitMove += Unit_OnUnitMoved;
        playerPlayerUnit.OnUnitShoot += Unit_OnUnitShoot;
        
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

        if (_isAiming)
        {
            Aim();
        }
        
        if (_isAiming || _isReloading)
        {
            _turnTimer -= Time.deltaTime;
            if (_turnTimer <= 0)
            {
                _turnTimer = _maxTurnTime;
                _turnEventsHandler.PlayerActed();
                Debug.Log("ADVANCING TURN");
            }
        }
    }

    private void Aim()
    {
        playerPlayerUnit.Aim(_crosshairController.transform.position);
        playerPlayerUnit.FlipSprite(_mouseWorldPosition);
        //Turn timer

        _actionType = typeof(ShootAction);
    }
    
    private void InputReader_UnitExecuteAction(object sender, ClickEventArgs e)
    {
        //Do shoot action;
        if (!playerPlayerUnit.isExecuting)
        {
            playerPlayerUnit.TakeAction(_playerCamera.ScreenToWorldPoint(e.m_Target), _actionType);
            playerPlayerUnit.ExecuteAction();

            if (_actionType != typeof(MoveAction))
            {
                _turnTimer = _maxTurnTime;
                _turnEventsHandler.PlayerActed();
            }
        }
    }
    private void InputReader_MoveUnit(object sender, MoveEventArgs e)
    {
        Vector2 gridWorldPosition = _levelGrid.GetWorldPositionOnGrid(e.m_Direction);
        Vector2 targetPosition = _levelGrid.GetWorldPositionOnGrid(playerPlayerUnit.transform.position) + gridWorldPosition;

        if (!playerPlayerUnit.isExecuting)
        {
            playerPlayerUnit.TakeAction(targetPosition, typeof(MoveAction));
            playerPlayerUnit.ExecuteAction();
        }
    }

    private void InputReader_Reload()
    {
        if (!playerPlayerUnit.isExecuting)
        {
            Debug.Log("Reloaded! ");
            playerPlayerUnit.Reload();
        }
    }
    private void InputReader_Aim()
    {
        _isAiming = true;
        _crosshairController.gameObject.SetActive(true);
        _mouseOnTileVisual.gameObject.SetActive(false);
    }
    private void InputReader_AimStop()
    {
        _isAiming = false;
        _actionType = typeof(MoveAction);
        playerPlayerUnit.StopAim();
        _crosshairController.gameObject.SetActive(false);
        _mouseOnTileVisual.gameObject.SetActive(true);
    }
    private void Unit_OnUnitShoot(object sender, EventArgs e)
    {
        _crosshairController.Shoot(_mouseWorldPosition);
    }
    private void Unit_OnUnitMoved(object sender, UnitMovedEventArgs e)
    {
        _turnEventsHandler.PlayerActed();
        if (_highlights.Count > 0)
        {
            Vector2 positionToRemove = new Vector2(e.targetPosition.x, e.targetPosition.y);
            int index = _highlights.FindIndex(t => t.m_WorldPosition == positionToRemove);
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
            
            _mouseOnTileVisual.position = _levelGrid.GetWorldPositionOnGrid(_mouseWorldPosition);
            _crosshairController.GetMousePosition(_mouseWorldPosition);
            
            if (_actionType == typeof(MoveAction) && 
                _levelGrid.GetWorldPositionOnGrid(_lastMouseWorldPosition) != _levelGrid.GetWorldPositionOnGrid(_mouseWorldPosition))
            {
                //Show preview
                _lastMouseWorldPosition = _mouseWorldPosition;
                //_playerUnit.TakeAction(_mouseWorldPosition, _actionType);
            }
        }
    }
    
    public void MouseClick()
    {
        if (!_isOverUI)
        {
            //Check what state the current unit is in
            //_playerUnit.CloseUI();
            Ray mouseRay = _playerCamera.ScreenPointToRay(_mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mouseRay.origin, mouseRay.direction);
        }
    }

    public PlayerUnit GetCurrentUnit()
    {
        return playerPlayerUnit;
    }
}
