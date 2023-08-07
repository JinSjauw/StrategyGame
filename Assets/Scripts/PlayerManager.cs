using System;
using System.Collections.Generic;
using CustomInput;
using UnitSystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Player
{
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
        [SerializeField] private PlayerUnit _playerUnit;
        
        //Mouse
        [SerializeField] private Transform _mouseOnTileVisual;
        private Vector2 _mousePosition;
        private Vector2 _mouseWorldPosition;
        private Vector2 _lastMouseWorldPosition;
        
        //Selection Box
        [SerializeField] private SelectionBox _selectionBox;
        //Not In use
        private Vector2 _startPoint;
        private Vector2 _endPoint;
        private bool _isDragging;
        //
        private CrosshairController _crosshairController;
        private InventoryController _inventoryController;
        private LoadoutSystem _loadoutSystem;
        private List<TileGridObject> _visibleTiles = new List<TileGridObject>();
        
        private bool _isOverUI;
        private bool _isReloading;
        private bool _isAiming;
        
        [SerializeField] private float _maxTurnTime;
        private float _turnTimer;
        
        private List<Vector2> _path;
        private Type _actionType;
        private List<TileGridObject> _highlights;
        
        private void Start()
        { 
            InitializePlayer();
            SubscribeToInput();
            Initialize();
        }
        
        private void InitializePlayer()
        {
            _playerUnit.Initialize(_levelGrid);
            _playerUnit.OnUnitMove += _levelGrid.Unit_OnUnitMoved;
            _playerUnit.OnUnitMove += Unit_OnUnitMoved;
            _playerUnit.OnUnitShoot += Unit_OnUnitShoot;
            
            _highlights = new List<TileGridObject>();
            UpdateVision();
        }
        private void SubscribeToInput()
        {
            _inputReader.MouseClickStop += MouseClick;
            _inputReader.MouseMoveStartEvent += MouseMoveStart;
            _inputReader.PlayerMoveEvent += InputReader_MoveUnit;
            _inputReader.PlayerClickEvent += InputReader_UnitExecuteAction;
            _inputReader.ReloadStart += InputReader_Reload;
            _inputReader.AimStart += InputReader_Aim;
            _inputReader.AimStop += InputReader_AimStop;
        }
        private void Initialize()
        {
            _actionType = typeof(MoveAction);
            _turnTimer = _maxTurnTime;
            
            if (_crosshairController == null)
            {
                _crosshairController = Instantiate(_crosshairPrefab).GetComponent<CrosshairController>();
                _crosshairController.Initialize(_playerUnit);
            }
            _crosshairController.OnWeaponChanged(_playerUnit.weapon);
            
            _inventoryController = GetComponent<InventoryController>();
            _inventoryController.Initialize(_inputReader);

            _loadoutSystem = GetComponent<LoadoutSystem>();
            _loadoutSystem.Initialize(_playerUnit, _inputReader, OnWeaponChanged);
        }
        
        //NEEDS A REFACTOR
        //Split up into PlayerController & PlayerManager
        private void Update()
        {
            //Detect if pointer is over UI
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                _isOverUI = EventSystem.current.IsPointerOverGameObject(PointerInputModule.kMouseLeftId);
            }
            if (_playerUnit == null)
            {
                return;
            }
            /*if (_isDragging)
            {
                _endPoint = _playerCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
                _selectionBox.DrawSelectionBox(_startPoint, _endPoint);
            }*/
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

        private void OnWeaponChanged()
        {
            _crosshairController.OnWeaponChanged(_playerUnit.weapon);
        }
        
        private void Aim()
        {
            _playerUnit.Aim(_crosshairController.transform.position);
            _playerUnit.FlipSprite(_mouseWorldPosition);
            
            _actionType = typeof(ShootAction);
        }
        
        //Fog of war
        private void UpdateVision()
        {
            //Debug.Log("Visible Tile List: " + _visibleTiles.Count);
            for (int i = 0; i < _visibleTiles.Count; i++)
            {
                //Debug.Log("Visible Tile Index: " + i);
                _visibleTiles[i].FogOn();
            }
            
            _visibleTiles.Clear();
            Vector2 playerPosition = _playerUnit.transform.position;
            float visionRadius = _playerUnit.unitData.detectionRadius;
            List<TileGridObject> circleArea = _levelGrid.GetTilesInCircle(playerPosition, visionRadius);
            List<TileGridObject> visibleArea = new List<TileGridObject>();

            for (int i = 0; i < circleArea.Count; i++)
            {
                if (!Physics2D.Linecast(playerPosition, circleArea[i].m_WorldPosition, LayerMask.GetMask("Enviroment")))
                {
                    visibleArea.Add(circleArea[i]);
                }
            }

            _visibleTiles = visibleArea;
            
            for (int i = 0; i < _visibleTiles.Count; i++)
            {
                _visibleTiles[i].ClearFog();
            }
        }

        private void InputReader_UnitExecuteAction(object sender, MouseEventArgs e)
        {
            if (_playerUnit == null) { return; }
            //Do shoot action;
            if (!_playerUnit.isExecuting)
            {
                _playerUnit.TakeAction(_playerCamera.ScreenToWorldPoint(e.MousePosition), _actionType);
                _playerUnit.ExecuteAction();

                if (_actionType != typeof(MoveAction))
                {
                    _turnTimer = _maxTurnTime;
                    _turnEventsHandler.PlayerActed();
                }
            }
        }
        private void InputReader_MoveUnit(object sender, MoveEventArgs e)
        {
            if (_playerUnit == null) { return; }

            Vector2 gridWorldPosition = _levelGrid.GetWorldPositionOnGrid(e.m_Direction);
            Vector2 targetPosition = _levelGrid.GetWorldPositionOnGrid(_playerUnit.transform.position) + gridWorldPosition;

            if (!_playerUnit.isExecuting)
            {
                _playerUnit.TakeAction(targetPosition, typeof(MoveAction));
                _playerUnit.ExecuteAction();
            }
        }

        private void InputReader_Reload()
        {
            if (_playerUnit == null) { return; }

            if (!_playerUnit.isExecuting)
            {
                Debug.Log("Reloaded! ");
                _playerUnit.Reload();
            }
        }
        private void InputReader_Aim()
        {
            if (_playerUnit == null) { return; }
            
            if(_playerUnit.weapon == null) { return; }
            
            _isAiming = true;
            _crosshairController.gameObject.SetActive(true);
            _mouseOnTileVisual.gameObject.SetActive(false);
        }
        private void InputReader_AimStop()
        {
            _isAiming = false;
            _actionType = typeof(MoveAction);
            _playerUnit.StopAim();
            _crosshairController.gameObject.SetActive(false);
            _mouseOnTileVisual.gameObject.SetActive(true);
        }
        private void Unit_OnUnitShoot(object sender, EventArgs e)
        {
            _crosshairController.Shoot(_mouseWorldPosition);
        }
        private void Unit_OnUnitMoved(object sender, UnitMovedEventArgs e)
        {
            UpdateVision();
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
                    highlightToRemove.m_TileVisual.gameObject.SetActive(false);
                }
            }
        }
        public void MouseMoveStart()
        {
            if (!_isOverUI)
            {
                _mousePosition = Mouse.current.position.ReadValue();
                _mouseWorldPosition = _playerCamera.ScreenToWorldPoint(_mousePosition);
                
                _mouseOnTileVisual.position = _levelGrid.GetWorldPositionOnGrid(_mouseWorldPosition);
                _crosshairController.GetMousePosition(_mouseWorldPosition);
                
                if (_actionType == typeof(MoveAction) && 
                    _levelGrid.GetWorldPositionOnGrid(_lastMouseWorldPosition) != _levelGrid.GetWorldPositionOnGrid(_mouseWorldPosition))
                {
                    //Show preview
                    _lastMouseWorldPosition = _mouseWorldPosition;
                }
            }
        }
        public void MouseClick()
        {
            if (!_isOverUI)
            {
                Ray mouseRay = _playerCamera.ScreenPointToRay(_mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(mouseRay.origin, mouseRay.direction);
                
                //Check if in radius and if it hit an interactable
                //Outline Shader when hovering over
            }
        }
        public PlayerUnit GetCurrentUnit()
        {
            return _playerUnit;
        }
    }
}

