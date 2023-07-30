using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.Core
{
    public abstract class AIAction : ScriptableObject
    {
        public string Name;
        private float _score;
        
        //Needs to know unitData
        
        public float score
        {
            get => _score;
            set => _score = Mathf.Clamp01(value);
        }

        public Consideration[] considerations;
        
        public virtual void Awake() { score = 0; }
        public abstract void Execute(NPCUnit npcUnit);
    }
}

