using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/BulletConfig")]
public class BulletConfig : ScriptableObject
{
    //SFX & VFX variables
    
    //Penetration Value
    //Damage;

    private RaycastHit2D _hitPoint;
    
    public bool DetectCollision(Vector2 currentPosition, Vector2 lastPosition, bool ignore = false)
    {
        //Add combined layerMask
        _hitPoint = Physics2D.Linecast(lastPosition, currentPosition);

        if (!_hitPoint.collider)
        {
            return false;
        }
        
        if (_hitPoint.collider.CompareTag("HalfCover") && ignore)
        {
            Debug.Log("Skipped HalfCover");
            return false;
        }
        
        if (_hitPoint.collider.CompareTag("Obstacles") || _hitPoint.collider.CompareTag("HalfCover") || _hitPoint.collider.CompareTag("Player"))
        {
            Debug.Log("Hit Tag: " + _hitPoint.collider.tag);
            return true;
        }
        
        return false;
    }

    public Vector2 Impact()
    {
        Debug.Log("Playing Impact @: " + _hitPoint.point);
        return _hitPoint.point;
    }
}
