using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    // State Enum
    public enum EnemyState { Patrol, Chase, Attack, ReceiveHit, Death }
    public EnemyState enemyState = EnemyState.Patrol;

    // Components
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private Animator _animator;
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private GameObject _enemy;
    //private EnemyHealthBar _enemyHealthBar;

    // Configurable Parameters
    [Header("Vision Settings")]
    [SerializeField] private float _visionRange = 10f;
    [SerializeField] private float _visionAngle = 60f;

    [Header("Attack Settings")]
    [SerializeField] private float _attackRange = 2f;
    [SerializeField] private float _attackDelay = 1f;
    [SerializeField] private int _attackDamage = 1;

    [Header("Patrol Settings")]
    [SerializeField] private Transform[] _patrolPoints;
    [SerializeField] private int _currentPatrolIndex = 0;

    [Header("Health Settings")]
    [SerializeField] private int _maxHealth = 100;
    [SerializeField] private int _currentHealth;

    // State Flags
    private bool _isDead = false;
    private float lastHitTime = 0;
    private float hitCooldown = 1f;

    public bool IsDead { get => _isDead; protected set => _isDead = value; }
    
    private void Start()
    {
        DayNightCycle.instance.OnDayStart += On_DayChange;
        DayNightCycle.instance.OnNightStart += On_NightChange;

        _currentHealth = _maxHealth;
    }

    void On_DayChange()
    {
        _enemy.SetActive(false);
    }
    void On_NightChange()
    {
       _enemy.SetActive(true);
    }

    // State Machine
    private void Update()
    {
        if (_isDead) return;

        switch (enemyState)
        {
            case EnemyState.Patrol:
                Patrol();
                break;
            case EnemyState.Chase:
                Chase();
                break;
            case EnemyState.Attack:
                Attack();
                break;
            case EnemyState.ReceiveHit:
                ReceiveHit();
                break;
            case EnemyState.Death:
                Death();
                break;
        }
    }

    // State Methods
    private void Patrol()
    {
        if (_patrolPoints.Length == 0) return;

        _agent.isStopped = false;
        _agent.SetDestination(_patrolPoints[_currentPatrolIndex].position);
        float distance = Vector3.Distance(transform.position, _patrolPoints[_currentPatrolIndex].position);
        _animator.SetBool("IsAttacking", false);
        _animator.SetBool("IsWalking", true);
        if (distance < 0.5f)
        {
            _currentPatrolIndex = _currentPatrolIndex + 1;
            if (_currentPatrolIndex >= _patrolPoints.Length)
            {
                _currentPatrolIndex = 0;
            }
        }

        if (CanSeePlayer())
        {
            ChangeState(EnemyState.Chase);
        }
    }

    private void Chase()
    {
        _agent.isStopped = false;
        _agent.SetDestination(_playerTransform.position);
        _animator.SetBool("IsAttacking", false);
        _animator.SetBool("IsWalking", true);
        if (Vector3.Distance(transform.position, _playerTransform.position) <= _attackRange)
        {
            ChangeState(EnemyState.Attack);
            _animator.SetBool("IsWalking", false);
        }
        else if (!CanSeePlayer())
        {
            ChangeState(EnemyState.Patrol);
            _animator.SetBool("IsWalking", true);
        }
    }
        
    private void Attack()
    {
        _agent.isStopped = true;
        if (Vector3.Distance(transform.position, _playerTransform.position) <= _attackRange)
        {
            _animator.SetBool("IsAttacking", true);

            // Deal damage to the player here
            Debug.Log("Melee attack hit the player!");
        }

        // Return to appropriate state based on player distance
        if (Vector3.Distance(transform.position, _playerTransform.position) > _attackRange)
        {
            ChangeState(EnemyState.Chase);
        }
    }  

    private void ReceiveHit()
    {
        _animator.SetBool("IsWalking", false);
        if (_currentHealth <= 0)
        {
            ChangeState(EnemyState.Death);
        }
        else
        {
            ChangeState(EnemyState.Chase);
        }
    }

    private void Death()
    {
        _agent.isStopped = true;
        _animator.SetBool("IsWalking", false);
        _isDead = true;
        SpawnOnDeath();
    }

    // Utility Methods
    public void TakeDamage(int amount)
    {
        if (_isDead) return;

        _currentHealth -= amount;
        //enemyHealthBar.UpdateHealthBar(_currentHealth, _maxHealth);

        ChangeState(EnemyState.ReceiveHit);
    }

    private bool CanSeePlayer()
    {
        Vector3 directionToPlayer = (_playerTransform.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);

        if (angle < _visionAngle / 2f && Vector3.Distance(transform.position, _playerTransform.position) <= _visionRange)
        {
            return true;
        }
        return false;
    }

    private void SpawnOnDeath()
    {
        int index = Random.Range(0, _patrolPoints.Length);
        transform.position = _patrolPoints[index].position;
        _currentHealth = _maxHealth;
        _isDead = false;
        ChangeState(EnemyState.Patrol);
    }

    private void ChangeState(EnemyState newState)
    {
        enemyState = newState;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Ignore hits that occur too close to the last hit
        if (Time.time - lastHitTime < hitCooldown)
            return;

        if (other.CompareTag("MultiTool"))
        {
            TakeDamage(PlayerController.Instance.AttackDamage);
        }

        lastHitTime = Time.time;
    }

    private void OnDrawGizmosSelected()
    {
        if (_playerTransform == null) return;

        // Set gizmo color for the vision range
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f);  // Orange with some transparency
        Gizmos.DrawWireSphere(transform.position, _visionRange);  // Draw vision range

        // Set gizmo color for the vision cone
        Gizmos.color = Color.yellow;

        Vector3 forward = transform.forward * _visionRange;
        Vector3 leftBoundary = Quaternion.Euler(0, -_visionAngle / 2f, 0) * forward;
        Vector3 rightBoundary = Quaternion.Euler(0, _visionAngle / 2f, 0) * forward;

        // Draw the vision cone boundaries
        Gizmos.DrawRay(transform.position, leftBoundary);
        Gizmos.DrawRay(transform.position, rightBoundary);

        // Optional: Draw a line to the player if they are within range
        if (CanSeePlayer())
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, _playerTransform.position);
        }
    }

}