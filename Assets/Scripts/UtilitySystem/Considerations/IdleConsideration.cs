using System.Collections;
using System.Collections.Generic;
using AI.Core;
using UnityEngine;

namespace AI.UtilityAI.Considerations
{
    [CreateAssetMenu(fileName = "Idle", menuName = "UtilityAI/Considerations/Idle")]
    public class IdleConsideration : Consideration
    {
        public override float ScoreConsideration(NPCUnit unit)
        {
            return 0.2f;
        }
    }
}
