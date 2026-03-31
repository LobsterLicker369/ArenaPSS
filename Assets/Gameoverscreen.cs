using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOverScreen : MonoBehaviour
{
    public static GameOverScreen Instance;

    [Header("UI")]
    public GameObject panel;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI highscoreText;

    [Header("Buttons")]
    public Button exitToMenuButton;
    public Button quitGameButton;

    void Awake()
    {
        Instance = this;
        if (panel != null) panel.SetActive(false);
    }

    void Start()
    {
        exitToMenuButton?.onClick.AddListener(OnExitToMenu);
        quitGameButton?.onClick.AddListener(OnQuit);
    }

    public void Show()
    {
        if (panel != null) panel.SetActive(true);

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        int score = ScoreManager.Instance != null ? ScoreManager.Instance.GetScore() : 0;
        int hs = PlayerPrefs.GetInt("Highscore", 0);

        if (finalScoreText != null) finalScoreText.text = $"Score: {score}";
        if (highscoreText != null) highscoreText.text = $"Best: {hs}";
    }

    void OnExitToMenu()
    {
        Time.timeScale = 1f;
        GameManager.Instance?.GoToMenu();
    }

    void OnQuit()
    {
        GameManager.Instance?.QuitGame();
    }
}
