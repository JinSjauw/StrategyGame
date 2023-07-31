using AI.Core;
using UnityEngine;

namespace AI.UtilityAI
{
    [CreateAssetMenu(fileName = "AISelectTarget", menuName = "UtilityAI/Actions/Select Target")]
    public class SelectTarget : AIAction
    {
        [Header("Target Selection Considerations")]
        [SerializeField] private Consideration[] targetConsiderations;
        public override void Execute(NPCUnit npcUnit)
        {
            //Run target selection HERE;
            npcUnit.awarenessSystem.SelectTarget(targetConsiderations);
        }
    }
}