using System;
using System.Collections.Generic;
using ActionSystem;
using UnityEngine;

public class Unit : MonoBehaviour
{
    //Unit Stats --> Pass onto the Action Instance
    [SerializeField] private UnitData _unitData;
    //Unit Loadout --> Pass onto the Action Instance
    //WeaponData field
    
    //Temp --> Action Collection
    [SerializeField] private List<BaseAction> _actions;
    //Put it into a dictionary
    private Dictionary<Type, BaseAction> _actionDictionary;
    [SerializeField] private BaseAction _selectedAction;
    
    //Unit Events
    private event EventHandler<UnitMovedEventArgs> _onUnitMove;
    
    //Unit UI
    [SerializeField] private Transform _unitUI;

    private ActionState _actionState;
    private bool _isExecuting;
    private bool _isFollowing;
    private SpriteRenderer _sprite;
    private Pathfinding _pathfinding;
    private List<Vector2> _actionResults;

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
    public ActionState actionState
    {
        get { return _actionState; }
    }
    public SpriteRenderer sprite
    {
        get { return _sprite;  }
    }
    public Pathfinding pathfinding
    {
        get { return _pathfinding; }
    }

    private void Start()
    {
        _onUnitMove?.Invoke(this, new UnitMovedEventArgs(this, transform.position, transform.position));
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
    
    private void CreateActionUI()
    {
        if (_unitUI.TryGetComponent(out UIController uiController))
        {
            uiController.CreateButtons(_actionDictionary, action => { _selectedAction = action; });
        }
    }

    public void Initialize(Pathfinding pathfinding)
    {
        _pathfinding = pathfinding;
        _sprite = GetComponentInChildren<SpriteRenderer>();
        _actionDictionary = new Dictionary<Type, BaseAction>();
        
        for (int i = 0; i < _actions.Count; i++)
        {
            BaseAction action = Instantiate(_actions[i]);
            action.Initialize(this);
            _actionDictionary[action.GetType()] = action;
        }
        CreateActionUI();
    }
    
    public void SetAction(Vector2 target)
    {
        //Initialize the variables
        _actionResults = _selectedAction.SetAction(target, OnActionComplete);
    }
    
    public void SetAction(Type actionType, Vector2 target)
    {
        //Initialize the variables
        _selectedAction = _actionDictionary[actionType];
        _actionResults = _selectedAction.SetAction(target, OnActionComplete);
    }

    public List<Vector2> PreviewAction(out Type actionType)
    {
        actionType = _selectedAction.GetType();
        return _actionResults;
    }
    
    public void StartAction()
    {
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

    public void OpenUI()
    {
        _unitUI.gameObject.SetActive(true);
    }
    
    public void CloseUI()
    {
        _unitUI.gameObject.SetActive(false);
    }
}
