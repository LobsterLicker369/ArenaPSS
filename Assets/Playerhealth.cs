using UnityEngine;


public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth Instance;

    [Header("Health")]
    public int maxHealth = 100;

    private int _currentHealth;

    void Awake()
    {
        Instance = this;
        _currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        _currentHealth -= amount;
        Debug.Log($"Player took {amount} damage. HP: {_currentHealth}");

        if (_currentHealth <= 0)
            Die();
    }

    void Die()
    {
        GameOverScreen.Instance?.Show();
    }
}
