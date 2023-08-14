using AI.Core;
using UnitSystem;
using UnityEngine;

namespace AI.UtilityAI
{
    [CreateAssetMenu(fileName = "ThrowGrenade", menuName = "UtilityAI/Actions/Throw Grenade")]

    public class ThrowGrenade : AIAction
    {
        [SerializeField] private Transform throwablePrefab;
        private bool _targetIsPlayer;
        public override void Execute(NPCUnit npcUnit)
        {
            Vector2 target = npcUnit.awarenessSystem.target.transform.position;
            //Calculate the randomness here?
            if (npcUnit.awarenessSystem.target.GetComponent<PlayerUnit>())
            {
                _targetIsPlayer = true;
            }
            npcUnit.controller.Throw(target, throwablePrefab, false);
        }
    }
}