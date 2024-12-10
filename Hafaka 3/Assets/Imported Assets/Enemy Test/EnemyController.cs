using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [SerializeField] Transform _playerTransfrom;
    [SerializeField] NavMeshAgent _navMeshAgent;
    void Start()
    {
        _navMeshAgent.SetDestination(_playerTransfrom.position);
    }
    void Update()
    {
        _navMeshAgent.SetDestination(_playerTransfrom.position);
        if (_navMeshAgent.remainingDistance < 5) { Debug.Log("Enemy Is Attacking"); }
    }
}
