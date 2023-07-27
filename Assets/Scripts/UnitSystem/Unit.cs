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
    
    //Unit UI
    [SerializeField] private Transform _unitUI;
    [SerializeField] private SpriteRenderer _unitSprite;

    private ActionState _actionState;
    private bool _isExecuting;
    private bool _isFollowing;
    private bool _isReloading;
    [SerializeField] private bool _isEnemy;
    private Pathfinding _pathfinding;
    private List<Vector2> _actionResults;
    private Coroutine _reloadRoutine;

    private Slider _reloadBar;
    //private event UnityAction SelectedActionChanged = delegate {  };
    
    public EventHandler<UnitMovedEventArgs> OnUnitMove
    {
        get => _onUnitMove;
        set => _onUnitMove = value;
    }
    public EventHandler OnUnitShoot
    {
        get => _onUnitShoot;
        set => _onUnitShoot = value;
    }
    public bool isExecuting
    {
        get { return _isExecuting; }
    }
    public bool isFollowing
    {
        get { return _isFollowing; }
        set { _isFollowing = value; }
    }
    public bool isEnemy
    {
        get { return _isEnemy; }
    }
    public SpriteRenderer unitSprite
    {
        get { return _unitSprite;  }
    }
    public Pathfinding pathfinding
    {
        get { return _pathfinding; }
    }
    public Weapon weapon
    {
        get { return _currentWeapon; }
    }

    private void Awake()
    {
        _currentWeapon = _currentWeapon.Equip(_weaponSprite.transform, OnShoot);
        _weaponSprite.sprite = _currentWeapon.GetSprite();
        
        _reloadBar = _unitUI.GetComponentInChildren<Slider>();
    }
    private void Start()
    {
        _onUnitMove?.Invoke(this, 
            new UnitMovedEventArgs(this, transform.position,transform.position));
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
            StopCoroutine(_reloadRoutine);
            _reloadRoutine = null;
            _currentWeapon.ReloadTimer = 0;
            _currentWeapon.Load();
            CloseUI();
        }
        else
        {
            _reloadBar.value = _currentWeapon.ReloadTimer;
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

    //Need to move this
    private void CreateActionUI()
    {
        if (_unitUI.TryGetComponent(out UIController uiController))
        {
            uiController.CreateButtons(_actionDictionary, action =>
            {
                _selectedAction.UnsetAction();
                _selectedAction = action; 
                Debug.Log(action.GetType() + " : " + _selectedAction.GetType());
                //_selectedAction.SetAction(OnActionComplete);
                //SelectedActionChanged.Invoke();
            });
        }
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
        //CreateActionUI();

        _selectedAction = _actionDictionary[typeof(MoveAction)];
        _selectedAction.SetAction(Vector2.zero);
        //SelectedActionChanged += UpdateAction;
    }

    //Call an Weapon.Aim() to have specific implementation (exampl. Scope view(Inverse Mask))
    public void Aim(Vector2 target)
    {
        Vector2 targetDirection = target - (Vector2)_weaponSprite.transform.position;
        float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
        _weaponSprite.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        
        //Do Weapon.Aim
        
        
        //_weaponSprite.transform.rotation = Quaternion.LookRotation(Vector3.forward, target - _weaponSprite.transform.position);
    }

    public void Reload()
    {
        if (_currentWeapon.AmmoCount > 0)
        {
            _currentWeapon.Eject();
            return;
        }

        OpenUI();
        
        if (_reloadRoutine == null)
        {
            _reloadRoutine = StartCoroutine(_currentWeapon.ReloadRoutine());
        }

        if (_currentWeapon.ReloadTimer >= .6f && _currentWeapon.ReloadTimer <= .8f)
        {
            _currentWeapon.ReloadTimer = 0;
            StopCoroutine(_reloadRoutine);
            _reloadRoutine = null;
            
            _currentWeapon.Load();
            CloseUI();
        }
        //Play reload animation
        //Get reload variables;

        //Do reload thing
        //weapon.Reload();
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
    
    public void UpdateAction()
    {
        _selectedAction.SetAction(Vector2.zero);
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

    //Need to move this to UI Class
    public void OpenUI()
    {
        _unitUI.gameObject.SetActive(true);
    }
    
    public void CloseUI()
    {
        _unitUI.gameObject.SetActive(false);
    }
    
}
