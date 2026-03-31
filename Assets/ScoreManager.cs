using UnityEngine;
using TMPro;


public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highscoreText;

    private int _score;
    private int _highscore;

    const string HIGHSCORE_KEY = "Highscore";

    void Awake()
    {
        Instance = this;
        _highscore = PlayerPrefs.GetInt(HIGHSCORE_KEY, 0);
    }

    void Start()
    {
        UpdateUI();
    }

    public void AddScore(int amount = 1)
    {
        _score += amount;

        if (_score > _highscore)
        {
            _highscore = _score;
            PlayerPrefs.SetInt(HIGHSCORE_KEY, _highscore);
            PlayerPrefs.Save();
        }

        UpdateUI();
    }

    public int GetScore() => _score;

    public void ResetScore()
    {
        _score = 0;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (scoreText != null) scoreText.text = $"Score: {_score}";
        if (highscoreText != null) highscoreText.text = $"Best: {_highscore}";
    }
}