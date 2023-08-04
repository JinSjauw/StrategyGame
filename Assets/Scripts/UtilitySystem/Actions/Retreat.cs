using System.Collections;
using System.Collections.Generic;
using AI.Awareness;
using AI.Core;
using UnityEngine;

namespace AI.UtilityAI
{
    [CreateAssetMenu(fileName = "AIRetreat", menuName = "UtilityAI/Actions/Retreat")]
    public class Retreat : AIAction
    {
        public override void Execute(NPCUnit npcUnit)
        {
            npcUnit.controller.Retreat(npcUnit.awarenessSystem.target);
        }
    }
}
