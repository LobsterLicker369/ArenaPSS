using UnityEngine;
using UnityEngine.Events;

public class EnemyHitbox : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 100;

    [Header("Events")]
    public UnityEvent onDeath;             
    public UnityEvent<int> onDamageTaken;  

    private int _currentHealth;

    void Awake()
    {
        _currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {

        if (_currentHealth <= 0) return;
        GetComponent<EnemyHitFlash>()?.Flash();
        _currentHealth -= amount;
        Debug.Log($"{gameObject.name} took {amount} damage. HP: {_currentHealth}/{maxHealth}");

        onDamageTaken?.Invoke(_currentHealth);

        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.AddScore(1);

        Destroy(gameObject);
    }
}