using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(NavMeshAgent))]
public class RangedEnemy : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 3f;
    public float stopDistance = 10f;    

    [Header("Shooting")]
    public GameObject projectilePrefab;
    public Transform firePoint;         
    public float fireRate = 2f;         //v sekundach btw

    private NavMeshAgent _agent;
    private Transform _player;
    private float _fireTimer;

    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.speed = moveSpeed;
    }

    void Start()
    {
        GameObject p = GameObject.FindWithTag("Player");
        if (p != null) _player = p.transform;

        _fireTimer = 0f; 
    }

    void Update()
    {
        if (_player == null) return;

        float dist = Vector3.Distance(transform.position, _player.position);

        if (dist > stopDistance)
        {
            //pohyb k hraci
            _agent.isStopped = false;
            _agent.SetDestination(_player.position);
        }
        else
        {
            //zastavit
            _agent.isStopped = true;

            //koukat na hrace proste tohle se bude hodit az zmenim design
            Vector3 dir = (_player.position - transform.position);
            dir.y = 0f;
            if (dir != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(dir);

            _fireTimer -= Time.deltaTime;
            if (_fireTimer <= 0f)
            {
                Shoot();
                _fireTimer = fireRate;
            }
        }
    }

    void Shoot()
    {
        if (projectilePrefab == null) return;

        Transform origin = firePoint != null ? firePoint : transform;
        Vector3 dirToPlayer = (_player.position - origin.position).normalized;

        GameObject proj = Instantiate(projectilePrefab, origin.position, Quaternion.identity);
        proj.GetComponent<EnemyProjectile>()?.SetDirection(dirToPlayer);
    }
}