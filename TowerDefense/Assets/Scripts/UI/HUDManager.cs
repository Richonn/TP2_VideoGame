using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDManager : MonoBehaviour
{
    [Header("Player 1 HUD")]
    [SerializeField] private TMP_Text p1ResourceText;
    [SerializeField] private Image p1ReadyBar;
    [SerializeField] private Image p1AvatarIcon;

    [Header("Player 2 HUD")]
    [SerializeField] private TMP_Text p2ResourceText;
    [SerializeField] private Image p2ReadyBar;
    [SerializeField] private Image p2AvatarIcon;

    [Header("Shared HUD")]
    [SerializeField] private TMP_Text waveText;
    [SerializeField] private TMP_Text helpText;
    [SerializeField] private TMP_Text phaseText;
    [SerializeField] private Image phaseImage;
    [SerializeField] private Image baseHPBar;
    public Sprite prepSprite;
    public Sprite defenseSprite;

    [Header("References")]
    [SerializeField] private ReadySystem readySystem;

    private int _p1Last, _p2Last;
    private RectTransform _phaseTextRT;
    private Vector2 _phaseTextBasePos;
    private static readonly Color HP_FULL = new Color(0.35f, 0.85f, 0.35f, 1f);
    private static readonly Color HP_LOW  = new Color(0.9f, 0.25f, 0.25f, 1f);

    void OnEnable()
    {
        GameManager.OnPhaseChanged += OnPhaseChanged;
        GameManager.OnWaveChanged += OnWaveChanged;
        BaseController.OnHPChanged += OnHPChanged;
        ResourceManager.OnResourcesChanged += OnResourcesChanged;
    }

    void Start()
    {
        if (phaseText != null)
        {
            _phaseTextRT = phaseText.rectTransform;
            _phaseTextBasePos = _phaseTextRT.anchoredPosition;
        }

        if (ResourceManager.Instance != null)
        {
            _p1Last = ResourceManager.Instance.GetResources(1);
            _p2Last = ResourceManager.Instance.GetResources(2);
            if (p1ResourceText != null) p1ResourceText.text = _p1Last.ToString();
            if (p2ResourceText != null) p2ResourceText.text = _p2Last.ToString();
        }

        BaseController base_ = FindFirstObjectByType<BaseController>();
        if (base_ != null && baseHPBar != null)
            baseHPBar.fillAmount = (float)base_.CurrentHP / base_.MaxHP;
        if (AvatarSessionManager.Instance != null)
        {
            for (int p = 1; p <= 2; p++)
            {
                Sprite icon = AvatarSessionManager.Instance.GetAvatarIcon(
                    AvatarSessionManager.Instance.GetPlayerAvatar(p));
                if (icon != null)
                    UpdatePlayerAvatarIcon(p, icon);
            }
        }
    }

    void OnDisable()
    {
        GameManager.OnPhaseChanged -= OnPhaseChanged;
        GameManager.OnWaveChanged -= OnWaveChanged;
        BaseController.OnHPChanged -= OnHPChanged;
        ResourceManager.OnResourcesChanged -= OnResourcesChanged;
    }

    void Update()
    {
        if (readySystem == null) return;
        float prog = readySystem.Progression;
        if (p1ReadyBar != null) p1ReadyBar.fillAmount = prog;
        if (p2ReadyBar != null) p2ReadyBar.fillAmount = prog;
    }

    private void OnPhaseChanged(GameManager.GameState state)
    {
        if (phaseText != null)
        {
            phaseText.text = state switch
            {
                GameManager.GameState.Preparation => "PREPARE THE BASE",
                GameManager.GameState.Defense => "DEFEND",
                _ => ""
            };

            if (_phaseTextRT != null)
            {
                _phaseTextRT.anchoredPosition = _phaseTextBasePos + new Vector2(0f, 80f);
                UITween.MoveTo(_phaseTextRT, _phaseTextBasePos, 0.5f, Easing.Ease.EaseOutBack);
                UITween.Punch(phaseText.transform, 0.22f, 0.35f);
            }
        }

        if (phaseImage != null)
        {
            phaseImage.sprite = state switch
            {
                GameManager.GameState.Preparation => prepSprite,
                GameManager.GameState.Defense => defenseSprite,
                _ => prepSprite
            };
            UITween.Punch(phaseImage.transform, 0.2f, 0.3f);
        }

        if (helpText != null)
            helpText.text = state switch
            {
                GameManager.GameState.Preparation => "Hold Tab or B to start wave",
                GameManager.GameState.Defense => "Wait the end of the wave",
                _ => ""
            };
    }

    private void OnWaveChanged(int wave)
    {
        if (waveText != null)
        {
            waveText.text = $"Vague {wave}";
            UITween.Punch(waveText.transform, 0.25f, 0.4f);
        }
        AudioManager.Instance?.PlaySFX(SFXType.UIWaveStart);
    }

    private void OnHPChanged(int currentHP, int maxHP)
    {
        if (baseHPBar == null) return;
        float target = (float)currentHP / maxHP;
        UITween.FillTo(baseHPBar, target, 0.35f, Easing.Ease.EaseOutCubic);
        UITween.Punch(baseHPBar.transform, 0.01f, 0.22f);
    }

    private void OnResourcesChanged(int playerIndex, int amount)
    {
        if (playerIndex == 1 && p1ResourceText != null)
        {
            UITween.CountTo(p1ResourceText, _p1Last, amount, 0.4f);
            _p1Last = amount;
            UITween.Punch(p1ResourceText.transform, 0.18f, 0.25f);
        }
        else if (playerIndex == 2 && p2ResourceText != null)
        {
            UITween.CountTo(p2ResourceText, _p2Last, amount, 0.4f);
            _p2Last = amount;
            UITween.Punch(p2ResourceText.transform, 0.18f, 0.25f);
        }
    }

    public void UpdatePlayerAvatarIcon(int playerNumber, Sprite avatarIcon)
    {
        if (playerNumber == 1 && p1AvatarIcon != null)
        {
            p1AvatarIcon.sprite = avatarIcon;
            p1AvatarIcon.enabled = avatarIcon != null;
        }
        else if (playerNumber == 2 && p2AvatarIcon != null)
        {
            p2AvatarIcon.sprite = avatarIcon;
            p2AvatarIcon.enabled = avatarIcon != null;
        }
    }
}