using System.Collections;
using System.Collections.Generic;
using AI.Core;
using UnityEngine;

namespace AI.UtilityAI
{
    [CreateAssetMenu(fileName = "AIIdle", menuName = "UtilityAI/Actions/Idle")]
    public class Idle : AIAction
    {
        public override void Execute(NPCUnit npcUnit)
        {
            npcUnit.Idle();
        }
    }
}

