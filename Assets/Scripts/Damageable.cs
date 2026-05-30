using UnityEngine;

public interface Damageable
{
    int Health{get ; set;}
    void Damage(int damage);
}
