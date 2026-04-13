using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class UIButtonFeedback : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerClickHandler
{
    [SerializeField] private float hoverScale = 1.08f;
    [SerializeField] private float pressScale = 0.95f;
    [SerializeField] private float duration = 0.12f;
    [SerializeField] private bool playSfx = true;

    private Vector3 _baseScale;
    private bool _initialized;

    public void Init()
    {
        if (_initialized) return;
        _baseScale = transform.localScale;
        _initialized = true;
    }

    void Awake() => Init();

    public void OnPointerEnter(PointerEventData eventData)
    {
        UITween.ScaleTo(transform, _baseScale * hoverScale, duration, Easing.Ease.EaseOutQuad);
        if (playSfx) AudioManager.Instance?.PlaySFX(SFXType.UIHover);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UITween.ScaleTo(transform, _baseScale, duration, Easing.Ease.EaseOutQuad);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        UITween.ScaleTo(transform, _baseScale * pressScale, duration * 0.6f, Easing.Ease.EaseOutQuad);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        UITween.ScaleTo(transform, _baseScale * hoverScale, duration, Easing.Ease.EaseOutBack);
        if (playSfx) AudioManager.Instance?.PlaySFX(SFXType.UIClick);
    }
}
