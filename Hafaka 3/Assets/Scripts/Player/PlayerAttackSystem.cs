using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerAttackSystem : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject toolHitbox;
    [SerializeField] private Transform toolTransform;
    [SerializeField] private ParticleSystem hitEffect;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip hitSound;

    [Header("Attack Settings")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackAngle = 60f;

    private PlayerController playerController;
    private int lastAttackCounter = -1;
    private bool isAttacking = false;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        // Check if attack counter changed
        if (lastAttackCounter != playerController.AttackCounter && !isAttacking)
        {
            lastAttackCounter = playerController.AttackCounter;
            StartCoroutine(PerformAttack());
        }
    }

    private IEnumerator PerformAttack()
    {
        isAttacking = true;

        // Wait for the animation to reach the point where the tool hits
        yield return new WaitForSeconds(0.2f); // Adjust based on your animation timing

        // Activate hitbox
        if (toolHitbox != null)
        {
            toolHitbox.SetActive(true);
        }

        // Perform hit detection
        DetectHits();

        // Keep hitbox active for a short duration
        yield return new WaitForSeconds(0.1f);

        // Deactivate hitbox
        if (toolHitbox != null)
        {
            toolHitbox.SetActive(false);
        }

        // Wait before allowing another attack
        yield return new WaitForSeconds(0.3f);

        isAttacking = false;
    }

    private void DetectHits()
    {
        // Use Physics.OverlapSphere for a spherical attack area
        Collider[] hitColliders = Physics.OverlapSphere(toolTransform.position, attackRange);

        foreach (Collider hitCollider in hitColliders)
        {
            // Check if hit object is an enemy
            var enemy = hitCollider.GetComponent<EnemyController>();
            if (enemy != null)
            {
                // Direction to the enemy
                Vector3 directionToEnemy = (hitCollider.transform.position - transform.position).normalized;

                // Check if enemy is in front of the player (within attack angle)
                float angle = Vector3.Angle(transform.forward, directionToEnemy);
                if (angle <= attackAngle / 2)
                {
                    // Apply damage (use player's attack damage)
                    enemy.TakeDamage(playerController.AttackDamage);

                    // Play hit effects
                    PlayHitEffect(hitCollider.transform.position);
                }
            }
        }
    }

    private void PlayHitEffect(Vector3 hitPosition)
    {
        // Play hit sound
        if (audioSource != null && hitSound != null)
        {
            audioSource.PlayOneShot(hitSound);
        }

        // Spawn hit particle effect
        if (hitEffect != null)
        {
            ParticleSystem effect = Instantiate(hitEffect, hitPosition, Quaternion.identity);
            effect.Play();
            Destroy(effect.gameObject, effect.main.duration);
        }
    }

    // Visualize the attack range in the editor
    private void OnDrawGizmosSelected()
    {
        if (toolTransform != null)
        {
            // Draw attack sphere
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(toolTransform.position, attackRange);

            // Draw attack angle
            Gizmos.color = Color.yellow;
            Vector3 rightDir = Quaternion.Euler(0, attackAngle / 2, 0) * transform.forward;
            Vector3 leftDir = Quaternion.Euler(0, -attackAngle / 2, 0) * transform.forward;
            Gizmos.DrawRay(transform.position, rightDir * attackRange);
            Gizmos.DrawRay(transform.position, leftDir * attackRange);
        }
    }
}