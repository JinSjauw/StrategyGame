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
        
        protected Unit Unit
        {
            get { return _unit; }
            set { _unit = value; }
        }
        protected ActionState State
        {
            get { return _state; }
            set { _state = value; }
        }

        internal void Initialize(Unit unit)
        {
            _unit = unit;
            _state = ActionState.Started;
        }
        
        internal void ActionComplete()
        {
            _state = ActionState.Completed;
        }

        internal void ActionStarted()
        {
            _state = ActionState.Started;
        }

        public virtual void PrepareAction() { }

        public abstract ActionState Execute();
    }
}
