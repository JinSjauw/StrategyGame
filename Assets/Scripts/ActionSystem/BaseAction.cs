using System;
using System.Collections.Generic;
using CustomInput;
using UnitSystem;
using UnityEngine;

namespace ActionSystem
{
    public abstract class BaseAction : ScriptableObject
    {
        private PlayerUnit _holderUnit;
        private UnitData _unitData;

        //[SerializeField] private InputReader _inputReader;

        protected PlayerUnit holderUnit
        {
            get { return _holderUnit; }
        }
        protected UnitData unitData
        {
            get { return _unitData; }
        }
        protected Action _onComplete { get; set; }
        //protected InputReader inputReader { get { return _inputReader; } }
        
        public virtual void Initialize(PlayerUnit unit, Action onComplete)
        {
            /*if (_inputReader == null)
            {
                Debug.Log("INPUT READER IS NULL");
            }*/
            _holderUnit = unit;
            _unitData = unit.GetUnitData();
            _onComplete = onComplete;
        }
        
        public abstract void SetAction(Vector2 target);
        public abstract List<Vector2> GetPreview();
        public abstract void Execute();
    }
}
