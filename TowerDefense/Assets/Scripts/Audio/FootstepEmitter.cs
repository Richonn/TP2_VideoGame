using UnityEngine;

public class FootstepEmitter : MonoBehaviour
{
    [SerializeField] private SFXType footstepType = SFXType.EnemyFootstep;
    [SerializeField] private float minInterval = 0.08f;

    [Header("Auto mode (no AnimationEvent required)")]
    [SerializeField] private bool autoEmit = false;
    [SerializeField] private float autoInterval = 0.35f;
    [SerializeField] private float movementEpsilon = 0.0005f;

    private float _lastStep;
    private Vector3 _lastPosition;
    private float _autoTimer;

    public SFXType Type { get => footstepType; set => footstepType = value; }
    public bool AutoEmit { get => autoEmit; set => autoEmit = value; }

    void Start()
    {
        _lastPosition = transform.position;
    }

    void Update()
    {
        if (!autoEmit) return;

        float sqrMoved = (transform.position - _lastPosition).sqrMagnitude;
        _lastPosition = transform.position;

        if (sqrMoved < movementEpsilon)
        {
            _autoTimer = 0f;
            return;
        }

        _autoTimer += Time.deltaTime;
        if (_autoTimer >= autoInterval)
        {
            _autoTimer = 0f;
            OnStep();
        }
    }

    public void OnStep()
    {
        if (Time.time - _lastStep < minInterval) return;
        _lastStep = Time.time;
        AudioManager.Instance?.PlaySFX(footstepType, transform.position);
    }
}
