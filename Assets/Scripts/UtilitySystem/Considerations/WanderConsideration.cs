using AI.Core;
using UnityEngine;

namespace AI.UtilityAI.Considerations
{
    [CreateAssetMenu(fileName = "Wander", menuName = "UtilityAI/Considerations/Wander")]
    public class WanderConsideration : Consideration
    {
        public override float ScoreConsideration(NPCUnit unit)
        {
            if (unit.awarenessSystem._seenTargets.Count > 0 || unit.awarenessSystem._heardTargets.Count < 0)
            {
                return 0f;
            }
            
            return 0.2f;
        }
    }
}