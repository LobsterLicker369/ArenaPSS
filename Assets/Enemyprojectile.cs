using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyProjectile : MonoBehaviour
{
    [Header("Settings")]
    public float speed = 10f;
    public float lifetime = 5f;

    private Rigidbody _rb;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = false;
        Destroy(gameObject, lifetime);
    }

    public void SetDirection(Vector3 dir)
    {
        _rb.linearVelocity = dir.normalized * speed;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth.Instance?.TakeDamage(100);
            Destroy(gameObject);
        }
    }
}