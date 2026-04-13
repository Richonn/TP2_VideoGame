using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public static class UITween
{
    public static Coroutine FadeTo(CanvasGroup cg, float target, float duration, Easing.Ease ease = Easing.Ease.EaseOutQuad)
    {
        if (cg == null) return null;
        return UITweenRunner.Instance.StartCoroutine(FadeRoutine(cg, target, duration, ease));
    }

    public static Coroutine FadeTo(Graphic g, float targetAlpha, float duration, Easing.Ease ease = Easing.Ease.EaseOutQuad)
    {
        if (g == null) return null;
        return UITweenRunner.Instance.StartCoroutine(GraphicAlphaRoutine(g, targetAlpha, duration, ease));
    }

    public static Coroutine ScaleTo(Transform t, Vector3 target, float duration, Easing.Ease ease = Easing.Ease.EaseOutBack)
    {
        if (t == null) return null;
        return UITweenRunner.Instance.StartCoroutine(ScaleRoutine(t, target, duration, ease));
    }

    public static Coroutine MoveTo(RectTransform rt, Vector2 targetAnchoredPos, float duration, Easing.Ease ease = Easing.Ease.EaseOutCubic)
    {
        if (rt == null) return null;
        return UITweenRunner.Instance.StartCoroutine(MoveRoutine(rt, targetAnchoredPos, duration, ease));
    }

    public static Coroutine ColorTo(Graphic g, Color target, float duration, Easing.Ease ease = Easing.Ease.EaseOutQuad)
    {
        if (g == null) return null;
        return UITweenRunner.Instance.StartCoroutine(ColorRoutine(g, target, duration, ease));
    }

    public static Coroutine FillTo(Image img, float target, float duration, Easing.Ease ease = Easing.Ease.EaseOutCubic)
    {
        if (img == null) return null;
        return UITweenRunner.Instance.StartCoroutine(FillRoutine(img, target, duration, ease));
    }

    public static Coroutine Punch(Transform t, float strength = 0.18f, float duration = 0.22f)
    {
        if (t == null) return null;
        return UITweenRunner.Instance.StartCoroutine(PunchRoutine(t, strength, duration));
    }

    public static Coroutine CountTo(TMP_Text text, int from, int to, float duration, string format = "{0}", Easing.Ease ease = Easing.Ease.EaseOutCubic)
    {
        if (text == null) return null;
        return UITweenRunner.Instance.StartCoroutine(CountRoutine(text, from, to, duration, format, ease));
    }

    private static IEnumerator FadeRoutine(CanvasGroup cg, float target, float duration, Easing.Ease ease)
    {
        float start = cg.alpha;
        float t = 0f;
        while (t < duration)
        {
            if (cg == null) yield break;
            t += Time.unscaledDeltaTime;
            cg.alpha = Mathf.Lerp(start, target, Easing.Evaluate(ease, t / duration));
            yield return null;
        }
        if (cg != null) cg.alpha = target;
    }

    private static IEnumerator GraphicAlphaRoutine(Graphic g, float target, float duration, Easing.Ease ease)
    {
        Color c = g.color;
        float start = c.a;
        float t = 0f;
        while (t < duration)
        {
            if (g == null) yield break;
            t += Time.unscaledDeltaTime;
            c.a = Mathf.Lerp(start, target, Easing.Evaluate(ease, t / duration));
            g.color = c;
            yield return null;
        }
        if (g != null) { c.a = target; g.color = c; }
    }

    private static IEnumerator ScaleRoutine(Transform tr, Vector3 target, float duration, Easing.Ease ease)
    {
        Vector3 start = tr.localScale;
        float t = 0f;
        while (t < duration)
        {
            if (tr == null) yield break;
            t += Time.unscaledDeltaTime;
            tr.localScale = Vector3.LerpUnclamped(start, target, Easing.Evaluate(ease, t / duration));
            yield return null;
        }
        if (tr != null) tr.localScale = target;
    }

    private static IEnumerator MoveRoutine(RectTransform rt, Vector2 target, float duration, Easing.Ease ease)
    {
        Vector2 start = rt.anchoredPosition;
        float t = 0f;
        while (t < duration)
        {
            if (rt == null) yield break;
            t += Time.unscaledDeltaTime;
            rt.anchoredPosition = Vector2.LerpUnclamped(start, target, Easing.Evaluate(ease, t / duration));
            yield return null;
        }
        if (rt != null) rt.anchoredPosition = target;
    }

    private static IEnumerator ColorRoutine(Graphic g, Color target, float duration, Easing.Ease ease)
    {
        Color start = g.color;
        float t = 0f;
        while (t < duration)
        {
            if (g == null) yield break;
            t += Time.unscaledDeltaTime;
            g.color = Color.Lerp(start, target, Easing.Evaluate(ease, t / duration));
            yield return null;
        }
        if (g != null) g.color = target;
    }

    private static IEnumerator FillRoutine(Image img, float target, float duration, Easing.Ease ease)
    {
        float start = img.fillAmount;
        float t = 0f;
        while (t < duration)
        {
            if (img == null) yield break;
            t += Time.unscaledDeltaTime;
            img.fillAmount = Mathf.Lerp(start, target, Easing.Evaluate(ease, t / duration));
            yield return null;
        }
        if (img != null) img.fillAmount = target;
    }

    private static IEnumerator PunchRoutine(Transform tr, float strength, float duration)
    {
        Vector3 baseScale = tr.localScale;
        float t = 0f;
        while (t < duration)
        {
            if (tr == null) yield break;
            t += Time.unscaledDeltaTime;
            float n = t / duration;
            float s = 1f + strength * Mathf.Sin(n * Mathf.PI) * (1f - n * 0.3f);
            tr.localScale = baseScale * s;
            yield return null;
        }
        if (tr != null) tr.localScale = baseScale;
    }

    private static IEnumerator CountRoutine(TMP_Text text, int from, int to, float duration, string format, Easing.Ease ease)
    {
        float t = 0f;
        while (t < duration)
        {
            if (text == null) yield break;
            t += Time.unscaledDeltaTime;
            int cur = Mathf.RoundToInt(Mathf.Lerp(from, to, Easing.Evaluate(ease, t / duration)));
            text.text = string.Format(format, cur);
            yield return null;
        }
        if (text != null) text.text = string.Format(format, to);
    }
}
