using UnityEngine;

namespace AI.Awareness
{
    public class DetectableTarget : MonoBehaviour
    {
        // Start is called before the first frame update
        private void Start()
        {
            DetectableManager.Instance.Register(this);
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

