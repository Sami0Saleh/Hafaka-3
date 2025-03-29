using UnityEngine;

[RequireComponent(typeof(Collider))]
public class HitBoxScript : MonoBehaviour
{
    [SerializeField] private float weaponDamage;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private ParticleSystem hitEffect;

    private bool hasDealtDamage = false;

    private void Awake()
    {
        // Make sure the collider is a trigger
        Collider col = GetComponent<Collider>();
        if (col != null && !col.isTrigger)
        {
            col.isTrigger = true;
        }

        // If no enemy layer is set, default to "Enemy" layer if it exists
        if (enemyLayer == 0)
        {
            int enemyLayerIndex = LayerMask.NameToLayer("Enemy");
            if (enemyLayerIndex >= 0)
            {
                enemyLayer = 1 << enemyLayerIndex;
            }
        }

        // Disable the GameObject at start
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        // Reset damage flag when hitbox is enabled
        hasDealtDamage = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        // If already dealt damage in this activation, ignore
        if (hasDealtDamage) return;

        // Check if the collider belongs to an enemy using layer mask or tag
        bool isEnemy = ((1 << other.gameObject.layer) & enemyLayer) != 0 || other.CompareTag("Enemy");

        if (isEnemy)
        {
            // Try to get any damageable interface
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                // Deal damage
                damageable.TakeDamage(weaponDamage);

                // Mark as dealt damage for this activation (optional - remove if you want to hit multiple enemies)
                hasDealtDamage = true;

                // Play hit sound
                PlayHitSound();

                // Spawn hit effect
                SpawnHitEffect(other.ClosestPoint(transform.position));
            }
            else
            {
                // Try the EnemyMovement from your other script as fallback
                var enemy = other.GetComponent<EnemyController>();
                if (enemy != null)
                {
                    enemy.TakeDamage(Mathf.RoundToInt(weaponDamage));

                    // Mark as dealt damage for this activation (optional)
                    hasDealtDamage = true;

                    // Play hit sound
                    PlayHitSound();

                    // Spawn hit effect
                    SpawnHitEffect(other.ClosestPoint(transform.position));
                }
            }
        }
    }

    private void PlayHitSound()
    {
        if (audioSource != null && hitSound != null)
        {
            audioSource.PlayOneShot(hitSound);
        }
    }

    private void SpawnHitEffect(Vector3 position)
    {
        if (hitEffect != null)
        {
            ParticleSystem effect = Instantiate(hitEffect, position, Quaternion.identity);
            effect.Play();
            Destroy(effect.gameObject, effect.main.duration);
        }
    }

    // Public methods to configure hitbox from parent weapon
    public void SetWeaponDamage(float damage)
    {
        weaponDamage = damage;
    }

    public float GetWeaponDamage()
    {
        return weaponDamage;
    }

    public void SetHitEffect(ParticleSystem effect)
    {
        hitEffect = effect;
    }

    public void SetHitSound(AudioClip sound, AudioSource source = null)
    {
        hitSound = sound;
        if (source != null)
        {
            audioSource = source;
        }
    }
}