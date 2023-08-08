using System;
using System.Collections.Generic;
using ActionSystem;
using Items;
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

        //Unit UI
        [SerializeField] private UIController _unitUI;

        private bool _isExecuting;
        private bool _isFollowing;
        
        private List<Vector2> _actionResults;
        
        private Coroutine _reloadRoutine;
        private bool _failedReload;
        private Slider _reloadBar;

        //public EventHandler<UnitMovedEventArgs> OnUnitMove { get => _onUnitMove; set => _onUnitMove = value; }
        public EventHandler OnUnitShoot { get => _onUnitShoot; set => _onUnitShoot = value; }
        public bool isExecuting { get => _isExecuting; }

        private void Awake()
        {
            //_currentWeapon = _currentWeapon.Equip(_weaponRenderer, OnShoot);
        }
        private void Start()
        {
            _onUnitReloadStart += _unitUI.OpenUI;
            _onUnitReloadFinish += _unitUI.CloseUI;
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
            StopCoroutine(_reloadRoutine);
            _reloadRoutine = null;
            _currentWeapon.ReloadTimer = 0;
            _currentWeapon.Load();
            _failedReload = false;
            _onUnitReloadFinish?.Invoke(this, EventArgs.Empty);
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

        //Call an Weapon.Aim() to have specific implementation (exampl. Scope view(Inverse Mask))
        public override void Aim(Vector2 target)
        {
            Vector2 targetDirection = target - (Vector2)_weaponRenderer.transform.position;
            float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
            _weaponRenderer.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        public void Reload()
        {
            if (_currentWeapon.AmmoCount > 0)
            {
                _currentWeapon.Eject();
                return;
            }
            if (_currentWeapon.BulletAmount <= 0)
            {
                return;
            }
            if (_reloadRoutine == null)
            {
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
                Debug.Log("Equipped: " + _currentWeapon.name);
            }
        }
        
        public void TakeAction(Vector2 target, Type actionType)
        {
            _selectedAction = _actionDictionary[actionType];
            _selectedAction.SetAction(target);
        }
        public List<Vector2> Preview()
        {
            return _selectedAction.GetPreview();
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
            StopReload();
            _weaponRenderer.sprite = null;
            _currentWeapon = null;
        }
    }
}

