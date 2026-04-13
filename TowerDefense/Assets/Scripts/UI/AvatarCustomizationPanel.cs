using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AvatarCustomizationPanel : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private int playerIndex = 1;
    [SerializeField] private AvatarCustomizer previewCustomizer;

    [Header("Class")]
    [SerializeField] private Button[] classButtons;
    [SerializeField] private AvatarClass[] classForButton;

    [Header("Primary color")]
    [SerializeField] private Button[] colorButtons;

    [Header("Secondary tint sliders")]
    [SerializeField] private Slider tintR;
    [SerializeField] private Slider tintG;
    [SerializeField] private Slider tintB;

    [Header("Transform")]
    [SerializeField] private Slider scaleSlider;
    [SerializeField] private Toggle flipToggle;

    [Header("Label")]
    [SerializeField] private TMP_Text summaryText;

    private AvatarProfile _profile;

    void Start()
    {
        _profile = AvatarProfile.LoadForPlayer(playerIndex);
        BindUI();
        Refresh();
    }

    private void BindUI()
    {
        for (int i = 0; i < (classButtons != null ? classButtons.Length : 0); i++)
        {
            int idx = i;
            if (classButtons[idx] != null)
                classButtons[idx].onClick.AddListener(() => OnClassSelected(idx));
        }

        for (int i = 0; i < (colorButtons != null ? colorButtons.Length : 0); i++)
        {
            int idx = i;
            if (colorButtons[idx] != null)
                colorButtons[idx].onClick.AddListener(() => OnColorSelected(idx));
        }

        if (tintR != null) tintR.onValueChanged.AddListener(v => { _profile.secondaryTint.r = v; Refresh(); });
        if (tintG != null) tintG.onValueChanged.AddListener(v => { _profile.secondaryTint.g = v; Refresh(); });
        if (tintB != null) tintB.onValueChanged.AddListener(v => { _profile.secondaryTint.b = v; Refresh(); });
        if (scaleSlider != null) scaleSlider.onValueChanged.AddListener(v => { _profile.scale = v; Refresh(); });
        if (flipToggle != null) flipToggle.onValueChanged.AddListener(v => { _profile.flipHorizontal = v; Refresh(); });
    }

    private void OnClassSelected(int buttonIdx)
    {
        if (classForButton == null || buttonIdx >= classForButton.Length) return;
        _profile.avatarClass = classForButton[buttonIdx];
        Refresh();
        AudioManager.Instance?.PlaySFX(SFXType.UIClick);
    }

    private void OnColorSelected(int idx)
    {
        _profile.primaryColorIndex = idx;
        Refresh();
        AudioManager.Instance?.PlaySFX(SFXType.UIClick);
    }

    private void Refresh()
    {
        previewCustomizer?.Apply(_profile);
        if (summaryText != null)
            summaryText.text = $"{_profile.avatarClass} | color {_profile.primaryColorIndex} | scale {_profile.scale:F2}";
    }

    public void Confirm()
    {
        _profile.SaveForPlayer(playerIndex);
        AudioManager.Instance?.PlaySFX(SFXType.UIOpen);
        gameObject.SetActive(false);
    }
}
