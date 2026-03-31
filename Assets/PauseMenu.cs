using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [Header("UI")]
    public GameObject pausePanel;

    [Header("Buttons")]
    public Button exitToMenuButton;
    public Button quitGameButton;

    private bool _isPaused;
    private PlayerInput _input;
    private InputAction _pause;

    void Awake()
    {
        _input = FindFirstObjectByType<PlayerInput>();
    }

    void OnEnable()
    {
        _pause = _input?.actions.FindAction("Pause", false);
        _pause?.Enable();
    }

    void Start()
    {
        if (pausePanel != null) pausePanel.SetActive(false);

        exitToMenuButton?.onClick.AddListener(OnExitToMenu);
        quitGameButton?.onClick.AddListener(OnQuit);
    }

    void Update()
    {
        if (_pause != null && _pause.triggered)
            TogglePause();
    }

    void TogglePause()
    {
        _isPaused = !_isPaused;
        pausePanel?.SetActive(_isPaused);
        Time.timeScale = _isPaused ? 0f : 1f;
        Cursor.lockState = _isPaused ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = _isPaused;
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