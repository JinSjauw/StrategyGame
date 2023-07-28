using System;
using System.Collections;
using System.Collections.Generic;
using ActionSystem;
using UnityEngine;
using UnityEngine.UI;

public class Unit : MonoBehaviour
{
    //Unit Stats --> Pass onto the Action Instance
    [SerializeField] private UnitData _unitData;
    //Unit Loadout --> Pass onto the Action Instance
    //WeaponData field
    [SerializeField] private Weapon _currentWeapon;
    [SerializeField] private SpriteRenderer _weaponSprite;
    
    //Temp --> Action Collection
    [SerializeField] private List<BaseAction> _actions;
    //Put it into a dictionary
    private Dictionary<Type, BaseAction> _actionDictionary;
    private BaseAction _selectedAction;
    
    //Unit Events
    private event EventHandler<UnitMovedEventArgs> _onUnitMove;
    private event EventHandler _onUnitShoot;
    private event EventHandler _onUnitReloadStart;
    private event EventHandler _onUnitReloadFinish;

    //Unit UI
    [SerializeField] private UIController _unitUI;
    [SerializeField] private SpriteRenderer _unitSprite;

    private ActionState _actionState;
    private bool _isExecuting;
    private bool _isFollowing;
    private bool _isReloading;
    [SerializeField] private bool _isEnemy;
    private Pathfinding _pathfinding;
    private List<Vector2> _actionResults;
    
    private Coroutine _reloadRoutine;
    [SerializeField] private bool _failedReload;
    private Slider _reloadBar;

    public EventHandler<UnitMovedEventArgs> OnUnitMove { get => _onUnitMove; set => _onUnitMove = value; }
    public EventHandler OnUnitShoot { get => _onUnitShoot; set => _onUnitShoot = value; }
    public bool isExecuting { get => _isExecuting; }
    public bool isFollowing { get => _isFollowing; set => _isFollowing = value; }
    public bool isEnemy { get => _isEnemy; }
    public SpriteRenderer unitSprite { get => _unitSprite; }
    public Pathfinding pathfinding { get => _pathfinding; }
    public Weapon weapon { get => _currentWeapon; }

    private void Awake()
    {
        _currentWeapon = _currentWeapon.Equip(_weaponSprite.transform, OnShoot);
        _weaponSprite.sprite = _currentWeapon.GetSprite();
        Debug.Log(_unitUI.name);
    }
    private void Start()
    {
        _onUnitMove?.Invoke(this, 
            new UnitMovedEventArgs(this, transform.position,transform.position));
        _onUnitReloadStart += _unitUI.OpenUI;
        _onUnitReloadFinish += _unitUI.CloseUI;
    }
    private void Update()
    {
        if (_isExecuting)
        {
            _selectedAction.Execute();
        }
        if (isEnemy)
        {
            return;
        }
        if (_currentWeapon.ReloadTimer >= 1)
        {
            FinishReload();
        }
        else
        {
            _unitUI.ReloadBar.value = _currentWeapon.ReloadTimer;
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

    private void FinishReload()
    {
        StopCoroutine(_reloadRoutine);
        _reloadRoutine = null;
        _currentWeapon.ReloadTimer = 0;
        _currentWeapon.Load();
        _failedReload = false;
        _onUnitReloadFinish?.Invoke(this, EventArgs.Empty);
    }
    
    public void Initialize(Pathfinding pathfinding)
    {
        _pathfinding = pathfinding;
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
    public void Aim(Vector2 target)
    {
        Vector2 targetDirection = target - (Vector2)_weaponSprite.transform.position;
        float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
        _weaponSprite.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
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

    public void FlipSprite(Vector2 target)
    {
        Vector2 weaponHolderPosition = _weaponSprite.transform.localPosition;
        float distance = Mathf.Abs(target.x - _weaponSprite.transform.position.x);
        if (target.x < _weaponSprite.transform.position.x && distance > .1f)
        {
            _weaponSprite.flipY = true;
            _unitSprite.flipX = true;
        }
        else
        {
            _weaponSprite.flipY = false;
            _unitSprite.flipX = false;
        }
        
        if (_unitSprite.flipX)
        {
            weaponHolderPosition.x = -0.1f;
        }
        else
        {
            weaponHolderPosition.x = 0.1f;
        }
        _weaponSprite.transform.localPosition = weaponHolderPosition;
    }
    
    public void TakeAction(Vector2 target, Type actionType)
    {
        _selectedAction = _actionDictionary[actionType];
        _selectedAction.SetAction(target);
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
    public UnitData GetUnitStats()
    {
        return _unitData;
    }
}
