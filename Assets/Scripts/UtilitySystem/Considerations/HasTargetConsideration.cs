using AI.Core;
using UnityEngine;

namespace AI.UtilityAI.Considerations
{
    [CreateAssetMenu(fileName = "Has Target", menuName = "UtilityAI/Considerations/Has Target")]
    public class HasTargetConsideration : Consideration
    {
        public override float ScoreConsideration(Vector2 target, NPCUnit unit)
        {
            return ScoreConsideration(unit);
        }

        public override float ScoreConsideration(NPCUnit unit)
        {
            if (unit.awarenessSystem.target == null)
            {
                return 0f;
            }
            
            return 1f;
        }
    }
}