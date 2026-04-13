using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    private GameObject _difficultyPanel;
    private CanvasGroup _difficultyGroup;
    private Transform _difficultyInnerPanel;

    void Start()
    {
        BuildDifficultyPanel();
        AudioManager.Instance?.PlayMusic(MusicTrack.Menu, 1.5f);
    }

    public void OnPlayPressed()
    {
        AudioManager.Instance?.PlaySFX(SFXType.UIClick);
        if (_difficultyPanel == null) return;
        _difficultyPanel.SetActive(true);
        if (_difficultyGroup != null)
        {
            _difficultyGroup.alpha = 0f;
            UITween.FadeTo(_difficultyGroup, 1f, 0.3f);
        }
        if (_difficultyInnerPanel != null)
        {
            _difficultyInnerPanel.localScale = Vector3.one * 0.6f;
            UITween.ScaleTo(_difficultyInnerPanel, Vector3.one, 0.45f, Easing.Ease.EaseOutBack);
        }
    }

    public void OnQuitPressed()
    {
        AudioManager.Instance?.PlaySFX(SFXType.UIBack);
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void HideDifficulty()
    {
        if (_difficultyPanel == null) return;
        if (_difficultyGroup != null)
            UITween.FadeTo(_difficultyGroup, 0f, 0.2f);
        _difficultyPanel.SetActive(false);
    }

    private void SelectDifficulty(GameManager.DifficultyLevel difficulty)
    {
        AudioManager.Instance?.PlaySFX(SFXType.UIClick);
        HideDifficulty();
        if (GameManager.Instance == null)
        {
            Debug.LogError("[MainMenu] GameManager not found!");
            return;
        }
        GameManager.Instance.StartGame(difficulty);
    }

    private void BuildDifficultyPanel()
    {
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null) return;

        _difficultyPanel = new GameObject("DifficultyPanel");
        _difficultyPanel.transform.SetParent(canvas.transform, false);
        _difficultyGroup = _difficultyPanel.AddComponent<CanvasGroup>();

        Image overlay = _difficultyPanel.AddComponent<Image>();
        overlay.color = new Color(0f, 0f, 0f, 0.85f);
        RectTransform overlayRT = _difficultyPanel.GetComponent<RectTransform>();
        overlayRT.anchorMin = Vector2.zero;
        overlayRT.anchorMax = Vector2.one;
        overlayRT.offsetMin = Vector2.zero;
        overlayRT.offsetMax = Vector2.zero;

        GameObject panel = new GameObject("Panel");
        _difficultyInnerPanel = panel.transform;
        panel.transform.SetParent(_difficultyPanel.transform, false);
        panel.AddComponent<Image>().color = new Color(0.12f, 0.12f, 0.18f, 1f);
        RectTransform panelRT = panel.GetComponent<RectTransform>();
        panelRT.anchorMin = new Vector2(0.5f, 0.5f);
        panelRT.anchorMax = new Vector2(0.5f, 0.5f);
        panelRT.pivot = new Vector2(0.5f, 0.5f);
        panelRT.sizeDelta = new Vector2(400, 300);
        panelRT.anchoredPosition = Vector2.zero;

        VerticalLayoutGroup vlg = panel.AddComponent<VerticalLayoutGroup>();
        vlg.spacing = 16;
        vlg.padding = new RectOffset(30, 30, 30, 30);
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;
        vlg.childAlignment = TextAnchor.UpperCenter;

        GameObject title = new GameObject("Title");
        title.transform.SetParent(panel.transform, false);
        TextMeshProUGUI titleTMP = title.AddComponent<TextMeshProUGUI>();
        titleTMP.text = "SELECT DIFFICULTY";
        titleTMP.fontSize = 32;
        titleTMP.fontStyle = FontStyles.Bold;
        titleTMP.alignment = TextAlignmentOptions.Center;
        titleTMP.color = Color.white;
        titleTMP.textWrappingMode = TextWrappingModes.NoWrap;
        LayoutElement titleLE = title.AddComponent<LayoutElement>();
        titleLE.preferredHeight = 60;
        titleLE.flexibleWidth = 1;

        CreateButton(panel, "Easy", new Color(0.15f, 0.5f, 0.15f, 1f),
            () => SelectDifficulty(GameManager.DifficultyLevel.Easy));
        CreateButton(panel, "Hard", new Color(0.65f, 0.15f, 0.15f, 1f),
            () => SelectDifficulty(GameManager.DifficultyLevel.Hard));
        CreateButton(panel, "Back", new Color(0.3f, 0.3f, 0.3f, 1f),
            () => { AudioManager.Instance?.PlaySFX(SFXType.UIBack); HideDifficulty(); });

        _difficultyPanel.SetActive(false);
    }

    private void CreateButton(GameObject parent, string label, Color color, UnityEngine.Events.UnityAction callback)
    {
        GameObject go = new GameObject(label);
        go.transform.SetParent(parent.transform, false);
        go.AddComponent<Image>().color = color;
        Button btn = go.AddComponent<Button>();
        ColorBlock cb = btn.colors;
        cb.highlightedColor = color * 1.2f;
        cb.pressedColor = color * 0.8f;
        btn.colors = cb;
        btn.onClick.AddListener(callback);

        UIButtonFeedback fb = go.AddComponent<UIButtonFeedback>();
        fb.Init();

        LayoutElement le = go.AddComponent<LayoutElement>();
        le.preferredHeight = 55;
        le.flexibleWidth = 1;

        GameObject text = new GameObject("Text");
        text.transform.SetParent(go.transform, false);
        TextMeshProUGUI tmp = text.AddComponent<TextMeshProUGUI>();
        tmp.text = label;
        tmp.fontSize = 26;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        tmp.textWrappingMode = TextWrappingModes.NoWrap;
        RectTransform rt = text.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }
}
