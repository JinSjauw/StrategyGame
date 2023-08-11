using System;
using System.Collections.Generic;
using System.Linq;
using ActionSystem;
using UnitSystem;
using UnityEngine;

[CreateAssetMenu(menuName = "Actions/MoveAction")]
public class MoveAction : BaseAction
{
    private List<TileGridObject> _pathPreview = new List<TileGridObject>();
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
        TileGridObject tileGridObject = holderUnit.levelGrid.GetTileGridObject(destination);
        tileGridObject.m_TileVisual.TurnHighlightOff();
    }

    private void GetPath()
    {
        _origin = holderUnit.transform.position;
        
        _path = holderUnit.pathfinding.FindPath(_origin, _target, true);
        
        if (_path.Count > 0)
        {
            _path.Remove(_path.First());
        }
        
        _pathIndex = 0;
        //_pathLength = _path.Count > 1 ? _path.Count : _path.Count;
        _pathLength = _path.Count;
    }
    
    public override void Initialize(PlayerUnit unit, Action onComplete)
    {
        base.Initialize(unit, onComplete);
        _animTarget = unit.unitRenderer;
    }

    public override void SetAction(Vector2 target)
    {
        if (holderUnit == null) { return; }
        
        _target = target;
        _origin = holderUnit.transform.position;
        _current = 0;
        
        holderUnit.FlipSprite(_target);
        
        GetPath();
    }

    public override void Preview()
    {
        for (int i = 0; i < _pathPreview.Count; i++)
        {
            _pathPreview[i].m_TileVisual.TurnHighlightOff();
        }
        
        _pathPreview.Clear();
        
        //_path = holderUnit.pathfinding.FindPath(_origin, _target, true);
        //Debug.Log($"In MoveAction {_path.Count} O: {_origin} T: {_target}");
        for (int i = 0; i < _path.Count; i++)
        {
            TileGridObject tileGridObject = holderUnit.levelGrid.GetTileGridObject(_path[i]);
            tileGridObject.m_TileVisual.TurnHighlightOn();
            _pathPreview.Add(tileGridObject);
        }
    }

    public override void StopPreview()
    {
        for (int i = 0; i < _path.Count; i++)
        {
            TileGridObject tileGridObject = holderUnit.levelGrid.GetTileGridObject(_path[i]);
            tileGridObject.m_TileVisual.TurnHighlightOff();
        }
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
