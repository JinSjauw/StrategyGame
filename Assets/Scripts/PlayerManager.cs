using System;
using System.Collections.Generic;
using ActionSystem;
using CustomInput;
using InventorySystem.Containers;
using InventorySystem.Items;
using Items;
using SoundManagement;
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

        [Header("Event Listeners")]
        [SerializeField] private InputReader _inputReader;
        [SerializeField] private TurnEventsHandler _turnEventsHandler;
        [SerializeField] private PlayerEventChannel _playerEventChannel;
        [SerializeField] private SFXEventChannel _sfxEventChannel;
        
        //Units
        [Header("Player Unit")] 
        [SerializeField] private GameObject _playerPrefab;
        [SerializeField] private PlayerUnit _playerUnit;
        //[SerializeField] private PlayerDataHolder _playerData;
        
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
        private bool _isAiming;
        private bool _isThrowing;

        [SerializeField] private float _maxTurnTime;
        private float _turnTimer;
        
        private List<Vector2> _path;
        private Type _actionType;
        private List<TileGridObject> _highlights;

        private void Awake()
        {
            _playerEventChannel.SendPlayerDataEvent += InitializePlayer;
        }

        private void OnDestroy()
        {
            _playerEventChannel.SendPlayerDataEvent -= InitializePlayer;
            _inputReader.MouseClickStop -= MouseClick;
            _inputReader.MouseMoveStartEvent -= MouseMoveStart;
            _inputReader.PlayerMoveEvent -= InputReader_MoveUnit;
            _inputReader.PlayerClickEvent -= InputReader_UnitExecuteAction;
            _inputReader.ReloadStart -= InputReader_Reload;
            _inputReader.AimStart -= InputReader_Aim;
            _inputReader.AimStop -= InputReader_AimStop;
        }

        private void Start()
        {
            _playerEventChannel.RequestPlayerSpawn();
        }

        private void InitializePlayer(object sender, Weapon weapon)
        {
            GameObject player = Instantiate(_playerPrefab, new Vector3(1, 1), Quaternion.identity);
            _playerUnit = player.GetComponent<PlayerUnit>();
            _inputReader.EnableGameplayInput();
            _playerUnit.Initialize(_levelGrid);
            _playerUnit.OnUnitMove += _levelGrid.Unit_OnUnitMoved;
            _playerUnit.OnUnitMove += Unit_OnUnitMoved;
            _playerUnit.OnUnitShoot += Unit_OnUnitShoot;
            
            _highlights = new List<TileGridObject>();
            UpdateVision();
            Initialize();
            _playerEventChannel.PlayerSpawned();
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
            _inventoryController.Initialize(_inputReader, _playerUnit);
            
            _loadoutSystem = GetComponent<LoadoutSystem>();
            _loadoutSystem.Initialize(_playerUnit, _inputReader, OnWeaponChanged);
            
            SubscribeToInput();
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
            _inputReader.ThrowAimStart += InputReader_ThrowStart;
            _inputReader.ThrowAimStop += InputReader_ThrowStop;
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
            if (_isAiming)
            {
                Aim();
            }

            if (_isAiming || _playerUnit.isReloading)
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
            if (_playerUnit.weapon == null) { return; }
            //Weapon Switch Sound
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
                Debug.Log("Reloading! ");
                _playerUnit.Reload();
            }
        }
        private void InputReader_Aim()
        {
            if (_playerUnit == null) { return; }
            
            if(_playerUnit.weapon == null) { return; }
            
            _playerUnit.StopPreviewAction();
            
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
        private void InputReader_ThrowStart()
        {
            Debug.Log("Throwing?");
            if (_inventoryController.GetPocketItem() == null) { return; }

            if (_inventoryController.GetPocketItem().GetItemType() != ItemType.Throwable) { return; }

            if (_playerUnit.isExecuting || _playerUnit.isReloading) { return; }
            
            _playerUnit.SetSelectedItem(_inventoryController.GetPocketItem());
            _playerUnit.FlipSprite(_mouseWorldPosition);
            _actionType = typeof(ThrowAction);
            _playerUnit.StopPreviewAction();
            _playerUnit.TakeAction(_mouseWorldPosition, _actionType);
            _playerUnit.PreviewAction();
            //Show preview
        }
        private void InputReader_ThrowStop()
        {
            _playerUnit.StopPreviewAction();
            _actionType = typeof(MoveAction);
        }
        
        private void Unit_OnUnitShoot(object sender, EventArgs e)
        {
            _sfxEventChannel.RequestSFX(_playerUnit.weapon.GetSFXConfig().GetShootClip(), _playerCamera.transform.position);
            _crosshairController.Recoil(_mouseWorldPosition);
        }
        private void Unit_OnUnitMoved(object sender, UnitMovedEventArgs e)
        {
            _sfxEventChannel.RequestSFX(_playerUnit.GetUnitSFX().GetWalkClip(), _playerCamera.transform.position, 0.8f);
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
            _mousePosition = Mouse.current.position.ReadValue();
            _mouseWorldPosition = _playerCamera.ScreenToWorldPoint(_mousePosition);
            
            _mouseOnTileVisual.position = _levelGrid.GetWorldPositionOnGrid(_mouseWorldPosition);
            _crosshairController.GetMousePosition(_mouseWorldPosition);
            
            if (_actionType == typeof(MoveAction) && !_playerUnit.isExecuting &&
                _levelGrid.GetWorldPositionOnGrid(_lastMouseWorldPosition) != _levelGrid.GetWorldPositionOnGrid(_mouseWorldPosition))
            {
                //Show preview
                _lastMouseWorldPosition = _mouseWorldPosition;
                _playerUnit.TakeAction(_mouseWorldPosition, _actionType);
                _playerUnit.PreviewAction();
                //Debug.Log("Previewing MoveAction");
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

                if (hit.collider)
                {
                    Debug.Log("ColliderName: " + hit.collider.name);
                    
                    LootContainer lootContainer = hit.collider.GetComponent<LootContainer>();
                    if (lootContainer != null)
                    {
                        _inventoryController.OpenLootContainer(lootContainer);
                        return;
                    }

                    ItemWorldContainer itemWorldContainer = hit.collider.GetComponent<ItemWorldContainer>();
                    if (itemWorldContainer != null)
                    {
                        _inventoryController.PickUpWorldItem(itemWorldContainer);
                    }
                    
                }
            }
        }
        public PlayerUnit GetCurrentUnit()
        {
            return _playerUnit;
        }
    }
}

