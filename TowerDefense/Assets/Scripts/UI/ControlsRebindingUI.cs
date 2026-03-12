using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ControlsRebindingUI : MonoBehaviour
{
    private Transform _container;

    private class RebindEntry
    {
        public KeyBindingManager.ActionType Action;
        public TextMeshProUGUI KeyDisplay;
        public Button RebindButton;
    }

    private readonly List<RebindEntry> _entries = new List<RebindEntry>();
    private RebindEntry _currentEntry;
    private float _timeout;
    private const float TIMEOUT = 5f;

    public void SetRebindingContainer(Transform container)
    {
        _container = container;
        RectTransform rt = container as RectTransform;
        if (rt != null)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }
        BuildUI();
    }

    public void RefreshUI() => BuildUI();

    void Start()
    {
        if (_container == null) return;
        BuildUI();
    }

    void Update()
    {
        if (_currentEntry == null) return;

        _timeout -= Time.unscaledDeltaTime;
        if (_timeout <= 0f) { CancelRebind(); return; }

        if (!Input.anyKeyDown) return;

        foreach (KeyCode k in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (!Input.GetKeyDown(k)) continue;
            if (k == KeyCode.Escape || k == KeyCode.Return) { CancelRebind(); return; }
            FinishRebind(k);
            return;
        }
    }

    private void BuildUI()
    {
        if (_container == null || KeyBindingManager.Instance == null) return;

        foreach (Transform child in _container) Destroy(child.gameObject);
        _entries.Clear();

        AddTitle("KEYBOARD CONTROLS — PLAYER 1");

        (KeyBindingManager.ActionType, string)[] actions =
        {
            (KeyBindingManager.ActionType.Move_Up,    "Up"),
            (KeyBindingManager.ActionType.Move_Down,  "Down"),
            (KeyBindingManager.ActionType.Move_Left,  "Left"),
            (KeyBindingManager.ActionType.Move_Right, "Right"),
            (KeyBindingManager.ActionType.PlaceTower, "Place Tower"),
            (KeyBindingManager.ActionType.Interact,   "Interact"),
            (KeyBindingManager.ActionType.LaunchWave, "Launch Wave"),
        };

        foreach (var (action, label) in actions)
            AddRow(action, label);

        LayoutRebuilder.ForceRebuildLayoutImmediate(_container as RectTransform);
    }

    private void AddTitle(string text)
    {
        GameObject go = new GameObject("Title");
        go.transform.SetParent(_container, false);

        TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 22;
        tmp.fontStyle = FontStyles.Bold;
        tmp.color = Color.white;
        tmp.alignment = TextAlignmentOptions.Left;
        tmp.textWrappingMode = TextWrappingModes.NoWrap;

        LayoutElement le = go.AddComponent<LayoutElement>();
        le.preferredHeight = 40;
        le.flexibleWidth = 1;

        SetAnchorStretchH(go.GetComponent<RectTransform>());
    }

    private void AddRow(KeyBindingManager.ActionType action, string label)
    {
        GameObject rowGO = new GameObject($"{action}_Row");
        rowGO.transform.SetParent(_container, false);

        HorizontalLayoutGroup hlg = rowGO.AddComponent<HorizontalLayoutGroup>();
        hlg.spacing = 10;
        hlg.childForceExpandWidth = false;
        hlg.childForceExpandHeight = true;
        hlg.padding = new RectOffset(0, 0, 4, 4);

        LayoutElement rowLE = rowGO.AddComponent<LayoutElement>();
        rowLE.preferredHeight = 52;
        rowLE.flexibleWidth = 1;

        SetAnchorStretchH(rowGO.GetComponent<RectTransform>());

        TextMeshProUGUI labelTMP = CreateText(rowGO, label, 20, TextAlignmentOptions.Left);
        LayoutElement labelLE = labelTMP.gameObject.AddComponent<LayoutElement>();
        labelLE.flexibleWidth = 1;
        labelLE.minWidth = 120;

        GameObject displayGO = new GameObject("Display");
        displayGO.transform.SetParent(rowGO.transform, false);
        displayGO.AddComponent<Image>().color = new Color(0.15f, 0.15f, 0.25f, 1f);

        LayoutElement displayLE = displayGO.AddComponent<LayoutElement>();
        displayLE.minWidth = 110;
        displayLE.preferredWidth = 110;

        KeyBindingManager.KeyBinding binding = KeyBindingManager.Instance.GetBinding(action);
        TextMeshProUGUI keyTMP = CreateText(displayGO, KeyBindingManager.GetKeyDisplayName(binding.KeyboardKey), 18, TextAlignmentOptions.Center);
        SetAnchorStretch(keyTMP.GetComponent<RectTransform>());

        GameObject btnGO = new GameObject("RebindBtn");
        btnGO.transform.SetParent(rowGO.transform, false);
        btnGO.AddComponent<Image>().color = new Color(0.25f, 0.5f, 0.75f, 1f);

        Button btn = btnGO.AddComponent<Button>();
        ColorBlock cb = btn.colors;
        cb.highlightedColor = new Color(0.35f, 0.65f, 0.9f, 1f);
        cb.pressedColor = new Color(0.15f, 0.35f, 0.6f, 1f);
        btn.colors = cb;

        LayoutElement btnLE = btnGO.AddComponent<LayoutElement>();
        btnLE.minWidth = 90;
        btnLE.preferredWidth = 90;

        CreateText(btnGO, "REBIND", 16, TextAlignmentOptions.Center);

        RebindEntry entry = new RebindEntry
        {
            Action = action,
            KeyDisplay = keyTMP,
            RebindButton = btn,
        };
        _entries.Add(entry);
        btn.onClick.AddListener(() => StartRebind(entry));
    }

    private TextMeshProUGUI CreateText(GameObject parent, string text, float size, TextAlignmentOptions align)
    {
        GameObject go = new GameObject("Text");
        go.transform.SetParent(parent.transform, false);

        TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = size;
        tmp.color = Color.white;
        tmp.alignment = align;
        tmp.textWrappingMode = TextWrappingModes.NoWrap;

        SetAnchorStretch(go.GetComponent<RectTransform>());
        return tmp;
    }

    private static void SetAnchorStretch(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    private static void SetAnchorStretchH(RectTransform rt)
    {
        rt.anchorMin = new Vector2(0f, 0.5f);
        rt.anchorMax = new Vector2(1f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
    }

    private void StartRebind(RebindEntry entry)
    {
        foreach (var e in _entries) e.RebindButton.interactable = false;
        _currentEntry = entry;
        _timeout = TIMEOUT;
        entry.KeyDisplay.text = "...";
    }

    private void FinishRebind(KeyCode key)
    {
        if (_currentEntry == null) return;
        KeyBindingManager.Instance.SetBinding(_currentEntry.Action, key);
        _currentEntry.KeyDisplay.text = KeyBindingManager.GetKeyDisplayName(key);
        EndRebind();
    }

    private void CancelRebind()
    {
        if (_currentEntry == null) return;
        KeyBindingManager.KeyBinding b = KeyBindingManager.Instance.GetBinding(_currentEntry.Action);
        _currentEntry.KeyDisplay.text = KeyBindingManager.GetKeyDisplayName(b.KeyboardKey);
        EndRebind();
    }

    private void EndRebind()
    {
        _currentEntry = null;
        foreach (var e in _entries) e.RebindButton.interactable = true;
    }
}
