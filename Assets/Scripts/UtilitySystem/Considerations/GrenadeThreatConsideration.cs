using AI.Core;
using UnityEngine;

namespace AI.UtilityAI.Considerations
{
    [CreateAssetMenu(fileName = "BoomBoomTooClose", menuName = "UtilityAI/Considerations/Grenade Threat Check")]
    public class GrenadeThreatConsideration : Consideration
    {
        public Vector2 grenadePosition;
        
        public override float ScoreConsideration(Vector2 target, NPCUnit unit)
        {
            return ScoreConsideration(unit);
        }

        public override float ScoreConsideration(NPCUnit unit)
        {
            Collider2D hit = Physics2D.OverlapCircle(unit.transform.position, 5f, LayerMask.GetMask("Explosives"));
            if (hit != null)
            {
                grenadePosition = hit.transform.position;
                return 1f;
            }
            return 0f;
        }
    }
}