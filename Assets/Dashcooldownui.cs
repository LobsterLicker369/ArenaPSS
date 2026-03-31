using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class DashCooldownUI : MonoBehaviour
{
    [Header("UI")]
    public Image fillImage;

    private FPSController _fpsController;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
     
        StartCoroutine(FindControllerNextFrame());
    }

    void Start()
    {
        _fpsController = FindFirstObjectByType<FPSController>(FindObjectsInactive.Include);
    }

    IEnumerator FindControllerNextFrame()
    {
        yield return null; 
        _fpsController = FindFirstObjectByType<FPSController>(FindObjectsInactive.Include);
    }

    void Update()
    {
        if (_fpsController == null || fillImage == null) return;

        float cooldown = _fpsController.dashCooldown;
        float remaining = Mathf.Max(0f, _fpsController.DashCooldownRemaining);
        fillImage.fillAmount = 1f - (remaining / cooldown);
    }
}