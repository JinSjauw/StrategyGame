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
        private Unit _holderUnit;
        private UnitData _unitData;

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
            _holderUnit = unit;
            _unitData = unit.GetUnitStats();
            _state = ActionState.Started;
        }

        public abstract void SetAction(Vector2 target);
        
        public abstract ActionState Execute();
    }
}
