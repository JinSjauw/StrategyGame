using System;
using System.Collections.Generic;
using UnityEngine;

namespace ActionSystem
{
    public enum ActionState
    {
        Started = 0,
        Performing = 1,
        Completed = 2,
    }
    
    public abstract class BaseAction : ScriptableObject
    {
        private ActionState _state;
        private Unit _holderUnit;
        private UnitData _unitData;

        [SerializeField] private InputReader _inputReader;

        protected Unit holderUnit
        {
            get { return _holderUnit; }
        }
        protected UnitData unitData
        {
            get { return _unitData; }
        }
        protected ActionState state
        {
            get { return _state; }
        }
        protected Action _onActionComplete { get; set; }

        /*internal void ActionComplete()
        {
            _state = ActionState.Completed;
        }*/

        /*internal void ActionStarted()
        {
            _state = ActionState.Started;
        }*/
        
        public virtual void Initialize(Unit unit)
        {
            if (_inputReader == null)
            {
                Debug.Log("INPUT READER IS NULL");
            }
            
            _holderUnit = unit;
            _unitData = unit.GetUnitStats();
            _state = ActionState.Started;
        }

        public abstract List<Vector2> SetAction(Vector2 target, Action onComplete);
        
        public abstract ActionState Execute();
    }
}
