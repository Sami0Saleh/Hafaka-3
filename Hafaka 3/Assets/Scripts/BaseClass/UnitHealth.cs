using System;
using UnityEngine;

public class UnitHealth : MonoBehaviour,IDamageable
{
    public virtual float Health { get; protected set; }
    public virtual float MaxHealth { get; protected set; }
    public bool IsAlive { get; protected set; }

    public static event Action<float> OnHit;
    public static event Action OnDeath;
    public virtual void TakeDamage(float damage)
    {
        Health-=damage;
        IsAlive = CheckIfDead();
        if (!IsAlive) Death(); 
        OnHit.Invoke(damage);
        
    }

    public bool CheckIfDead()
    {
        if (Health <= 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    public virtual void Death()
    {
        Debug.Log("Player is Dead");
        OnDeath?.Invoke();
    }
    public virtual void Awake()
    {
        MaxHealth = 100;
        Health = MaxHealth;
        IsAlive = true;
    }
}
