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

    [SerializeField] private MoveAction _moveAction;
    private BaseAction _selectedAction;
    
    //Unit Events
    private event EventHandler<UnitMovedEventArgs> _onUnitMove;
    
    //Unit UI
    [SerializeField] private Transform _unitUI;

    private ActionState _actionState;
    private bool _isExecuting;
    private bool _isFollowing;
    private SpriteRenderer _sprite;
    private Pathfinding _pathfinding;

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
    public ActionState ActionState
    {
        get { return _actionState; }
    }
    public SpriteRenderer Sprite
    {
        get { return _sprite;  }
    }
    public Pathfinding pathfinding
    {
        get { return _pathfinding; }
    }

    private void Awake()
    {
        
    }

    private void Start()
    {
        _onUnitMove?.Invoke(this, new UnitMovedEventArgs(this, transform.position, transform.position));
    }

    private void Update()
    {
        if (_isExecuting)
        {
            if (_actionState != ActionState.Completed)
            {
                //Temp --> selectedAction.Execute();
                _actionState = _selectedAction.Execute();
            }
            else
            {
                _isExecuting = false;
            }
        }
    }

    private void CreateActionUI()
    {
        if (_unitUI.TryGetComponent(out UIController uiController))
        {
            uiController.CreateButtons(_actions, action =>
            {
                _selectedAction = action;
                Debug.Log(action.GetType());
            });
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
        
        /*_actionDictionary = new Dictionary<Type, BaseAction>();
        foreach (BaseAction action in _actions)
        {
            if (!_actionDictionary.ContainsKey(action.GetType()))
            {
                _actionDictionary[action.GetType()] = action;
            }
        }*/

        CreateActionUI();
    }
    
    public void SetAction(Vector2 target)
    {
        //Initialize the variables
        _selectedAction.SetAction(target);
    }
    
    public void SetAction(Type actionType, Vector2 target)
    {
        //Initialize the variables
        _selectedAction = _actionDictionary[actionType];
        _selectedAction.SetAction(target);
    }

    public void StartAction()
    {
        _actionState = ActionState.Started;
        _isExecuting = true;
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
