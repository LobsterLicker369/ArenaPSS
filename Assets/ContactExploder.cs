using UnityEngine;


public class ContactExploder : MonoBehaviour
{
    [Header("Settings")]
    public int damage = 100;
    public float explodeDistance = 1.5f;

    private Transform _player;

    void Start()
    {
        GameObject p = GameObject.FindWithTag("Player");
        if (p != null) _player = p.transform;
    }

    void Update()
    {
        if (_player == null) return;

        float dist = Vector3.Distance(transform.position, _player.position);
        if (dist <= explodeDistance)
            Explode();
    }

    void Explode()
    {
        if (PlayerHealth.Instance != null)
            PlayerHealth.Instance.TakeDamage(damage);

   
        Destroy(gameObject);
    }
}
