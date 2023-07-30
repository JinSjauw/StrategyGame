using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.Core
{
    public class NPCUnit : MonoBehaviour
    {
        //NPC AGENT
        //Potential movement controller - To move the agent in the world
        
        //AI Brain
        private AIBrain _aiBrain;
        
        //Actions List
        [SerializeField] private AIAction[] _availableActions;
        
        //World Variables
        [SerializeField] private TurnEventsHandler _turnEventsHandler;
        private LevelGrid _levelGrid;
        private Pathfinding _pathfinding;
        
        //Awareness System -- HERE ^
        
        //TEMP
        [SerializeField] private Unit _playerUnit;
        public Unit playerUnit { get => _playerUnit; }

        private void Awake()
        {
            _levelGrid = FindObjectOfType<LevelGrid>();
            _turnEventsHandler.OnTurnAdvanced += OnTurnAdvanced;
            _aiBrain = GetComponent<AIBrain>();
        }

        private void Start()
        {
            _pathfinding = new Pathfinding(_levelGrid);
        }

        //Listen to the turnSystem
        private void OnTurnAdvanced()
        {
            _aiBrain.DecideBestAction(_availableActions, ExecuteBestAction);
        }

        private void ExecuteBestAction(AIAction bestAction)
        {
            bestAction.Execute(this);
        }
        
        //Coroutines?

        #region Coroutines

        public void Chase()
        {
            StartCoroutine(ChaseCoroutine(_playerUnit.transform.position));
        }
        private IEnumerator ChaseCoroutine(Vector2 chasePosition)
        {
            List<Vector2> path = _pathfinding.FindPath(transform.position, chasePosition, true);
            
            yield return null;

            transform.position = path[1];
            
            Debug.Log("CHASE: " + path[1]);
        }

        public void Idle()
        {
            StartCoroutine(IdleCoroutine());
        }
        private IEnumerator IdleCoroutine()
        {
            yield return new WaitForSeconds(1.5f);
            
            Debug.Log("IDLING");
        }
        
        public void Wander()
        {
            StartCoroutine(WanderCoroutine());
        }
        private IEnumerator WanderCoroutine()
        {
            yield return null;
            
            Debug.Log("Wandering...");

            transform.position += new Vector3(1, 0, 0);
        }
        
        #endregion


        
    }
}
