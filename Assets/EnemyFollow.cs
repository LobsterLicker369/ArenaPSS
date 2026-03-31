using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyFollow : MonoBehaviour
{
    public float moveSpeed = 6f;
    private NavMeshAgent _agent;
    private Transform _target;

    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.speed = moveSpeed;
        _target = GameObject.FindWithTag("Player")?.transform;
    }

    void Update()
    {
        if (_target != null)
            _agent.SetDestination(_target.position);
    }
}