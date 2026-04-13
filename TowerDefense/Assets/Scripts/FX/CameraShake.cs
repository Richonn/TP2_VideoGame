using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    private Vector3 _basePosition;
    private Coroutine _current;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        _basePosition = transform.localPosition;
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void Shake(float amplitude, float duration)
    {
        if (_current != null)
        {
            StopCoroutine(_current);
            transform.localPosition = _basePosition;
        }
        _current = StartCoroutine(ShakeRoutine(amplitude, duration));
    }

    private IEnumerator ShakeRoutine(float amplitude, float duration)
    {
        _basePosition = transform.localPosition;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float damping = 1f - Mathf.Clamp01(t / duration);
            Vector2 offset = Random.insideUnitCircle * amplitude * damping;
            transform.localPosition = _basePosition + new Vector3(offset.x, offset.y, 0f);
            yield return null;
        }
        transform.localPosition = _basePosition;
        _current = null;
    }
}
