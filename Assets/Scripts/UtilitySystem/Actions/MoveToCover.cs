using AI.Core;
using UnityEngine;

namespace AI.UtilityAI
{
    [CreateAssetMenu(fileName = "AIMoveToCover", menuName = "UtilityAI/Actions/Move To Cover")]
    public class MoveToCover : AIAction
    {
        [SerializeField] private Consideration[] targetConsiderations;
        public override void Execute(NPCUnit npcUnit)
        { 
            Vector2 coverPosition = npcUnit.awarenessSystem.SelectCover(targetConsiderations);
            npcUnit.controller.MoveToPosition(coverPosition);
        }
    }
}