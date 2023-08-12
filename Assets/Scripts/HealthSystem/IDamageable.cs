using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    public void SpawnDebris(Vector2 direction, Vector2 impactPosition);
    public void TakeDamage(int damage);
}
