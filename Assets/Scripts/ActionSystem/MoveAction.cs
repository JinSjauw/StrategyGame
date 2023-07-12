using System;
using System.Collections.Generic;
using ActionSystem;
using UnityEngine;

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
    private Vector2 _direction;
    private bool _isFollowing;
    private void PlayMoveAnimation(float evaluator)
    {
        if (evaluator < 0.1f)
        {
            evaluator = 0;
        }
        //Bounce
        _animTarget.transform.localPosition = new Vector2(0, bounceCurve.Evaluate(evaluator));
        //Wobble
        _animTarget.transform.localRotation = Quaternion.Euler(0,0,rotationCurve.Evaluate(evaluator) * 5f);
    }

    private void OnUnitMoved(Vector2 origin, Vector2 destination)
    {
        UnitMovedEventArgs unitMovedEvent = new UnitMovedEventArgs(holderUnit, origin, destination);
        holderUnit.OnUnitMove?.Invoke(holderUnit, unitMovedEvent);
    }

    private List<Vector2> OnActionSet()
    {
        //Move Action
        //Return path list
        //Targeted Ability
        //All targets / Vector2's || UNITS
        //Return List Vector2's
        return new List<Vector2>();
    }
    
    public override void Initialize(Unit unit)
    {
        base.Initialize(unit);
        _animTarget = unit.sprite;
    }

    public override List<Vector2> SetAction(Vector2 target)
    {
        _isFollowing = holderUnit.isFollowing;
        _origin = holderUnit.transform.position;
        _path = holderUnit.pathfinding.FindPath(_origin, target, _isFollowing);

        _pathIndex = 1;
        _pathLength = _isFollowing && _path.Count > 1 ? _path.Count - 1 : _path.Count;

        ActionStarted();

        return _path;
    }

    public override ActionState Execute()
    {
        //Moving
        if (_pathIndex < _pathLength && _path.Count > 1)
        {
            _destination = _path[_pathIndex];
            _current = Mathf.MoveTowards(_current, 1, unitData.moveSpeed * Time.deltaTime);
            
            if (_current < 1f)
            {
                holderUnit.transform.position = Vector2.Lerp(_origin, _destination, movementCurve.Evaluate(_current));
                _direction = _destination - _origin;
                _animTarget.flipX = _direction.normalized.x < 0;
                PlayMoveAnimation(_current);
            }
            else if(_current >= 1f)
            {
                _pathIndex++;
                _current = 0;
                _origin = _destination;

                if (_pathIndex < _path.Count)
                {
                    OnUnitMoved(_origin, _path[_pathIndex]);
                }
            }
        } else
        {
            ActionComplete();
            _pathIndex = 1;
            PlayMoveAnimation(0);
        }

        return state;
    }
}
