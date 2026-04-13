using UnityEngine;

public class AutoReleaseParticle : MonoBehaviour
{
    [SerializeField] private float fallbackDuration = 1.5f;

    private VFXManager _manager;
    private VFXType _type;
    private ParticleSystem _ps;
    private float _timer;
    private bool _armed;

    public void Init(VFXManager manager, VFXType type)
    {
        _manager = manager;
        _type = type;
        _ps = GetComponent<ParticleSystem>();
        if (_ps != null)
        {
            ParticleSystem.MainModule main = _ps.main;
            main.stopAction = ParticleSystemStopAction.None;
        }
        _timer = 0f;
        _armed = true;
    }

    void OnEnable()
    {
        _timer = 0f;
        _armed = true;
    }

    void Update()
    {
        if (!_armed) return;
        _timer += Time.deltaTime;

        bool done;
        if (_ps != null)
            done = !_ps.IsAlive(true);
        else
            done = _timer >= fallbackDuration;

        if (done)
        {
            _armed = false;
            if (_manager != null)
                _manager.Release(_type, gameObject);
            else
                gameObject.SetActive(false);
        }
    }
}
