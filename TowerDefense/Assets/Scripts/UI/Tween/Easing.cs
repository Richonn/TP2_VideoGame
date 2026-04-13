using UnityEngine;

public static class Easing
{
    public enum Ease
    {
        Linear,
        EaseInQuad,
        EaseOutQuad,
        EaseInOutQuad,
        EaseInCubic,
        EaseOutCubic,
        EaseInOutCubic,
        EaseOutBack,
        EaseInBack,
        EaseOutElastic,
        EaseInOutSine
    }

    public static float Evaluate(Ease type, float t)
    {
        t = Mathf.Clamp01(t);
        switch (type)
        {
            case Ease.Linear:         return t;
            case Ease.EaseInQuad:     return t * t;
            case Ease.EaseOutQuad:    return 1f - (1f - t) * (1f - t);
            case Ease.EaseInOutQuad:  return t < 0.5f ? 2f * t * t : 1f - Mathf.Pow(-2f * t + 2f, 2f) * 0.5f;
            case Ease.EaseInCubic:    return t * t * t;
            case Ease.EaseOutCubic:   return 1f - Mathf.Pow(1f - t, 3f);
            case Ease.EaseInOutCubic: return t < 0.5f ? 4f * t * t * t : 1f - Mathf.Pow(-2f * t + 2f, 3f) * 0.5f;
            case Ease.EaseOutBack:
            {
                const float c1 = 1.70158f;
                const float c3 = c1 + 1f;
                float x = t - 1f;
                return 1f + c3 * x * x * x + c1 * x * x;
            }
            case Ease.EaseInBack:
            {
                const float c1 = 1.70158f;
                const float c3 = c1 + 1f;
                return c3 * t * t * t - c1 * t * t;
            }
            case Ease.EaseOutElastic:
            {
                if (t <= 0f) return 0f;
                if (t >= 1f) return 1f;
                const float c4 = (2f * Mathf.PI) / 3f;
                return Mathf.Pow(2f, -10f * t) * Mathf.Sin((t * 10f - 0.75f) * c4) + 1f;
            }
            case Ease.EaseInOutSine:  return -(Mathf.Cos(Mathf.PI * t) - 1f) * 0.5f;
            default:                  return t;
        }
    }
}
