using AI.Awareness;
using AI.Core;
using UnityEngine;

namespace AI.UtilityAI.Considerations
{
    [CreateAssetMenu(fileName = "LineOfSight", menuName = "UtilityAI/Considerations/Line Of Sight")]
    public class LineOfSightConsideration : Consideration
    {
        [SerializeField] private bool invert;
        [SerializeField] private LayerMask layer;
        public override float ScoreConsideration(Vector2 target, NPCUnit unit)
        {
            return ScoreConsideration(unit);
        }

        public override float ScoreConsideration(NPCUnit unit)
        {
            DetectableTarget target = unit.awarenessSystem.target;

            if (target == null) { return 0f;}
            
            if (Physics2D.Linecast(unit.transform.position, target.transform.position, layer))
            {
                if (invert)
                {
                    return 0f;
                }
                return .7f;
            }

            if (invert)
            {
                return .7f;
            }
            return 0f;
        }
    }
}