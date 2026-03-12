using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class LoadingScreenController : MonoBehaviour
{
    public static string TargetScene = "Game";

    private const float MIN_DELAY = 2f;

    private RectTransform _fillRect;
    private TextMeshProUGUI _progressText;

    void Start()
    {
        GameObject camGO = new GameObject("LoadingCamera");
        Camera cam = camGO.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.08f, 0.08f, 0.12f, 1f);
        cam.cullingMask = 0;

        BuildUI();
        StartCoroutine(RunLoadSequence());
    }

    private IEnumerator RunLoadSequence()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(TargetScene);
        op.allowSceneActivation = false;

        float elapsed = 0f;

        while (true)
        {
            elapsed += Time.unscaledDeltaTime;

            float assetProgress = Mathf.Clamp01(op.progress / 0.9f);
            float timeProgress = Mathf.Clamp01(elapsed / MIN_DELAY);
            float displayProgress = Mathf.Min(assetProgress, timeProgress);

            _fillRect.anchorMax = new Vector2(displayProgress, 1f);
            _progressText.text = $"{Mathf.RoundToInt(displayProgress * 100)}%";

            if (elapsed >= MIN_DELAY && op.progress >= 0.9f)
            {
                _fillRect.anchorMax = Vector2.one;
                _progressText.text = "100%";
                yield return new WaitForSecondsRealtime(0.3f);
                op.allowSceneActivation = true;
                yield break;
            }

            yield return null;
        }
    }

    private void BuildUI()
    {
        GameObject canvasGO = new GameObject("LoadingCanvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 200;

        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        canvasGO.AddComponent<GraphicRaycaster>();

        GameObject background = new GameObject("Background");
        background.transform.SetParent(canvasGO.transform, false);
        background.AddComponent<Image>().color = new Color(0.08f, 0.08f, 0.12f, 1f);
        Stretch(background.GetComponent<RectTransform>());

        GameObject title = new GameObject("Title");
        title.transform.SetParent(canvasGO.transform, false);
        TextMeshProUGUI titleText = title.AddComponent<TextMeshProUGUI>();
        titleText.text = "CHARGEMENT";
        titleText.fontSize = 72;
        titleText.fontStyle = FontStyles.Bold;
        titleText.color = Color.white;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.textWrappingMode = TextWrappingModes.NoWrap;
        RectTransform titleRT = title.GetComponent<RectTransform>();
        titleRT.anchorMin = new Vector2(0.2f, 0.55f);
        titleRT.anchorMax = new Vector2(0.8f, 0.72f);
        titleRT.offsetMin = Vector2.zero;
        titleRT.offsetMax = Vector2.zero;

        GameObject barBackground = new GameObject("BarBackground");
        barBackground.transform.SetParent(canvasGO.transform, false);
        barBackground.AddComponent<Image>().color = new Color(0.2f, 0.2f, 0.28f, 1f);
        RectTransform barRT = barBackground.GetComponent<RectTransform>();
        barRT.anchorMin = new Vector2(0.15f, 0.44f);
        barRT.anchorMax = new Vector2(0.85f, 0.51f);
        barRT.offsetMin = Vector2.zero;
        barRT.offsetMax = Vector2.zero;

        GameObject fillGO = new GameObject("Fill");
        fillGO.transform.SetParent(barBackground.transform, false);
        fillGO.AddComponent<Image>().color = new Color(0.25f, 0.55f, 0.9f, 1f);
        _fillRect = fillGO.GetComponent<RectTransform>();
        _fillRect.anchorMin = Vector2.zero;
        _fillRect.anchorMax = Vector2.zero;
        _fillRect.offsetMin = Vector2.zero;
        _fillRect.offsetMax = Vector2.zero;

        GameObject textGO = new GameObject("Percentage");
        textGO.transform.SetParent(canvasGO.transform, false);
        _progressText = textGO.AddComponent<TextMeshProUGUI>();
        _progressText.text = "0%";
        _progressText.fontSize = 36;
        _progressText.color = new Color(0.75f, 0.75f, 0.85f, 1f);
        _progressText.alignment = TextAlignmentOptions.Center;
        _progressText.textWrappingMode = TextWrappingModes.NoWrap;
        RectTransform textRT = textGO.GetComponent<RectTransform>();
        textRT.anchorMin = new Vector2(0.15f, 0.37f);
        textRT.anchorMax = new Vector2(0.85f, 0.44f);
        textRT.offsetMin = Vector2.zero;
        textRT.offsetMax = Vector2.zero;
    }

    private static void Stretch(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }
}
