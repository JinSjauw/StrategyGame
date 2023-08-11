using AI.Core;
using Items;
using UnityEngine;

namespace AI.UtilityAI.Considerations
{
    [CreateAssetMenu(fileName = "Has Item Check", menuName = "UtilityAI/Considerations/Has Item Check")]
    public class HasItemConsideration : Consideration
    {
        [SerializeField] private ItemType typeToCheck;
        public override float ScoreConsideration(Vector2 target, NPCUnit unit)
        {
            return ScoreConsideration(unit);
        }

        public override float ScoreConsideration(NPCUnit unit)
        {
            if (unit.HasItem(typeToCheck))
            {
                if (typeToCheck == ItemType.Throwable)
                {
                    return .5f;
                }
                
                return 0.75f;
            }
            return 0f;
        }
    }
}