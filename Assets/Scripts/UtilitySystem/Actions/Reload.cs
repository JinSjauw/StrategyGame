using AI.Core;
using UnityEngine;

namespace AI.UtilityAI
{
    [CreateAssetMenu(fileName = "ReloadAction", menuName = "UtilityAI/Actions/Reload")]
    public class Reload : AIAction
    {
        public override void Execute(NPCUnit npcUnit)
        {
            npcUnit.weapon.Load();
        }
    }
}