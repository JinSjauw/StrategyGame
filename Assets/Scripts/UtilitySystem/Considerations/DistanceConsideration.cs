using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI.Core;

namespace AI.UtilityAI.Considerations
{
    [CreateAssetMenu(fileName = "Distance", menuName = "UtilityAI/Considerations/Distance To Player")]
    public class DistanceConsideration : Consideration
    {
        [SerializeField] private AnimationCurve distanceCurve;
        [SerializeField] private float min = 1;
        [SerializeField] private float max;
        public override float ScoreConsideration(NPCUnit unit)
        {
            if (unit.awarenessSystem._seenTargets.Count <= 0)
            {
                return 0f;
            }

            Vector2 targetPosition = unit.awarenessSystem._seenTargets[0].transform.position;
            float distance = Vector2.Distance(targetPosition, unit.transform.position);
            float scaledValue = (distance / min) / (max - min);
            
            Debug.Log("World distance: " + distance + " Scaled Value: " + scaledValue + " Value on Curve: " + distanceCurve.Evaluate(scaledValue));
            
            return distanceCurve.Evaluate(scaledValue);
        }
    }
}

