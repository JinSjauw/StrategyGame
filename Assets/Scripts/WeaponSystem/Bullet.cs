using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    //[SerializeField] private float velocity;

    //Have a bullet config SO so it can have different characteristics when Object Pooling by reassinging bullet SO!!!

    [SerializeField] private BulletConfig _bulletConfig;
    
    private Vector2 _direction;
    private Vector2 _lastPosition;
    private Vector2 _currentPosition;

    private bool _hasHit = false;
    private bool _ignoreCover;
    
    // Update is called once per frame
    private void Update()
    {
        if (_hasHit)
        {
            return;
        }
        
        if (!DetectCollision())
        {
            _lastPosition = transform.position;
            transform.position += (Vector3)_direction * _bulletConfig.velocity * Time.deltaTime;
            _currentPosition = transform.position;
        }
        else
        {
            //Stop projectile halfway up the collider **Because perspective**
            transform.position = _bulletConfig.Impact() + _direction * .3f;
            _hasHit = true;
        }
    }

    private bool DetectCollision()
    {
        //Raycast between last and current position
        //Check for hit in between.
        //Pass Action delegate for bullet SO to decide when its enough.
        //Run Bullet SO Detect(), Check for bool return;
        //Debug.Log(_lastPosition + " " + _currentPosition);
        return _bulletConfig.DetectCollision(_currentPosition, _lastPosition, _ignoreCover);
    }
    
    public void Fire(Vector3 direction, bool ignore = false)
    {
        _ignoreCover = ignore;
        _lastPosition = transform.position;
        _currentPosition = transform.position;
        transform.up = direction;
        _direction = direction;
    }
}
