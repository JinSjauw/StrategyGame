using System.Collections;
using System.Collections.Generic;
using AI.Core;
using UnityEngine;

namespace AI.UtilityAI
{
    [CreateAssetMenu(fileName = "ShootAction", menuName = "UtilityAI/Actions/Shoot")]
    public class Shoot : AIAction
    {
        public override void Execute(NPCUnit npcUnit)
        {
            Vector2 target = npcUnit.awarenessSystem.target.transform.position;
            //Calculate the randomness here?
            
            npcUnit.controller.Shoot(target);
        }
    }   
}
