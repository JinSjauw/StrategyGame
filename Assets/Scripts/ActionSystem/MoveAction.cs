using System.Collections;
using System.Collections.Generic;
using ActionSystem;
using UnityEngine;

[CreateAssetMenu(menuName = "Actions/MoveAction")]
public class MoveAction : BaseAction
{
    private List<Vector2> _path;
    private int _pathIndex;

    public void SetPath(List<Vector2> path)
    {
        _path = path;
        _pathIndex = 1;
        ActionStarted();
    }
    //Should Run in the update
    public override ActionState Execute()
    {
        //Moving
        if (_pathIndex < _path.Count)
        {
            Transform unitTransform = Unit.transform;
            Vector2 unitPosition = unitTransform.position;
            Vector2 destination = _path[_pathIndex];
            
            float distance = Vector2.Distance(unitPosition, destination);
            if (distance > 0.01f)
            {
                Vector2 direction = new Vector2(destination.x - unitPosition.x, destination.y - unitPosition.y);
                Vector2 newPosition = direction.normalized * UnitStats.moveSpeed;
                unitPosition += newPosition * Time.deltaTime;

                bool passedPoint = Vector2.Dot(
                        ((Vector2)unitTransform.position - unitPosition).normalized, 
                        (destination - unitPosition).normalized) > 0;
                if (passedPoint)
                {
                    unitTransform.position = destination;
                }
                else
                {
                    unitTransform.position = unitPosition;
                }
            }
            else if (distance <= 0.01f)
            {
                _pathIndex++;
            }
        }
        else
        {
            ActionComplete();
            _pathIndex = 1;
        }
        
        return State;
    }
}
