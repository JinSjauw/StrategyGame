using System;
using System.Collections.Generic;
using ActionSystem;
using UnityEngine;
using UnityEngine.Events;

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
    
    //Unit UI
    [SerializeField] private Transform _unitUI;
    [SerializeField] private SpriteRenderer _playerSprite;

    private ActionState _actionState;
    private bool _isExecuting;
    private bool _isFollowing;
    [SerializeField] private bool _isEnemy;
    private Pathfinding _pathfinding;
    private List<Vector2> _actionResults;
    
    //private event UnityAction SelectedActionChanged = delegate {  };
    
    public EventHandler<UnitMovedEventArgs> OnUnitMove
    {
        get => _onUnitMove;
        set => _onUnitMove = value;
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
    
    public SpriteRenderer playerSprite
    {
        get { return _playerSprite;  }
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
        _currentWeapon = _currentWeapon.Equip(_weaponSprite.transform);
        _weaponSprite.sprite = _currentWeapon.GetSprite();
        
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
    }
    private void OnActionComplete()
    {
        _isExecuting = false;
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

    public void Aim(Vector3 target)
    {
        Vector3 weaponHolderPosition = _weaponSprite.transform.localPosition;

        if (_playerSprite.flipX)
        {
            weaponHolderPosition.x = -0.1f;
        }
        else
        {
            weaponHolderPosition.x = 0.1f;
        }

        _weaponSprite.transform.localPosition = weaponHolderPosition;


        if (target.x < _weaponSprite.transform.position.x)
        {
            _weaponSprite.flipY = true;
            _playerSprite.flipX = true;
        }
        else
        {
            _weaponSprite.flipY = false;
            _playerSprite.flipX = false;
        }
       
        
        
        Vector2 targetDirection = target - _weaponSprite.transform.position;
        float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
        _weaponSprite.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        
        //_weaponSprite.transform.rotation = Quaternion.LookRotation(Vector3.forward, target - _weaponSprite.transform.position);
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
