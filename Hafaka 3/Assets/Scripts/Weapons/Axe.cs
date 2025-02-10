using UnityEngine;

public class Axe : WeaponBase
{
    private float lastAttackTime;
    [SerializeField] Animator axeAnimator;

    void Start()
    {
        Damage = 10f;
        AttackCoolDown = 1f;
        WeaponType = WeaponType.Melee;
    }

    void Update()
    {
        // Handle input for attacking (optional)
        if (Input.GetMouseButtonDown(0) && Time.time >= lastAttackTime + AttackCoolDown)
        {
           
            Attack();
        }
    }

    public override void Attack()
    {
        lastAttackTime = Time.time;
        NotifyAttack();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Enemy") && collision.transform.TryGetComponent(out IDamageable enemy))
        {
            enemy.TakeDamage(Damage);
        }
    }
}
