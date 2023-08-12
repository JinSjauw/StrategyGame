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
    private RaycastHit2D _hitPoint;

    private DebrisDispenser _debrisDispenser;
    
    private void Awake()
    {
        _bulletSprite = GetComponent<SpriteRenderer>();
        _debrisDispenser = GetComponent<DebrisDispenser>();
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
        _hitPoint = Physics2D.Linecast(_lastPosition, _currentPosition);
        if (!_hitPoint.collider)
        {
            return false;
        }
        if (_ignoreCoverObject != null && _hitPoint.collider == _ignoreCoverObject)
        {
            return false;
        }
        if (_hitPoint.collider.CompareTag("HalfCover") && _ignoreCover)
        {
            Debug.Log("Skipped HalfCover");
            return false;
        }
        if (_hitPoint.collider.CompareTag("Obstacles") || _hitPoint.collider.CompareTag("HalfCover") && !_ignoreCover)
        {
            Debug.Log("Hit Tag: " + _hitPoint.collider.tag);
            //Dispense Impact effect
            _debrisDispenser.DispenseDebris(-(_currentPosition - _lastPosition));
            return true;
        }
        if (_hitPoint.collider.CompareTag("UnitHead") || _hitPoint.collider.CompareTag("UnitBody"))
        {
            Debug.Log("HIT UNIT: " + _hitPoint.collider.name);
            IDamageable hitUnit = _hitPoint.collider.GetComponentInParent<IDamageable>();
            hitUnit.TakeDamage(bullet.damage);
            hitUnit.SpawnDebris(_lastPosition - _currentPosition, _currentPosition);
            return true;
        }

        return false;
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
