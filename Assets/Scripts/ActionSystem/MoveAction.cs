using System.Collections;
using System.Collections.Generic;
using ActionSystem;
using UnityEngine;

[CreateAssetMenu(menuName = "Actions/MoveAction")]
public class MoveAction : BaseAction
{
    private List<Vector2> _path;
    private int _pathIndex;

    private float moveSpeed = 5;

    public void SetPath(List<Vector2> path)
    {
        _path = path;
        _pathIndex = 1;
        ActionStarted();
    }
    
    //Should Run in the update
    public override ActionState Execute()
    {
        //Debug.Log("EXECUTING MOVEACTION " + Unit.name);
        //Moving
        if (_pathIndex < _path.Count)
        {
            Transform unitTransform = Unit.transform;
            Vector2 unitPosition = unitTransform.position;
            Vector2 destination = _path[_pathIndex];
            
            float distance = Vector2.Distance(unitPosition, destination);
            //Debug.Log(distance);
            if (distance > 0.01f)
            {
                Vector2 direction = new Vector2(destination.x - unitPosition.x, destination.y - unitPosition.y);
                //Debug.Log(direction);
                //Debug.Log(moveSpeed);
                Vector2 newPosition = direction.normalized * moveSpeed;
                unitTransform.position += (Vector3)newPosition * Time.deltaTime;
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
