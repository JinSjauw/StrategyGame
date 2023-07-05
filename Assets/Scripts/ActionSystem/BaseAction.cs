using System;
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
        private Unit _unit;
        private UnitStats _unitStats;

        protected Unit Unit
        {
            get { return _unit; }
            set { _unit = value; }
        }
        protected UnitStats UnitStats
        {
            get { return _unitStats; }
            set { _unitStats = value; }
        }
        protected ActionState State
        {
            get { return _state; }
            set { _state = value; }
        }

        internal void ActionComplete()
        {
            _state = ActionState.Completed;
        }

        internal void ActionStarted()
        {
            _state = ActionState.Started;
        }
        
        public virtual void Initialize(Unit unit)
        {
            _unit = unit;
            _unitStats = unit.GetUnitStats();
            _state = ActionState.Started;
        }
        
        public abstract ActionState Execute();
    }
}
