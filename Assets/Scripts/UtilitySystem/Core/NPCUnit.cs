using System.Collections.Generic;
using AI.Awareness;
using Items;
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
        [SerializeField] private List<BaseItem> _items;
        [SerializeField] private Bullet _bullet;
        
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

            Copy(_availableActions);
        }

        private void Start()
        {
            _controller.Initialize(_levelGrid, this);
            _awarenessSystem.Initialize(_levelGrid, this);
            _onUnitMove += _levelGrid.Unit_OnUnitMoved;

            weapon = Instantiate(weapon).Equip(_weaponRenderer, OnShoot, true);
            //weapon.Load();
            _unitRenderer.enabled = false;
            _weaponRenderer.enabled = false;

            Initialize(_levelGrid);
        }

        private void Update()
        {
            if (awarenessSystem.target != null)
            {
                //Aim;
                /*Vector2 targetPosition = awarenessSystem.target.transform.position;
                Aim(targetPosition);
                FlipSprite(targetPosition);*/
            }
            else
            {
                StopAim();
            }
        }

        public override void Aim(Vector2 target)
        {
            Vector2 targetDirection = target - (Vector2)_weaponRenderer.transform.position;
            float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
            _weaponRenderer.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
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

        public bool HasItem(ItemType typeToCheck)
        {
            for (int i = 0; i < _items.Count; i++)
            {
                BaseItem item = _items[i];
                if (item.GetItemType() == typeToCheck)
                {
                    return true;
                }
            }

            return false;
        }
        
        public BaseItem GetThrowable()
        {
            for (int i = 0; i < _items.Count; i++)
            {
                BaseItem item = _items[i];
                if (item.GetItemType() == ItemType.Throwable)
                {
                    _items.Remove(item);
                    return item;
                }
            }
            return null;
        }
    }
}
