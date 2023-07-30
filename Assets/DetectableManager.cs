using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.Awareness
{
    public class DetectableManager : MonoBehaviour
    {
        public static DetectableManager Instance { get; private set; } = null;

        public List<DetectableTarget> _detectableTargets { get; private set; } = new List<DetectableTarget>();

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public void Register(DetectableTarget target)
        {
            _detectableTargets.Add(target);
        }

        public void Deregister(DetectableTarget target)
        {
            _detectableTargets.Remove(target);
        }
    }
}

