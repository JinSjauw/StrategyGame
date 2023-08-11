using System;
using System.Collections;
using System.Collections.Generic;
using Items;
using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    //[SerializeField] private float velocity;

    //Have a bullet config SO so it can have different characteristics when Object Pooling by reassinging bullet SO!!!

    [SerializeField] private Bullet bullet;

    private SpriteRenderer _bulletSprite;
    
    private Vector2 _direction;
    private Vector2 _lastPosition;
    private Vector2 _currentPosition;

    private bool _hasHit = false;
    private bool _ignoreCover;

    private Collider2D _ignoreCoverObject;
    
    private void Awake()
    {
        _bulletSprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (_hasHit)
        {
            //Play impact effect
            //Destroy after finishing
            return;
        }
        
        if (!DetectCollision())
        {
            _lastPosition = transform.position;
            transform.position += (Vector3)_direction * bullet.velocity * Time.deltaTime;
            _currentPosition = transform.position;
        }
        else
        {
            //Stop projectile halfway up the collider **Because perspective**
            //transform.position = bullet.Impact() + _direction * .3f;
            _hasHit = true;
            Destroy(gameObject);
        }
    }

    private bool DetectCollision()
    {
        //Raycast between last and current position
        //Check for hit in between.
        //Pass Action delegate for bullet SO to decide when its enough.
        //Run Bullet SO Detect(), Check for bool return;
        //Debug.Log(_lastPosition + " " + _currentPosition);
        return bullet.DetectCollision(_currentPosition, _lastPosition, _ignoreCoverObject, _ignoreCover);
    }

    public void SetBullet(Bullet bullet)
    {
        this.bullet = bullet.Copy();
        //_bulletSprite.sprite = _bulletConfig.bulletSprite;
        _bulletSprite.color = this.bullet.colorTest;
    }
    
    public void Fire(Vector3 direction, bool ignore = false)
    {
        _ignoreCover = ignore;
        _lastPosition = transform.position;
        _currentPosition = transform.position;
        transform.up = direction;
        _direction = direction;
    }

    public void IgnoreCollider(Collider2D coverHitCollider)
    {
        _ignoreCoverObject = coverHitCollider;
    }
}
