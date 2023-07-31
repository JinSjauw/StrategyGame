using System;
using System.Collections;
using System.Collections.Generic;
using AI.Awareness;
using UnityEngine;
using UnityEngine.Animations;

namespace UnitSystem
{
    public abstract class Unit : MonoBehaviour
    {
        [SerializeField] protected Weapon _currentWeapon;
        [SerializeField] protected SpriteRenderer _unitSprite;
        [SerializeField] protected SpriteRenderer _weaponSprite;
        [SerializeField] protected UnitData _unitData;
        [SerializeField] protected TargetType _targetType;

        protected LevelGrid _levelGrid;
        protected Pathfinding _pathfinding;

        protected event EventHandler<UnitMovedEventArgs> _onUnitMove;
        
        public SpriteRenderer unitSprite { get => _unitSprite; }
        public Pathfinding pathfinding { get => _pathfinding; }
        public Weapon weapon { get => _currentWeapon; }
        public UnitData unitData { get => _unitData; }
        public TargetType targetType { get => _targetType; }
        public EventHandler<UnitMovedEventArgs> OnUnitMove { get => _onUnitMove; set => _onUnitMove = value; }

        public virtual void Initialize(LevelGrid levelGrid)
        {
            _levelGrid = levelGrid;
            _onUnitMove?.Invoke(this, 
                new UnitMovedEventArgs(this, transform.position,transform.position));
        }
        public virtual void Aim(Vector2 target){}
        public virtual void StopAim()
        {
            _weaponSprite.transform.rotation = new Quaternion(0, 0, 0, 0);
            if (unitSprite.flipX)
            {
                _weaponSprite.flipX = true;
                _weaponSprite.flipY = false;
            }
        }
        
        public void FlipSprite(Vector2 target)
        {
            Vector2 weaponHolderPosition = _weaponSprite.transform.localPosition;
            float distance = Mathf.Abs(target.x - transform.position.x);
            if (target.x < _weaponSprite.transform.position.x && distance > .1f)
            {
                if (_weaponSprite.transform.localRotation.eulerAngles.z == 0)
                {
                    _weaponSprite.flipX = true;
                    _weaponSprite.flipY = false;
                }
                else
                {
                    _weaponSprite.flipY = true;
                    _weaponSprite.flipX = false;
                }
                _unitSprite.flipX = true;
            }
            else
            {
                _weaponSprite.flipX = false;
                _weaponSprite.flipY = false;
                _unitSprite.flipX = false;
            }
                
            if (_unitSprite.flipX)
            {
                weaponHolderPosition.x = -0.1f;
            }
            else
            {
                weaponHolderPosition.x = 0.1f;
            }
            _weaponSprite.transform.localPosition = weaponHolderPosition;
        }
        public UnitData GetUnitData() { return _unitData; }
    }
}


