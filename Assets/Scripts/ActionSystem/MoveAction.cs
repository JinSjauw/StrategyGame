using System;
using System.Collections.Generic;
using System.Linq;
using ActionSystem;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "Actions/MoveAction")]
public class MoveAction : BaseAction
{
    private List<Vector2> _path;
    private int _pathIndex;
    private int _pathLength;

    private SpriteRenderer _animTarget;
    [SerializeField] private AnimationCurve movementCurve;
    [SerializeField] private AnimationCurve bounceCurve;
    [SerializeField] private AnimationCurve rotationCurve;
    
    private float _current = 0;
    private Vector2 _origin;
    private Vector2 _destination;
    private Vector2 _target;
    private Vector2 _direction;
    private bool _isFollowing;
    
    private void PlayMoveAnimation(float evaluator)
    {
        if (evaluator < 0.1f)
        {
            evaluator = 0;
        }
        //Bounce
        _animTarget.transform.parent.localPosition = new Vector2(0, bounceCurve.Evaluate(evaluator));
        //Wobble
        _animTarget.transform.parent.localRotation = Quaternion.Euler(0,0,rotationCurve.Evaluate(evaluator) * 5f);
    }

    private void OnUnitMoved(Vector2 origin, Vector2 destination)
    {
        UnitMovedEventArgs unitMovedEvent = new UnitMovedEventArgs(holderUnit, origin, destination);
        holderUnit.OnUnitMove?.Invoke(holderUnit, unitMovedEvent);
        //Debug.Log("Unit Moved!");
    }

    private void GetPath()
    {
        _origin = holderUnit.transform.position;
        
        _path = holderUnit.pathfinding.FindPath(_origin, _target, _isFollowing);
        if (_path.Count > 0)
        {
            _path.Remove(_path.First());
        }
        
        _pathIndex = 0;
        _pathLength = _isFollowing && _path.Count > 1 ? _path.Count - 1 : _path.Count;
    }
    
    public override void Initialize(Unit unit, Action onComplete)
    {
        base.Initialize(unit, onComplete);
        _animTarget = unit.unitSprite;
    }

    public override void SetAction(Vector2 target)
    {
        //inputReader.MouseMoveStartEvent += OnInput;
        //Debug.Log("Target in Action: " + target);
        _isFollowing = holderUnit.isFollowing;
        _target = target;
        _origin = holderUnit.transform.position;
        _current = 0;
        
        holderUnit.FlipSprite(_target);
        
        GetPath();
    }

    public override List<Vector2> GetPreview()
    {
        return _path;
    }

    public override void Execute()
    {
        //Moving
        if (_path.Count < 1)
        {
            _onComplete();
            _path.Clear();
        }
        
        if (_pathIndex < _pathLength && _path.Count >= 1)
        {
            _destination = _path[_pathIndex];
            _current = Mathf.MoveTowards(_current, 1, unitData.moveSpeed * Time.deltaTime);
            if (_current < 1f)
            {
                holderUnit.transform.position = Vector2.Lerp(_origin, _destination, movementCurve.Evaluate(_current));
                //_direction = _destination - _origin;
                PlayMoveAnimation(_current);
            }
            else if(_current >= 1f)
            {
                OnUnitMoved(_origin, _destination);
                _pathIndex++;
                _current = 0;
                _origin = _destination;
            }
        } 
        else
        {
            _onComplete();
            _path.Clear();
            _pathIndex = 1;
            PlayMoveAnimation(0);
        }
    }
}
