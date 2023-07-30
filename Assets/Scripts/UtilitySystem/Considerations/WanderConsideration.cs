using AI.Core;
using UnityEngine;

namespace AI.UtilityAI.Considerations
{
    [CreateAssetMenu(fileName = "Wander", menuName = "UtilityAI/Considerations/Wander")]
    public class WanderConsideration : Consideration
    {
        public override float ScoreConsideration(NPCUnit unit)
        {
            return 0.5f;
        }
    }
}