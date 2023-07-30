using System.Collections;
using System.Collections.Generic;
using AI.Core;
using UnityEngine;

namespace AI.UtilityAI
{
    [CreateAssetMenu(fileName = "AIWander", menuName = "UtilityAI/Actions/Wander")]
    public class Wander : AIAction
    {
        public override void Execute(NPCUnit npcUnit)
        {
            npcUnit.controller.Wander();
        }
    }
}

