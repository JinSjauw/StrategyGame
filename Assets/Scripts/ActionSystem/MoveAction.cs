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

    private SpriteRenderer _animTarget;
    [SerializeField] private AnimationCurve movementCurve;
    [SerializeField] private AnimationCurve bounceCurve;
    [SerializeField] private AnimationCurve rotationCurve;
    
    private float _current = 0;
    private Vector2 _origin;
    private Vector2 _destination;
    private Vector2 _direction;

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
        UnitMovedEventArgs unitMovedEvent = new UnitMovedEventArgs(Unit, origin, destination);
        Unit.OnUnitMove?.Invoke(Unit, unitMovedEvent);
    }
    
    public override void Initialize(Unit unit)
    {
        base.Initialize(unit);
        _animTarget = unit.Sprite;
        
    }
    //HMM
    public void SetPath(List<Vector2> path)
    {
        if (path.Count <= 0)
        {
            return;
        }
        
        _path = path;
        _pathIndex = 1;
        
        _origin = Unit.transform.position;
        _direction = _path[path.Count - 1] - (Vector2)Unit.transform.position;
        _animTarget.flipX = _direction.normalized.x < 0;
        
        ActionStarted();
    }
    
    public override ActionState Execute()
    {
        //Moving
        if (_pathIndex < _path.Count)
        {
            _destination = _path[_pathIndex];
            _current = Mathf.MoveTowards(_current, 1, unitData.moveSpeed * Time.deltaTime);

            if (_current < 1f)
            {
                Unit.transform.position = Vector2.Lerp(_origin, _destination, movementCurve.Evaluate(_current));
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

        return State;
    }
}
