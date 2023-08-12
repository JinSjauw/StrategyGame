using UnityEngine;

public class HealthSystem : MonoBehaviour, IDamageable, IHealable
{
    [SerializeField] private int maxHealthPoints;
    [SerializeField] private int healthPoints;
    [SerializeField] private Transform splatterPrefab;
    private DebrisDispenser _debrisDispenser;
    
    public int maxHealth { get => maxHealthPoints; }
    public int currentHealth { get => healthPoints; }
    
    private void Awake()
    {
        healthPoints = maxHealthPoints;
        _debrisDispenser = GetComponent<DebrisDispenser>();
    }

    public void SpawnDebris(Vector2 direction, Vector2 impactPosition)
    {
        /*FakeHeightObject splatter = Instantiate(splatterPrefab, transform.position, Quaternion.identity).GetComponent<FakeHeightObject>();
        splatter.Initialize(direction.normalized * -1.2f, 2f, 4);*/
        _debrisDispenser.transform.position = impactPosition;
        _debrisDispenser.DispenseDebris(-direction);
    }

    public void TakeDamage(int damage)
    {
        healthPoints -= damage;
        Debug.Log(gameObject.name + " took " + damage);

        if (healthPoints <= 0)
        {
            Destroy(gameObject);
            Debug.Log(gameObject.name + " DIED!");
        }
    }

    public void Heal(int heal)
    {
        healthPoints += heal;
        Debug.Log(gameObject.name + " healed " + heal);
    }
}
