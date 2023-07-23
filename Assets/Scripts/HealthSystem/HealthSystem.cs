using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour, IDamageable, IHealable
{
    [SerializeField] private int maxHealthPoints;
    [SerializeField] private int healthPoints;

    private void Awake()
    {
        healthPoints = maxHealthPoints;
    }

    public void TakeDamage(int damage)
    {
        healthPoints -= damage;
        Debug.Log(gameObject.name + " took " + damage);

        if (healthPoints <= 0)
        {
            Debug.Log(gameObject.name + " DIED!");
        }
    }

    public void Heal(int heal)
    {
        healthPoints += heal;
        Debug.Log(gameObject.name + " healed " + heal);
    }
}
