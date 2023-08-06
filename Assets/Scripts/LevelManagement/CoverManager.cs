using System.Collections.Generic;
using UnityEngine;

namespace AI.Awareness
{
    public class CoverManager : MonoBehaviour
    {
        public static CoverManager Instance { get; private set; } = null;

        private LevelGrid _levelGrid;
        
        public List<CoverObject> _coverObjects { get; private set; } = new List<CoverObject>();

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            _levelGrid = FindObjectOfType<LevelGrid>();
        }

        public void Register(CoverObject coverObject)
        {
            _coverObjects.Add(coverObject);
            _levelGrid.GetTileGridObject(_levelGrid.GetGridPosition(coverObject.transform.position)).isOccupied = true;
        }

        public void Deregister(CoverObject coverObject)
        {
            _coverObjects.Remove(coverObject);
            _levelGrid.GetTileGridObject(_levelGrid.GetGridPosition(coverObject.transform.position)).isOccupied = false;
        }
    }
}
