using AI.Core;
using UnityEngine;

namespace AI.UtilityAI.Considerations
{
    [CreateAssetMenu(fileName = "Cover Available", menuName = "UtilityAI/Considerations/Cover Available")]
    public class CoverAvailable : Consideration
    {
        public override float ScoreConsideration(Vector2 target, NPCUnit unit)
        {
            return ScoreConsideration(unit);
        }

        public override float ScoreConsideration(NPCUnit unit)
        {
            if (unit.awarenessSystem._coverObjects.Count <= 0)
            {
                return 0f;
            }

            return .7f;
        }
    }
}