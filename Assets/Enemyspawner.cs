using UnityEngine;
using TMPro;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawning")]
    public GameObject enemyPrefab;          
    public GameObject rangedEnemyPrefab;    
    public Transform[] spawnPoints;
    public float baseSpawnInterval = 3f;
    public float minSpawnInterval = 0.5f;
    public int baseMaxEnemies = 5;
    public int maxEnemiesCap = 30;

    [Header("Wave UI")]
    public TextMeshProUGUI waveText;
    public float waveTextDuration = 2f;

    private float _spawnTimer;
    private float _currentInterval;
    private int _currentMaxEnemies;
    private int _currentWave = 0;
    private int _lastDisplayedWave = 0;

    void Start()
    {
        _currentInterval = baseSpawnInterval;
        _currentMaxEnemies = baseMaxEnemies;
        _spawnTimer = _currentInterval;

        if (waveText != null) waveText.gameObject.SetActive(false);

        ShowWave(1);
    }

    void Update()
    {
        UpdateDifficulty();

        _spawnTimer -= Time.deltaTime;
        if (_spawnTimer <= 0f)
        {
            TrySpawn();
            _spawnTimer = _currentInterval;
        }
    }

    void UpdateDifficulty()
    {
        int score = ScoreManager.Instance != null ? ScoreManager.Instance.GetScore() : 0;

        int newWave = (score / 5) + 1;
        if (newWave != _currentWave)
        {
            _currentWave = newWave;
            if (_currentWave != _lastDisplayedWave)
                ShowWave(_currentWave);
        }

        float difficultyScale = score / 5f;
        _currentInterval = Mathf.Max(minSpawnInterval, baseSpawnInterval - difficultyScale * 0.2f);
        _currentMaxEnemies = Mathf.Min(maxEnemiesCap, baseMaxEnemies + score / 3);
    }

    void TrySpawn()
    {
        EnemyHitbox[] alive = FindObjectsByType<EnemyHitbox>(FindObjectsSortMode.None);
        if (alive.Length >= _currentMaxEnemies) return;

        if (spawnPoints.Length == 0)
        {
            Debug.LogWarning("No spawn points assigned.");
            return;
        }

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

   
        bool spawnRanged = _currentWave >= 3
            && rangedEnemyPrefab != null
            && Random.value < 0.4f;

        GameObject prefabToSpawn = spawnRanged ? rangedEnemyPrefab : enemyPrefab;
        Instantiate(prefabToSpawn, spawnPoint.position, Quaternion.identity);
    }

    void ShowWave(int wave)
    {
        _lastDisplayedWave = wave;
        if (waveText != null)
            StartCoroutine(DisplayWaveText(wave));
    }

    IEnumerator DisplayWaveText(int wave)
    {
        waveText.text = $"Wave {wave}";
        waveText.gameObject.SetActive(true);
        yield return new WaitForSeconds(waveTextDuration);
        waveText.gameObject.SetActive(false);
    }
}