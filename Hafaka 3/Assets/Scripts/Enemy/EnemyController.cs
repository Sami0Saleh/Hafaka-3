using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour,IDamageable
{
    [Header("References")]
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private GameObject enemyModel;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject leftHandHitbox;
    [SerializeField] private GameObject rightHandHitbox;

    [Header("Movement Settings")]
    [SerializeField] private float patrolSpeed = 3.5f;
    [SerializeField] private float chaseSpeed = 6f;
    [SerializeField] private float decelerationRate = 2f;
    [SerializeField] private float accelerationRate = 3f;

    [Header("Detection Settings")]
    [SerializeField] private float detectionDistance = 15f;
    [SerializeField] private float attackDistance = 5f;
    [SerializeField] private float attackCooldown = 1.5f;

    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 10;
    [SerializeField] private float fadeOutDuration = 2f;
    [SerializeField] private float respawnDelay = 3f;

    private NavMeshAgent navMeshAgent;
    private EnemyState enemyState;
    private int currentPatrolIndex = 0;
    private bool isAttacking = false;
    private bool isNight = false;
    private int currentHealth;
    private bool useLeftHand = true;
    private float currentSpeed;
    private bool missedAttack = false;
    private Vector3 oppositeDirection;
    private Material enemyMaterial;
    private float initialAttackDistance;

    private void Awake()
    {
        playerHealth = FindObjectOfType<PlayerHealth>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        enemyState = EnemyState.Idle;

        // Initialize enemy material for fade effect
  

        currentHealth = maxHealth;
        initialAttackDistance = attackDistance;

        // Disable hitboxes initially
        if (leftHandHitbox) leftHandHitbox.SetActive(false);
        if (rightHandHitbox) rightHandHitbox.SetActive(false);
    }

    private void Start()
    {
        // Select random patrol point to start
        if (patrolPoints.Length > 0)
        {
            currentPatrolIndex = Random.Range(0, patrolPoints.Length);
        }
    }

    private void Update()
    {
        if (!isNight || currentHealth <= 0) return; // Enemy only acts at night and when alive

        StateUpdate();

        // Handle acceleration/deceleration
        if (enemyState == EnemyState.Chase)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, chaseSpeed, Time.deltaTime * accelerationRate);
        }
        else if (missedAttack)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, 0, Time.deltaTime * decelerationRate);
            if (currentSpeed < 0.5f)
            {
                missedAttack = false;
                ResetAggressionFromOppositeDirection();
            }
        }

        navMeshAgent.speed = currentSpeed;
    }

    private void StateUpdate()
    {
        float distance = Vector3.Distance(playerHealth.transform.position, transform.position);

        switch (enemyState)
        {
            case EnemyState.Idle:
                // Transition to wandering
                enemyState = EnemyState.Wandering;
                Patrol();
                break;

            case EnemyState.Wandering:
                // Check if patrol destination reached
                if (navMeshAgent.remainingDistance < 0.5f)
                {
                    currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
                    navMeshAgent.SetDestination(patrolPoints[currentPatrolIndex].position);
                }

                // Detect player
                if (distance <= detectionDistance)
                {
                    enemyState = EnemyState.Chase;
                    currentSpeed = patrolSpeed; // Start acceleration from current speed
                }
                break;

            case EnemyState.Chase:
                // Update destination to player position
                navMeshAgent.SetDestination(playerHealth.transform.position);

                // Within attack range
                if (distance <= attackDistance && !isAttacking)
                {
                    enemyState = EnemyState.Attack;
                    StartCoroutine(Attack());
                }

                // Lost player
                if (distance > detectionDistance)
                {
                    enemyState = EnemyState.Wandering;
                    Patrol();
                }
                break;

            case EnemyState.Attack:
                // Handled in the Attack coroutine
                if (!isAttacking)
                {
                    if (distance > attackDistance)
                    {
                        enemyState = EnemyState.Chase;
                    }
                }
                break;
        }
    }

    private void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        navMeshAgent.SetDestination(patrolPoints[currentPatrolIndex].position);
        currentSpeed = patrolSpeed;
        navMeshAgent.speed = patrolSpeed;
    }

    private IEnumerator Attack()
    {
        isAttacking = true;

        // Calculate if player is within range to actually hit
        bool canActuallyHit = Vector3.Distance(playerHealth.transform.position, transform.position) <= attackDistance;

        // Play animation
        if (animator != null)
        {
            animator.SetTrigger(useLeftHand ? "LeftAttack" : "RightAttack");
        }

        // Wait for animation to reach hit point
        yield return new WaitForSeconds(0.3f);

        // Activate appropriate hitbox
        if (useLeftHand)
        {
            if (leftHandHitbox) leftHandHitbox.SetActive(true);
        }
        else
        {
            if (rightHandHitbox) rightHandHitbox.SetActive(true);
        }

        // Check if hit connects
        if (canActuallyHit)
        {
            // Deal damage to player
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(10); // Assuming TakeDamage method exists on PlayerHealth
            }
        }
        else
        {
            // Mark as missed for deceleration
            missedAttack = true;
            oppositeDirection = transform.position - playerHealth.transform.position;
        }

        // Deactivate hitbox
        yield return new WaitForSeconds(0.1f);
        if (leftHandHitbox) leftHandHitbox.SetActive(false);
        if (rightHandHitbox) rightHandHitbox.SetActive(false);

        // Switch hands for next attack
        useLeftHand = !useLeftHand;

        // Cooldown
        yield return new WaitForSeconds(attackCooldown);

        isAttacking = false;
        enemyState = EnemyState.Chase;
    }

    private void ResetAggressionFromOppositeDirection()
    {
        // Move in the opposite direction temporarily to reset aggression
        if (oppositeDirection != Vector3.zero)
        {
            Vector3 newPosition = transform.position + oppositeDirection.normalized * 2f;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(newPosition, out hit, 5f, NavMesh.AllAreas))
            {
                transform.position = hit.position;
            }

            // Temporarily increase attack distance to prevent immediate re-attack
            StartCoroutine(ResetAttackDistance());
        }
    }

    private IEnumerator ResetAttackDistance()
    {
        attackDistance *= 1.5f;
        yield return new WaitForSeconds(2f);
        attackDistance = initialAttackDistance;
    }

    public void TakeDamage(float damage)
    {
        if (currentHealth <= 0) return;

        currentHealth -= (int)damage;

        // Play hit animation/effect  
        if (animator != null)
        {
            animator.SetTrigger("Hit");
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Stop movement
        navMeshAgent.isStopped = true;
        enemyState = EnemyState.Idle;

        // Disable colliders
        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (Collider col in colliders)
        {
            col.enabled = false;
        }

        // Play death animation
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        // Fade out
        //StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        float elapsedTime = 0;
        Color originalColor = enemyMaterial.color;

        // Gradually fade out
        while (elapsedTime < fadeOutDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeOutDuration);

            // Update material transparency
            if (enemyMaterial != null)
            {
                Color newColor = originalColor;
                newColor.a = alpha;
                enemyMaterial.color = newColor;
            }

            yield return null;
        }

        // Hide the enemy
        enemyModel.SetActive(false);

        // Wait for respawn delay
        yield return new WaitForSeconds(respawnDelay);

        // Respawn
        Respawn();
    }

    private void Respawn()
    {
        // Reset health
        currentHealth = maxHealth;

        // Choose random patrol point for respawn
        if (patrolPoints.Length > 0)
        {
            currentPatrolIndex = Random.Range(0, patrolPoints.Length);
            transform.position = patrolPoints[currentPatrolIndex].position;
        }

        // Re-enable colliders
        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (Collider col in colliders)
        {
            col.enabled = true;
        }

        // Reset material transparency
        if (enemyMaterial != null)
        {
            Color color = enemyMaterial.color;
            color.a = 1f;
            enemyMaterial.color = color;
        }

        // Show the enemy
        enemyModel.SetActive(true);

        // Reset state
        enemyState = EnemyState.Idle;
        navMeshAgent.isStopped = false;
        missedAttack = false;
    }

    public void SetNightState(bool nightActive)
    {
        isNight = nightActive;

        if (nightActive)
        {
            // Only show enemy model if it has health
            enemyModel.SetActive(currentHealth > 0);

            // If this is the first time activating at night, set initial patrol
            if (enemyState == EnemyState.Idle)
            {
                Patrol();
                enemyState = EnemyState.Wandering;
            }
        }
        else
        {
            // Hide during day
            enemyModel.SetActive(false);
        }
    }
}

public enum EnemyState
{
    Idle,
    Wandering,
    Chase,
    Attack
}