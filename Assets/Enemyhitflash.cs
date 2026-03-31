using UnityEngine;
using System.Collections;


public class EnemyHitFlash : MonoBehaviour
{
    [Header("Settings")]
    public Renderer enemyRenderer;  
    public Color flashColor = Color.red;
    public float flashDuration = 0.1f;

    private Color _originalColor;
    private Coroutine _flashRoutine;

    void Awake()
    {
        if (enemyRenderer == null)
            enemyRenderer = GetComponentInChildren<Renderer>();

        if (enemyRenderer != null)
            _originalColor = enemyRenderer.material.color;
    }

    public void Flash()
    {
        if (enemyRenderer == null) return;

        if (_flashRoutine != null)
            StopCoroutine(_flashRoutine);

        _flashRoutine = StartCoroutine(DoFlash());
    }

    IEnumerator DoFlash()
    {
        enemyRenderer.material.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        enemyRenderer.material.color = _originalColor;
    }
}