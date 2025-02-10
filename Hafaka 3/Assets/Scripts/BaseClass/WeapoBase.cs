using System;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    public virtual float AttackCoolDown { get; protected set; }
    public virtual WeaponType WeaponType { get; protected set; }
    public virtual float Damage { get; protected set; }

    public event Action<float> OnAttack;

    // Core attack method to be implemented by specific weapons
    public abstract void Attack();

    protected void NotifyAttack()
    {
        OnAttack?.Invoke(Damage);
    }
}

public enum WeaponType
{
    Melee,
    Ranged
}
