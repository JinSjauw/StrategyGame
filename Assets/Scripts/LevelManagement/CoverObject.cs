using AI.Awareness;
using UnityEngine;

public class CoverObject : MonoBehaviour, IDamageable
{
    // Start is called before the first frame update
    void Start()
    {
        CoverManager.Instance.Register(this);
    }

    private void OnDestroy()
    {
        if (CoverManager.Instance != null)
        {
            CoverManager.Instance.Deregister(this);
        }
    }

    public void SpawnDebris(Vector2 direction, Vector2 impactPosition)
    {
        
    }

    public void TakeDamage(int damage)
    {
        
    }
}
