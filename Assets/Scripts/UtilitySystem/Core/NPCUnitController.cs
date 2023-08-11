using System;
using System.Collections;
using System.Collections.Generic;
using AI.Awareness;
using Items;
using Player;
using SoundManagement;
using UnitSystem;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AI.Core
{
    public class NPCUnitController : MonoBehaviour
    {
        [SerializeField] private MovementAnimation _moveAnimation;
        [SerializeField] private SFXEventChannel _sfxEventChannel;

        private LevelGrid _levelGrid;
        private Pathfinding _pathfinding;
        private NPCUnit _npcUnit;
        private SpriteRenderer _unitSprite;
        private UnitData _unitData;
        
        public void Initialize(LevelGrid levelGrid, NPCUnit npcUnit)
        {
            _levelGrid = levelGrid;
            _pathfinding = new Pathfinding(_levelGrid);

            _npcUnit = npcUnit;
            _unitSprite = npcUnit.unitRenderer;
            _unitData = npcUnit.unitData;
        }

        #region Actions
        
        private float _current;
        
        private void OnUnitMoved(Vector2 origin, Vector2 destination)
        {
            UnitMovedEventArgs unitMovedEvent = new UnitMovedEventArgs(_npcUnit, origin, destination);
            _npcUnit.OnUnitMove?.Invoke(_npcUnit, unitMovedEvent);
        }
        
        private void MoveAnimation(Vector2 origin, Vector2 destination, Action onComplete)
        {
            _current = Mathf.MoveTowards(_current, 1, _unitData.moveSpeed * Time.deltaTime);
            if (_current < 1f)
            {
                transform.position = Vector2.Lerp(origin, destination, _moveAnimation.MovementCurve.Evaluate(_current));
                _moveAnimation.PlayMoveAnimation(_current, _unitSprite);
            }
            else if (_current >= 1f)
            {
                OnUnitMoved(origin, destination);
                _current = 0;
                _moveAnimation.PlayMoveAnimation(0, _unitSprite);
                onComplete();
            }
        }
        
        #endregion
        
        #region Coroutines
        
        public void MoveToPosition(Vector2 targetPosition)
        {
            _npcUnit.FlipSprite(targetPosition);
            StartCoroutine(MoveToCoroutine(targetPosition));
        }
        private IEnumerator MoveToCoroutine(Vector2 targetPosition)
        {
            List<Vector2> path = _pathfinding.FindPath(transform.position, targetPosition, false);
            if (path.Count > 1 && !_levelGrid.GetTileGridObject(path[1]).isOccupied)
            {
                Vector2 origin = path[0];
                Vector2 destination = path[1];
                bool isWalking = true;
                while (isWalking)
                {
                    yield return null;
                    MoveAnimation(origin, destination, () => isWalking = false);
                }
                //Debug.Log("Moving To: " + path[1]);
            }
        }
        
        public void Retreat(Vector2 target)
        {
            //Debug.Log(_npcUnit.name + " Retreating From: " + target.name);
            StartCoroutine(RetreatCoroutine(target));
        }
        private IEnumerator RetreatCoroutine(Vector2 target)
        {
            //Run in the direction of map edge.
            Vector2 origin = transform.position;
            Vector2 destination = _pathfinding.GetRandomNeighbour(transform.position, true, true, target, true);
            _npcUnit.FlipSprite(destination);
            if (Vector2.Distance(transform.position, destination) > 0.1f)
            {
                bool isWalking = true;
                
                while (isWalking)
                {
                    yield return null;
                    MoveAnimation(origin, destination, () => isWalking = false);
                }
            }
            //Debug.Log("Running Away!!!!!");
            //Select neighbour 
            yield return null;
        }

        public void Idle()
        {
            StartCoroutine(IdleCoroutine());
        }
        private IEnumerator IdleCoroutine()
        {
            yield return new WaitForSeconds(1.5f);
                
            //Debug.Log("IDLING");
        }
            
        public void Wander()
        {
            StartCoroutine(WanderCoroutine());
        }
        private IEnumerator WanderCoroutine()
        {
            Vector2 origin = transform.position;
            Vector2 destination = _pathfinding.GetRandomNeighbour(transform.position, true, true);
            _npcUnit.FlipSprite(destination);
            if (Vector2.Distance(transform.position, destination) > 0.1f)
            {
                bool isWalking = true;
                
                while (isWalking)
                {
                    yield return null;
                    MoveAnimation(origin, destination, () => isWalking = false);
                }
            }
            //Debug.Log("Wandering...");
        }
            
        public void Shoot(Vector2 target)
        {
            //
            _npcUnit.Aim(target);

            target += Random.insideUnitCircle;

            bool ignore = false;
            int throughCoverChance = Random.Range(0, 4);
            if (throughCoverChance >= 3) { ignore = true; }
            
            //Debug.Log("CoverIgnore: " + throughCoverChance);
            
            _npcUnit.Aim(target);
            _npcUnit.FlipSprite(target);
            _npcUnit.weapon.Shoot(ignore);
            //Debug.Log(" SHOOOOOOTT");
            _sfxEventChannel.RequestSFX(_npcUnit.weapon.GetSFXConfig().GetShootClip(), _npcUnit.transform.position);
        }
        #endregion

        public void Throw(Vector2 target, Transform throwablePrefab, bool targetIsPlayer)
        {
            Vector2 adjustedTarget = target;
            Vector2 origin = transform.position;
            float distance = Vector2.Distance(origin, target);
            //Debug.Log($"Distance: {distance}");
            if (distance > _npcUnit.unitData.detectionRadius / 2)
            {
                //Set it to the closest
                Vector2 validTarget = target - origin;
                adjustedTarget = origin + Vector2.ClampMagnitude(validTarget, _npcUnit.unitData.detectionRadius / 2);
                Debug.Log($"Target: {target}");
            }
            
            //Get grenade from NPCUNIT
            BaseItem throwableItem = _npcUnit.GetThrowable();
            if (throwableItem == null) return;
            
            ThrowableObject throwable = Instantiate(throwablePrefab).GetComponent<ThrowableObject>();
            throwable.Initialize(throwableItem, transform.position, adjustedTarget, targetIsPlayer);
        }
    }
}
