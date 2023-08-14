using AI.Core;
using UnityEngine;

namespace AI.UtilityAI.Considerations
{
    [CreateAssetMenu(fileName = "AmmoAmount", menuName = "UtilityAI/Considerations/AmmoAmount")]
    public class AmmoConsideration : Consideration
    {
        public override float ScoreConsideration(Vector2 target, NPCUnit unit)
        {
            return ScoreConsideration(unit);
        }

        public override float ScoreConsideration(NPCUnit unit)
        {
            max = unit.weapon.AmmoCapacity;
            //Debug.Log(max + " unit: " + unit.name);
            float scaledValue = (unit.weapon.AmmoCount / min) / (max - min);
            
            return responseCurve.Evaluate(scaledValue);
        }
    }
}