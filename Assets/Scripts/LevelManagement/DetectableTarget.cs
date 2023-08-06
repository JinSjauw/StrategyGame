using UnitSystem;
using UnityEngine;

namespace AI.Awareness
{

    public enum TargetType
    {
        Player = 0,
        EnemyOne = 1,
        EnemyTwo = 2,
        EnemyThree = 3,
        Grenades = 4,
    }
    
    
    public class DetectableTarget : MonoBehaviour
    {
        // Start is called before the first frame update

        public TargetType targetType { get; private set; }

        private void Start()
        {
            DetectableManager.Instance.Register(this);
            targetType = GetComponent<Unit>().targetType;
        }
    
        private void OnDestroy()
        {
            if (DetectableManager.Instance != null)
            {
                DetectableManager.Instance.Deregister(this);
            }
        }
    }
}

