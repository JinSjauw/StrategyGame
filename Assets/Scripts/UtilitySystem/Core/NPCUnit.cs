using System;
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

            weapon.Equip(_weaponRenderer, OnShoot);
            //_weaponSprite.sprite = _currentWeapon.GetSprite();
            
            Copy(_availableActions);
        }

        private void Start()
        {
            _controller.Initialize(_levelGrid, this);
            _awarenessSystem.Initialize(_levelGrid, this);
            
            _onUnitMove += _levelGrid.Unit_OnUnitMoved;
        }
    
        private void Copy(AIAction[] actions)
        {
            for (int i = 0; i < actions.Length; i++)
            {
                AIAction action = actions[i];
                action = Instantiate(action);
                actions[i] = action;
                for (int j = 0; j < action.considerations.Length; j++)
                {
                    Consideration consideration = action.considerations[j];
                    consideration = Instantiate(consideration);
                    action.considerations[j] = consideration;
                }
            }
        }

        private void OnShoot()
        {
            Debug.Log("NPC: " + gameObject.name + " SHOT!");
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

        private void OnDestroy()
        {
            _onUnitMove -= _levelGrid.Unit_OnUnitMoved;
            _turnEventsHandler.OnTurnAdvanced -= OnTurnAdvanced;
        }
    }
}
