using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI.Core;

namespace AI.UtilityAI.Considerations
{
    [CreateAssetMenu(fileName = "Distance", menuName = "UtilityAI/Considerations/Distance To Target")]
    public class DistanceConsideration : Consideration
    {
        private float CalculateScore(Vector2 origin, Vector2 target)
        {
            Vector2 targetPosition = target;
            float distance = Vector2.Distance(targetPosition, origin);
            float scaledValue = (distance / min) / (max - min);

            //Debug.Log("World distance: " + distance + " Scaled Value: " + scaledValue + " Value on Curve: " + responseCurve.Evaluate(scaledValue));
            
            return responseCurve.Evaluate(scaledValue);
        }

        public override float ScoreConsideration(Vector2 target, NPCUnit unit)
        {
            max = unit.unitData.detectionRadius;
            
            return CalculateScore(unit.transform.position, target);
        }

        public override float ScoreConsideration(NPCUnit unit)
        {
            if (unit.awarenessSystem._seenTargets.Count <= 0 || unit.awarenessSystem.target == null)
            {
                return 0f;
            }

            max = unit.unitData.detectionRadius;
            
            Vector2 targetPosition = unit.awarenessSystem.target.transform.position;
            return CalculateScore(unit.transform.position, targetPosition);
        }
    }
}

