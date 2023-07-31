using AI.Awareness;
using UnitSystem;
using UnityEngine;

namespace AI.Core
{
    [RequireComponent(
        typeof(AIBrain), 
        typeof(NPCUnitController),
        typeof(AwarenessSystem))]
    public class NPCUnit : Unit
    {
        //NPC AGENT
        //Potential movement controller - To move the agent in the world
        
        //AI
        private AIBrain _aiBrain;
        private NPCUnitController _controller;
        private AwarenessSystem _awarenessSystem;
        private HealthSystem _healthSystem;
        
        
        //Actions List
        [SerializeField] private AIAction[] _availableActions;
        
        //World Variables
        [SerializeField] private TurnEventsHandler _turnEventsHandler;
        
        //Unit
        public NPCUnitController controller { get => _controller; }
        public AwarenessSystem awarenessSystem { get => _awarenessSystem; }
        public HealthSystem healthSystem { get => _healthSystem; }
        
        
        private void Awake()
        {
            _levelGrid = FindObjectOfType<LevelGrid>();
            _aiBrain = GetComponent<AIBrain>();
            _controller = GetComponent<NPCUnitController>();
            _awarenessSystem = GetComponent<AwarenessSystem>();
            _healthSystem = GetComponent<HealthSystem>();
            
            _turnEventsHandler.OnTurnAdvanced += OnTurnAdvanced;
        }

        private void Start()
        {
            _controller.Initialize(_levelGrid, this);
            _awarenessSystem.Initialize(this);

            _onUnitMove += _levelGrid.Unit_OnUnitMoved;
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
