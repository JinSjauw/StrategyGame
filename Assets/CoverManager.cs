using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.Awareness
{
    public class CoverManager : MonoBehaviour
    {
        public static CoverManager Instance { get; private set; } = null;
        public List<CoverObject> _coverObjects { get; private set; } = new List<CoverObject>();
        
        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public void Register(CoverObject coverObject)
        {
            _coverObjects.Add(coverObject);
        }

        public void Deregister(CoverObject coverObject)
        {
            _coverObjects.Remove(coverObject);
        }
    }
}
