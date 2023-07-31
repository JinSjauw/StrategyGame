using AI.Core;
using UnityEngine;

namespace AI.UtilityAI.Considerations
{
    [CreateAssetMenu(fileName = "Find Target", menuName = "UtilityAI/Considerations/Find Target")]
    public class TargetConsideration : Consideration
    {
        public override float ScoreConsideration(NPCUnit unit)
        {
            if (unit.awarenessSystem._seenTargets.Count > 0 && unit.awarenessSystem.target == null)
            {
                return 1f;
            }
            
            return 0f;
        }
    }
}