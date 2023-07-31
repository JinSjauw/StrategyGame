using System.Collections;
using System.Collections.Generic;
using UnitSystem;
using UnityEngine;

namespace AI.Awareness
{
    public class AwarenessSystem : MonoBehaviour
    {
        //Search for DetectableTarget Scripts;
        private UnitData _unitData;
        
        public List<DetectableTarget> _seenTargets { get; private set; } = new List<DetectableTarget>();
        public List<DetectableTarget> _heardTargets { get; private set; } = new List<DetectableTarget>();

        private void CanSee(DetectableTarget seen)
        {
            if (_heardTargets.Contains(seen))
            {
                _heardTargets.Remove(seen);
            }

            if (!_seenTargets.Contains(seen))
            {
                _seenTargets.Add(seen);   
            }
        }

        private void CanHear(DetectableTarget heard)
        {
            if (!_heardTargets.Contains(heard))
            {
                _heardTargets.Add(heard);
            }
        }
    
        public void Initialize(UnitData unitData)
        {
            _unitData = unitData;
        }
    
        //When the turn advances check for radius
        public void CheckDetection()
        {
            _seenTargets.Clear();
            _heardTargets.Clear();
            //Check for any enemy units In range
            for (int i = 0; i < DetectableManager.Instance._detectableTargets.Count; i++)
            {
                DetectableTarget candidateTarget = DetectableManager.Instance._detectableTargets[i];

                if (candidateTarget.gameObject == gameObject) { continue; }

                Vector2 vectorToTarget = candidateTarget.transform.position - transform.position;
            
                if(vectorToTarget.sqrMagnitude > _unitData.detectionRadius * _unitData.detectionRadius) { continue; }

                if (Physics2D.Raycast(transform.position, candidateTarget.transform.position))
                {
                    CanSee(candidateTarget);
                }
                else
                {
                    CanHear(candidateTarget);
                }
            }

            foreach (var target in _seenTargets)
            {
                Debug.Log(target.name);
            }
        }
    }
}


