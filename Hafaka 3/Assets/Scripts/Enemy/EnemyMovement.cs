using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField]PlayerHealth playerHealth;
    NavMeshAgent navMeshAgent;
    EnemyState enemyState;

    float speed = 15f;

    float ChaseDistance = 15f;
    float AttackDistance = 5f;

    private void Awake()
    {
        playerHealth = FindAnyObjectByType<PlayerHealth>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        enemyState = EnemyState.Idle;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        StateChange();
    }
    private void StateChange()
    {
        float distance = Vector3.Distance(playerHealth.transform.position, this.transform.position);
        if (distance <= ChaseDistance&&enemyState != EnemyState.Chase)
        {
            enemyState = EnemyState.Chase;
            navMeshAgent.SetDestination(playerHealth.transform.position);
            navMeshAgent.speed = speed;
        }
        else if (distance <= AttackDistance && enemyState != EnemyState.Attack)
        {
            enemyState = EnemyState.Attack;
            //Attack
        }
        else
        {
            enemyState = EnemyState.Idle;
        }
    }
}
public enum EnemyState
{
    Idle,
    Chase,
    Attack,
    Wandering
}