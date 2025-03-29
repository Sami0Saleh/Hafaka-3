using UnityEngine;

public class Axe : WeaponBase
{
    private float lastAttackTime;
    [SerializeField] private Animator axeAnimator;
    [SerializeField] private GameObject hitboxObject;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private ParticleSystem hitEffect;
    [SerializeField] private float hitboxActiveDuration = 0.2f;

    private HitBoxScript toolHitbox;

    void Start()
    {
        Damage = 10f;
        AttackCoolDown = 1f;
        WeaponType = WeaponType.Melee;

        // Create and setup hitbox if not already assigned
        SetupHitbox();
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

        // Play attack animation
        if (axeAnimator != null)
        {
            axeAnimator.SetTrigger("Attack");
        }

        // Activate hitbox with a delay that matches the animation
        StartCoroutine(ActivateHitbox());

        // Notify any listeners about the attack
        NotifyAttack();
    }

    private System.Collections.IEnumerator ActivateHitbox()
    {
        // Wait for the animation to reach the point where the axe would hit
        yield return new WaitForSeconds(0.1f); // Adjust based on your animation

        // Activate hitbox
        if (hitboxObject != null)
        {
            hitboxObject.SetActive(true);
        }

        // Keep hitbox active for the specified duration
        yield return new WaitForSeconds(hitboxActiveDuration);

        // Deactivate hitbox
        //if (hitboxObject != null)
        //{
        //    hitboxObject.SetActive(false);
        //}
    }

    private void SetupHitbox()
    {
        // If hitbox not assigned, create one
        if (hitboxObject == null)
        {
            hitboxObject = new GameObject("AxeHitbox");
            hitboxObject.transform.parent = transform;
            hitboxObject.transform.localPosition = Vector3.forward * 0.5f; // Position in front of the axe

            // Add collider
            BoxCollider hitboxCollider = hitboxObject.AddComponent<BoxCollider>();
            hitboxCollider.size = new Vector3(0.3f, 0.3f, 0.3f); // Adjust size to match axe head
            hitboxCollider.isTrigger = true;
        }

        // Add SimpleToolHitbox component if not already present
        toolHitbox = hitboxObject.GetComponent<HitBoxScript>();
        if (toolHitbox == null)
        {
            toolHitbox = hitboxObject.AddComponent<HitBoxScript>();
        }

        // Configure the hitbox
        toolHitbox.SetWeaponDamage(Damage);
        toolHitbox.SetHitEffect(hitEffect);
        toolHitbox.SetHitSound(hitSound, audioSource);

        // Ensure hitbox is disabled initially
        //hitboxObject.SetActive(false);
    }

    // Optional: Update damage in realtime if it changes
    

    // This can be removed as we're using triggers now
    // private void OnCollisionEnter(Collision collision)
    // {
    //     if (collision.transform.CompareTag("Enemy") && collision.transform.TryGetComponent(out IDamageable enemy))
    //     {
    //         enemy.TakeDamage(Damage);
    //     }
    // }
}