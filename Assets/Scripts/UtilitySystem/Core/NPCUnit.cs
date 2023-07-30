using AI.Awareness;
using UnityEngine;

namespace AI.Core
{
    [RequireComponent(
        typeof(AIBrain), 
        typeof(NPCUnitController),
        typeof(AwarenessSystem))]
    public class NPCUnit : MonoBehaviour
    {
        //NPC AGENT
        //Potential movement controller - To move the agent in the world
        
        //AI
        private AIBrain _aiBrain;
        private NPCUnitController _controller;
        private AwarenessSystem _awarenessSystem;
        
        private LevelGrid _levelGrid;
        private Pathfinding _pathfinding;
        
        //Actions List
        [SerializeField] private AIAction[] _availableActions;
        
        //World Variables
        [SerializeField] private TurnEventsHandler _turnEventsHandler;
        
        //Unit
        [SerializeField] private Weapon _currentWeapon;
        [SerializeField] private SpriteRenderer _unitSprite;
        [SerializeField] private SpriteRenderer _weaponSprite;

        //For various stats for considerations
        [SerializeField] private UnitData _unitData;
        public NPCUnitController controller { get => _controller; }
        public AwarenessSystem awarenessSystem { get => _awarenessSystem; }

        private void Awake()
        {
            _levelGrid = FindObjectOfType<LevelGrid>();
            _aiBrain = GetComponent<AIBrain>();
            _controller = GetComponent<NPCUnitController>();
            _awarenessSystem = GetComponent<AwarenessSystem>();
            _turnEventsHandler.OnTurnAdvanced += OnTurnAdvanced;
        }

        private void Start()
        {
            _controller.Initialize(_levelGrid);
            _awarenessSystem.Initialize(_unitData);
        }

        //Listen to the turnSystem
        private void OnTurnAdvanced()
        {
            _awarenessSystem.CheckDetection();
            _aiBrain.DecideBestAction(_availableActions, ExecuteBestAction);
        }

        private void ExecuteBestAction(AIAction bestAction)
        {
            bestAction.Execute(this);
        }
    }
}
