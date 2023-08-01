using System.Collections;
using System.Collections.Generic;
using AI.Core;
using UnityEngine;


namespace AI.UtilityAI.Considerations
{
    [CreateAssetMenu(fileName = "Health", menuName = "UtilityAI/Considerations/Health")]
    public class HealthConsideration : Consideration
    {
        public override float ScoreConsideration(Vector2 target, NPCUnit unit)
        {
            return ScoreConsideration(unit);
        }

        public override float ScoreConsideration(NPCUnit unit)
        {
            max = unit.healthSystem.maxHealth;

            float currentHealth = unit.healthSystem.currentHealth;
            float scaledValue = (currentHealth / min) / (max - min);

            return responseCurve.Evaluate(scaledValue);
        }
    }
}

