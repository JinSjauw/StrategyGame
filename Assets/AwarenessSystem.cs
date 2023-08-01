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
        private LevelGrid _levelGrid;
        
        public DetectableTarget target { get; private set; }
        public Vector2 coverPosition { get; private set; }
        
        public List<DetectableTarget> _seenTargets { get; private set; } = new List<DetectableTarget>();
        public List<DetectableTarget> _heardTargets { get; private set; } = new List<DetectableTarget>();
        public List<CoverObject> _coverObjects { get; private set; } = new List<CoverObject>();

        private float CalculateConsiderations(float lumpedScore, int length)
        {
            float averageScore = lumpedScore;
            float modFactor = 1 - (1 / length);
            float makeupValue = (1 - averageScore) * modFactor;
            return averageScore + (makeupValue * averageScore);
        }
        
        public void CheckDetection()
        {
            DetectTargets();
            DetectCover();
        }
    
        public void Initialize(LevelGrid levelGrid, NPCUnit npcUnit)
        {
            _unitData = npcUnit.unitData;
            _npcUnit = npcUnit;
            _levelGrid = levelGrid;
        }

        #region Target Selection/Detection

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
        private void DetectTargets()
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
            float score = 0;
            DetectableTarget bestTarget = null;
            //Run some considerations
            for (int i = 0; i < _seenTargets.Count; i++)
            {
                float lumpedScore = 1f;
                for (int j = 0; j < considerations.Length; j++)
                {
                    float considerationScore = considerations[j].ScoreConsideration(_seenTargets[i].transform.position, _npcUnit);
                    Debug.Log("Consideration Score: " + considerationScore);
                    lumpedScore *= considerationScore;
                    if (lumpedScore == 0)
                    {
                        score = 0;
                        break;
                    }
                }
                score = CalculateConsiderations(lumpedScore, considerations.Length);
                if (bestScore < score)
                {
                    bestTarget = _seenTargets[i];
                    bestScore = score;
                }
            }
            target = bestTarget;
            
            if (target != null)
            {
                Debug.Log(transform.name + " Selected: " + target.name);
            }
        }
        
        //When the turn advances check for radius

        #endregion

        #region Cover Selection/Detection
        
        private void DetectCover()
        {
            _coverObjects.Clear();
            //Check coverList;
            for (int i = 0; i < CoverManager.Instance._coverObjects.Count; i++)
            {
                CoverObject coverCandidate = CoverManager.Instance._coverObjects[i];
                
                if(_coverObjects.Contains(coverCandidate)) { continue; }

                Vector2 vectorToCover = coverCandidate.transform.position - transform.position;
                
                if(vectorToCover.sqrMagnitude > (_unitData.detectionRadius / 2) * (_unitData.detectionRadius / 2)) { continue; }
                
                _coverObjects.Add(coverCandidate);
            }
        }

        public Vector2 SelectCover(Consideration[] considerations)
        {
            /*if (coverPosition != Vector2.zero)
            {
                return coverPosition;
            }*/

            float bestScore = 0;
            Vector2 bestCover = Vector2.zero;

            CoverObject closestCover = _coverObjects[0];
            float closestDistance = _unitData.detectionRadius * 2;
            //Run some considerations
            for (int coverIndex = 0; coverIndex < _coverObjects.Count; coverIndex++)
            {
                CoverObject cover = _coverObjects[coverIndex];
                if(cover == closestCover) { continue; }
                float distance = Vector2.Distance(cover.transform.position, _npcUnit.transform.position);
                if (distance < closestDistance)
                {
                    closestCover = cover;
                    closestDistance = distance;
                }
            }
            
            //Look for a suitable cover position;
            List<Vector2> coverPositions = new List<Vector2>();
            List<TileGridObject> neighbours = new List<TileGridObject>();
            neighbours = _levelGrid.GetNeighbours(_levelGrid.GetGridPosition(closestCover.transform.position), true);

            for (int neighbourIndex = 0; neighbourIndex < neighbours.Count; neighbourIndex++)
            {
                TileGridObject tileGridObject = neighbours[neighbourIndex];
                if(tileGridObject.isOccupied || !tileGridObject.isWalkable){ continue; }

                RaycastHit2D hit = Physics2D.Linecast(tileGridObject.m_WorldPosition, target.transform.position, LayerMask.GetMask("Cover"));
                
                if(!hit.collider) { continue; }
                
                if (hit.collider.gameObject == closestCover.gameObject)
                {
                    coverPositions.Add(tileGridObject.m_WorldPosition);
                }
            }

            if (coverPositions.Count >= 1)
            {
                bestCover = coverPositions[Random.Range(0, coverPositions.Count - 1)];
            }
            
            if (coverPositions.Count > 0)
            {
                coverPosition = bestCover;
            }
            
            return coverPosition;
        }

        #endregion
        
    }
}


