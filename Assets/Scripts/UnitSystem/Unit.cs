using System;
using System.Collections;
using System.Collections.Generic;
using AI.Awareness;
using Items;
using UnityEngine;
using UnityEngine.Animations;

namespace UnitSystem
{
    public abstract class Unit : MonoBehaviour
    {
        [SerializeField] protected Weapon _currentWeapon;
        [SerializeField] protected SpriteRenderer _unitRenderer;
        [SerializeField] protected SpriteRenderer _weaponRenderer;
        [SerializeField] protected SpriteRenderer _bodyArmorRenderer;
        [SerializeField] protected SpriteRenderer _headgearRenderer;
        [SerializeField] protected UnitData _unitData;
        [SerializeField] protected TargetType _targetType;

        protected LevelGrid _levelGrid;
        protected Pathfinding _pathfinding;

        protected event EventHandler<UnitMovedEventArgs> _onUnitMove;
        
        public SpriteRenderer unitRenderer { get => _unitRenderer; }
        public SpriteRenderer weaponRenderer { get => _weaponRenderer; }
        public Pathfinding pathfinding { get => _pathfinding; }
        public Weapon weapon
        {
            get => _currentWeapon;
            set => _currentWeapon = value;
        }

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
            _weaponRenderer.transform.rotation = new Quaternion(0, 0, 0, 0);
            if (unitRenderer.flipX)
            {
                _weaponRenderer.flipX = true;
                _weaponRenderer.flipY = false;
            }
        }
        public void FlipSprite(Vector2 target)
        {
            Vector2 weaponHolderPosition = _weaponRenderer.transform.localPosition;
            float distance = Mathf.Abs(target.x - transform.position.x);
            if (target.x < _weaponRenderer.transform.position.x && distance > .1f)
            {
                if (_weaponRenderer.transform.localRotation.eulerAngles.z == 0)
                {
                    _weaponRenderer.flipX = true;
                    _weaponRenderer.flipY = false;
                }
                else
                {
                    _weaponRenderer.flipY = true;
                    _weaponRenderer.flipX = false;
                }
                _unitRenderer.flipX = true;
                _bodyArmorRenderer.flipX = true;
                _headgearRenderer.flipX = true;
            }
            else
            {
                _weaponRenderer.flipX = false;
                _weaponRenderer.flipY = false;
                
                _unitRenderer.flipX = false;
                _bodyArmorRenderer.flipX = false;
                _headgearRenderer.flipX = false;
            }
                
            if (_unitRenderer.flipX)
            {
                weaponHolderPosition.x = -0.1f;
            }
            else
            {
                weaponHolderPosition.x = 0.1f;
            }
            _weaponRenderer.transform.localPosition = weaponHolderPosition;
        }
        public UnitData GetUnitData() { return _unitData; }
    }
}


