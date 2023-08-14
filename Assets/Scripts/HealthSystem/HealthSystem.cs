using Player;
using UnitSystem;
using UnityEngine;

public class HealthSystem : MonoBehaviour, IDamageable, IHealable
{
    [SerializeField] private float maxHealthPoints;
    [SerializeField] private float healthPoints;
    [SerializeField] private PlayerHUDEvents _playerHUD;
    [SerializeField] private bool isPlayer;

    private UnitData _unitData;
    private DebrisDispenser _debrisDispenser;
    
    public float maxHealth { get => maxHealthPoints; }
    public float currentHealth { get => healthPoints; }
    
    private void Awake()
    {
        _debrisDispenser = GetComponent<DebrisDispenser>();
        _unitData = GetComponent<Unit>().GetUnitData();
        maxHealthPoints = _unitData.maxHealth;
        healthPoints = maxHealthPoints;
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
        float mitigatedDamage = damage * ((100 - _unitData.totalDamageReduction) / 100);
        healthPoints -= mitigatedDamage;
        Debug.Log(gameObject.name + " took " + damage + " dealt damage: " + mitigatedDamage);
        
        if (isPlayer)
        {
            _playerHUD.RaiseHealthChanged(-damage);
        }
        
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
        _playerHUD.RaiseHealthChanged(heal);
    }
}
