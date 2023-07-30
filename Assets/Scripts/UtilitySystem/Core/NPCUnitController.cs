using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.Core
{
    public class NPCUnitController : MonoBehaviour
    {
        private LevelGrid _levelGrid;
        private Pathfinding _pathfinding;
        
        public void Initialize(LevelGrid levelGrid)
        {
            _levelGrid = levelGrid;
            _pathfinding = new Pathfinding(_levelGrid);
        }
        
        #region Coroutines

        public void Chase(Vector2 targetPosition)
        {
            StartCoroutine(ChaseCoroutine(targetPosition));
        }
        private IEnumerator ChaseCoroutine(Vector2 chasePosition)
        {
            List<Vector2> path = _pathfinding.FindPath(transform.position, chasePosition, true);
                
            yield return null;
    
            transform.position = path[1];
                
            Debug.Log("CHASE: " + path[1]);
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
            yield return null;
                
            Debug.Log("Wandering...");
    
            transform.position += new Vector3(1, 0, 0);
        }
            
        #endregion
    }
}
