using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.Core
{
    public abstract class Consideration : ScriptableObject
    {
        public string Name;

        private float _score;
        public float score
        {
            get => _score;
            set => _score = Mathf.Clamp01(value);
        }

        public virtual void Awake()
        {
            score = 0;
        }

        //Maybe pass in an AwarenessSystem. Something that lets the AGENT observe the world.
        //Instead of NPC Unit
        public abstract float ScoreConsideration(NPCUnit unit);
    }
}

