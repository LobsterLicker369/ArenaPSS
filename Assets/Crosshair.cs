using UnityEngine;
using UnityEngine.UI;


public class Crosshair : MonoBehaviour
{
    [Header("Crosshair Settings")]
    public Color color = Color.white;
    public int lineLength = 10;
    public int lineThickness = 2;
    public int gapSize = 4;     

    private Canvas _canvas;

    void Awake()
    {
        
        _canvas = GetComponentInParent<Canvas>();
        if (_canvas == null)
        {
            GameObject canvasObj = new GameObject("CrosshairCanvas");
            _canvas = canvasObj.AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            transform.SetParent(canvasObj.transform);
        }

        CreateLines();
    }


    //createlines sem nechal udelat ai protoze se mi to hrozne nechtelo psat :)
    void CreateLines()
    {
        // Top
        CreateLine("Top", new Vector2(0, gapSize + lineLength * 0.5f), new Vector2(lineThickness, lineLength));
        // Bottom
        CreateLine("Bottom", new Vector2(0, -(gapSize + lineLength * 0.5f)), new Vector2(lineThickness, lineLength));
        // Left
        CreateLine("Left", new Vector2(-(gapSize + lineLength * 0.5f), 0), new Vector2(lineLength, lineThickness));
        // Right
        CreateLine("Right", new Vector2(gapSize + lineLength * 0.5f, 0), new Vector2(lineLength, lineThickness));
    }

    void CreateLine(string lineName, Vector2 anchoredPos, Vector2 size)
    {
        GameObject obj = new GameObject(lineName);
        obj.transform.SetParent(transform, false);

        RectTransform rt = obj.AddComponent<RectTransform>();
        rt.sizeDelta = size;
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = anchoredPos;

        Image img = obj.AddComponent<Image>();
        img.color = color;
    }
}