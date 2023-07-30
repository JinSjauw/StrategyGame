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
        [SerializeField] private float min;
        [SerializeField] private float max;
        public override float ScoreConsideration(NPCUnit unit)
        {
            float distance = Vector2.Distance(unit.playerUnit.transform.position, unit.transform.position);
            float scaledValue = (distance / min) / (max - min);
            
            Debug.Log("World distance: " + distance + " Scaled Value: " + scaledValue + " Value on Curve: " + distanceCurve.Evaluate(scaledValue));
            
            return distanceCurve.Evaluate(scaledValue);
        }
    }
}

