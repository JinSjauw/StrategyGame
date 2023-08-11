using AI.Core;
using AI.UtilityAI.Considerations;
using UnityEngine;

namespace AI.UtilityAI
{
    [CreateAssetMenu(fileName = "AIRetreat", menuName = "UtilityAI/Actions/Retreat")]
    public class Retreat : AIAction
    {
        public override void Execute(NPCUnit npcUnit)
        {
            for (int i = 0; i < considerations.Length; i++)
            {
                GrenadeThreatConsideration grenade = considerations[i] as GrenadeThreatConsideration;
                if (grenade != null)
                {
                    npcUnit.controller.Retreat(grenade.grenadePosition);
                    return;
                }
            }
            npcUnit.controller.Retreat(npcUnit.awarenessSystem.target.transform.position);
        }
    }
}
