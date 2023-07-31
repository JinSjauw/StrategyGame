using System.Collections;
using System.Collections.Generic;
using AI.Core;
using UnitSystem;
using UnityEngine;

namespace AI.Awareness
{
    public class AwarenessSystem : MonoBehaviour
    {
        //Search for DetectableTarget Scripts;
        private UnitData _unitData;
        private NPCUnit _npcUnit;

        public DetectableTarget target { get; private set; }
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
                
                //Select closest Target
            }
        }

        private void CanHear(DetectableTarget heard)
        {
            if (!_heardTargets.Contains(heard))
            {
                _heardTargets.Add(heard);
            }
        }
    
        public void Initialize(NPCUnit npcUnit)
        {
            _unitData = npcUnit.unitData;
            _npcUnit = npcUnit;
        }
        
        public void SetClosestTarget()
        {
            int closestTargetIndex = 0;
            for (int i = 0; i < _seenTargets.Count; i++)
            {
                
                if (Vector2.Distance(_seenTargets[i].transform.position, transform.position) <=
                    Vector2.Distance(_seenTargets[closestTargetIndex].transform.position, transform.position))
                {
                    closestTargetIndex = i;
                }
            }

            target = _seenTargets[closestTargetIndex];
        }

        public void SelectTarget(Consideration[] considerations)
        {
            float bestScore = 0;
            DetectableTarget bestTarget = null;
            //Run some considerations
            for (int i = 0; i < _seenTargets.Count; i++)
            {
                float score;
                float lumpedScore = 1f;
                for (int j = 0; j < considerations.Length; j++)
                {
                    float considerationScore = considerations[j].ScoreConsideration(_npcUnit);
                    lumpedScore *= considerationScore;

                    if (lumpedScore == 0)
                    {
                        score = 0;
                        break;
                    }
                }

                //Average Scheme the lumped score
                float averageScore = lumpedScore;
                float modFactor = 1 - (1 / considerations.Length);
                float makeupValue = (1 - averageScore) * modFactor;
                score = averageScore + (makeupValue * averageScore);

                if (bestScore < score)
                {
                    bestTarget = _seenTargets[i];
                }
            }

            target = bestTarget;
            if (target != null)
            {
                Debug.Log(transform.name + " Selected: " + target.name);
            }
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

                if(candidateTarget.targetType == _npcUnit.targetType) { continue; }
                
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

            //Check if you lost current target
            if (target != null)
            {
                Vector2 vectorToCurrentTarget = target.transform.position - transform.position;

                if (vectorToCurrentTarget.sqrMagnitude > _unitData.detectionRadius * _unitData.detectionRadius)
                {
                    target = null;
                }
            }
            
        }
    }
}


