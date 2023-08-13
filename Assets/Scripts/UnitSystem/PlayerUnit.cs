using System;
using System.Collections.Generic;
using ActionSystem;
using InventorySystem;
using Items;
using Player;
using SoundManagement;
using UnityEngine;
using UnityEngine.UI;

namespace UnitSystem
{
    public class PlayerUnit : Unit
    {
        //Temp --> Action Collection
        [SerializeField] private List<BaseAction> _actions;
        //Put it into a dictionary
        private Dictionary<Type, BaseAction> _actionDictionary;
        private BaseAction _selectedAction;
        
        //Unit Events
        //private event EventHandler<UnitMovedEventArgs> _onUnitMove;
        private event EventHandler _onUnitShoot;
        private event EventHandler _onUnitReloadStart;
        private event EventHandler _onUnitReloadFinish;
        
        [SerializeField] private SFXEventChannel _sfxChannel;
        [SerializeField] protected UnitSFXConfig _unitSfxConfig;
        
        [Header("ReloadSystem ##WIP")] 
        [SerializeField] private InventoryEvents _inventoryEvents;

        [Header("Weapon UI Events #WIP")] 
        [SerializeField] private PlayerHUDEvents _playerHUD;
        //Unit UI
        [SerializeField] private UIController _unitUI;

        private bool _isExecuting;
        private bool _isReloading;
        
        
        private List<Vector2> _actionResults;
        
        private Coroutine _reloadRoutine;
        private bool _failedReload;
        private Slider _reloadBar;

        private ItemContainer _selectedItem;
        
        //public EventHandler<UnitMovedEventArgs> OnUnitMove { get => _onUnitMove; set => _onUnitMove = value; }
        public EventHandler OnUnitShoot { get => _onUnitShoot; set => _onUnitShoot = value; }
        public bool isExecuting { get => _isExecuting; }
        public bool isReloading { get => _isReloading; }
        public LevelGrid levelGrid { get => _levelGrid; }
        public ItemContainer selectedItem { get => _selectedItem; set => _selectedItem = value; }

        private void Start()
        {
            _onUnitReloadStart += _unitUI.OpenUI;
            _onUnitReloadFinish += _unitUI.CloseUI;
            _inventoryEvents.SendAmmo += SaveAmmo;
        }

        private void SaveAmmo(object sender, List<Bullet> bullets)
        {
            if(_currentWeapon == null) return;
            
            Debug.Log(_currentWeapon.name);
            _currentWeapon.GiveBullets(bullets);
        }

        private void Update()
        {
            if (_isExecuting)
            {
                _selectedAction.Execute();
            }

            if (_currentWeapon != null)
            {
                CheckReload();
            }
        }

        private void OnActionComplete()
        {
            _isExecuting = false;
        }

        private void OnShoot()
        {
            _playerHUD.RaisePlayerShoot(1);
            _onUnitShoot?.Invoke(this, EventArgs.Empty);
        }

        private void CheckReload()
        {
            if (_currentWeapon.ReloadTimer >= 1)
            {
                FinishReload();
            }
            else
            {
                _unitUI.ReloadBar.value = _currentWeapon.ReloadTimer;
            }
        }
        
        private void FinishReload()
        {
            _playerHUD.RaisePlayerReload(weapon.AmmoCapacity);
            StopCoroutine(_reloadRoutine);
            _reloadRoutine = null;
            _currentWeapon.ReloadTimer = 0;
            //Pass a list on to this function
            _currentWeapon.Load();
            _failedReload = false;
            _isReloading = false;
            _onUnitReloadFinish?.Invoke(this, EventArgs.Empty);
            _sfxChannel.RequestSFX(_currentWeapon.GetSFXConfig().GetLoadClip(), Camera.main.transform.position);
        }

        private void StopReload()
        {
            if (_reloadRoutine != null)
            {
                StopCoroutine(_reloadRoutine);
                _reloadRoutine = null;
            }
            
            _currentWeapon.ReloadTimer = 0;
            _failedReload = false;
        }
        
        public override void Initialize(LevelGrid levelGrid)
        {
            _levelGrid = levelGrid;
            _pathfinding = new Pathfinding(levelGrid);
            _actionDictionary = new Dictionary<Type, BaseAction>();
            
            for (int i = 0; i < _actions.Count; i++)
            {
                BaseAction action = Instantiate(_actions[i]);
                action.Initialize(this, OnActionComplete);
                _actionDictionary[action.GetType()] = action;
            }
            _selectedAction = _actionDictionary[typeof(MoveAction)];
            _selectedAction.SetAction(Vector2.zero);
        }

        public UnitSFXConfig GetUnitSFX()
        {
            return _unitSfxConfig;
        }
        
        //Call an Weapon.Aim() to have specific implementation (exampl. Scope view(Inverse Mask))
        public override void Aim(Vector2 target)
        {
            Vector2 targetDirection = target - (Vector2)_weaponRenderer.transform.position;
            float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
            _weaponRenderer.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        public void Reload()
        {
            Debug.Log($"Current Weapon {_currentWeapon.name} {_currentWeapon.isLoaded}");
            if (_currentWeapon.isLoaded)
            {
                _currentWeapon.Eject();
                _playerHUD.RaisePlayerReload(0);
                _inventoryEvents.OnRequestAmmo(_currentWeapon.AmmoCapacity);
                _sfxChannel.RequestSFX(_currentWeapon.GetSFXConfig().GetEjectClip(), Camera.main.transform.position);
                return;
            }
            if (_currentWeapon.BulletAmount <= 0)
            {
                Debug.Log("NO AMMO");
                return;
            }
            if (_reloadRoutine == null)
            {
                _isReloading = true;
                _onUnitReloadStart?.Invoke(this, EventArgs.Empty);
                _reloadRoutine = StartCoroutine(_currentWeapon.ReloadRoutine());
                return;
            }
            if (_failedReload)
            {
                return;
            }
            if (_currentWeapon.ReloadTimer >= .6f && _currentWeapon.ReloadTimer <= .8f)
            {
                FinishReload();
            }
            else
            {
                _failedReload = true;
            }
        }

        public void EquipWeapon(Weapon weaponToEquip)
        {
            _currentWeapon = weaponToEquip;
            if (_currentWeapon != null)
            {
                _currentWeapon = _currentWeapon.Equip(weaponRenderer, OnShoot);
                _playerHUD.RaiseWeaponSwitched(_currentWeapon.GetSprite());
                Debug.Log("Equipped: " + _currentWeapon.name);
            }
        }
        
        public void TakeAction(Vector2 target, Type actionType)
        {
            _selectedAction = _actionDictionary[actionType];
            _selectedAction.SetAction(target);
        }
        public void PreviewAction()
        {
            _selectedAction.Preview();
        }
        public void StopPreviewAction()
        {
            _selectedAction.StopPreview();
        }
        
        public void ExecuteAction()
        {
            //Invoke actionTaken event to advance turn
            _isExecuting = true;
        }
        public Type GetActionType()
        {
            return _selectedAction.GetType();
        }

        public void UnEquip()
        {
            if (_currentWeapon == null) { return; }
            StopReload();
            _weaponRenderer.sprite = null;
            _currentWeapon = null;
        }

        public void SetSelectedItem(ItemContainer pocketItem)
        {
            _selectedItem = pocketItem;
        }
    }
}

