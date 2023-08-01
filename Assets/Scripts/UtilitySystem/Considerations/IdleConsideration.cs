using System.Collections;
using System.Collections.Generic;
using AI.Core;
using UnityEngine;

namespace AI.UtilityAI.Considerations
{
    [CreateAssetMenu(fileName = "Idle", menuName = "UtilityAI/Considerations/Idle")]
    public class IdleConsideration : Consideration
    {
        public override float ScoreConsideration(Vector2 target, NPCUnit unit)
        {
            return ScoreConsideration(unit);
        }

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
