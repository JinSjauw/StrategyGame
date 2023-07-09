using System;
using System.Collections;
using System.Collections.Generic;
using ActionSystem;
using Unity.Mathematics;
using Unity.VisualScripting;
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

    private void OnActionSet()
    {
        
    }
    
    public override void Initialize(Unit unit)
    {
        base.Initialize(unit);
        _animTarget = unit.Sprite;
    }

    public override void SetAction(Vector2 target)
    {
        _isFollowing = holderUnit.isFollowing;

        _path = holderUnit.pathfinding.FindPath(holderUnit.transform.position, target, _isFollowing);

        _pathIndex = 1;
        _pathLength = _isFollowing && _path.Count > 1 ? _path.Count - 1 : _path.Count;

        _origin = holderUnit.transform.position;
        _direction = target - (Vector2)holderUnit.transform.position;
        _animTarget.flipX = _direction.normalized.x < 0;

        ActionStarted();
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
                PlayMoveAnimation(_current);
            }
            else if(_current >= 1f)
            {
                OnUnitMoved(_origin, _destination);
                _pathIndex++;
                _current = 0;
                _origin = _destination;
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
