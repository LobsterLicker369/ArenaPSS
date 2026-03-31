using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [Header("Buttons")]
    public Button playButton;
    public Button controlsButton;
    public Button quitButton;

    [Header("Panels")]
    public GameObject controlsPanel;    

    [Header("UI")]
    public TextMeshProUGUI highscoreText;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

       
        int hs = PlayerPrefs.GetInt("Highscore", 0);
        if (highscoreText != null)
            highscoreText.text = $"Best: {hs}";

     
        if (controlsPanel != null)
            controlsPanel.SetActive(false);

      
        playButton?.onClick.AddListener(OnPlay);
        controlsButton?.onClick.AddListener(OnControls);
        quitButton?.onClick.AddListener(OnQuit);
    }

    void OnPlay()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.StartGame();
    }

    void OnControls()
    {
        if (controlsPanel != null)
            controlsPanel.SetActive(!controlsPanel.activeSelf);
    }

    void OnQuit()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.QuitGame();
    }
}