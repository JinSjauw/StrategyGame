using System.Collections;
using System.Collections.Generic;
using AI.Core;
using UnityEngine;

namespace AI.UtilityAI
{
    [CreateAssetMenu(fileName = "AIChase", menuName = "UtilityAI/Actions/Chase")]
    public class Chase : AIAction
    {
        /* Move towards player */
        public override void Execute(NPCUnit npcUnit)
        {
            //npcUnit.Chase;
            npcUnit.controller.MoveToPosition(npcUnit.awarenessSystem.target.transform.position);
        }
    }
}

