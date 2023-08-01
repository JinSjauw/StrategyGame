using System;
using System.Collections;
using System.Collections.Generic;
using AI.Awareness;
using UnitSystem;
using UnityEngine;

namespace AI.Core
{
    public class NPCUnitController : MonoBehaviour
    {
        [SerializeField] private MovementAnimation _moveAnimation;
        
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
            _unitSprite = npcUnit.unitSprite;
            _unitData = npcUnit.unitData;
        }

        #region Actions
        
        private float _current;
        
        private void OnUnitMoved(Vector2 origin, Vector2 destination)
        {
            UnitMovedEventArgs unitMovedEvent = new UnitMovedEventArgs(_npcUnit, origin, destination);
            _npcUnit.OnUnitMove?.Invoke(_npcUnit, unitMovedEvent);
        }
        
        private void Move(Vector2 origin, Vector2 destination, Action onComplete)
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
                    Move(origin, destination, () => isWalking = false);
                }
                Debug.Log("CHASE: " + path[1]);
            }
        }
        
        public void Retreat(DetectableTarget target)
        {
            Debug.Log(_npcUnit.name + " Retreating From: " + target.name);
            StartCoroutine(RetreatCoroutine());
        }
        private IEnumerator RetreatCoroutine()
        {
            //Run in the direction of map edge.
            Debug.Log("Running Away!!!!!");
            yield return null;
        }

        public void Idle()
        {
            StartCoroutine(IdleCoroutine());
        }
        private IEnumerator IdleCoroutine()
        {
            yield return new WaitForSeconds(1.5f);
                
            Debug.Log("IDLING");
        }
            
        public void Wander()
        {
            StartCoroutine(WanderCoroutine());
        }
        private IEnumerator WanderCoroutine()
        {
            Vector2 origin = transform.position;
            Vector2 destination = _pathfinding.GetRandomNeighbour(transform.position, true, true);

            if (Vector2.Distance(transform.position, destination) > 0.1f)
            {
                bool isWalking = true;
                
                while (isWalking)
                {
                    yield return null;
                    Move(origin, destination, () => isWalking = false);
                }
            }
            Debug.Log("Wandering...");
        }
            
        #endregion
    }
}
