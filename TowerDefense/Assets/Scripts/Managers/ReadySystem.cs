using UnityEngine;

public class ReadySystem : MonoBehaviour
{
    [SerializeField] private float holdDuration = 1.5f;

    public float Progression { get; private set; }

    private float _heldTime;

    void Update()
    {
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.CurrentState != GameManager.GameState.Preparation) return;
        if (InputManager.Instance == null) return;

        bool held = InputManager.Instance.GetInput(1).LaunchWaveHeld
                 || InputManager.Instance.GetInput(2).LaunchWaveHeld;

        if (held)
        {
            _heldTime += Time.deltaTime;
            Progression = Mathf.Clamp01(_heldTime / holdDuration);

            if (_heldTime >= holdDuration)
                LaunchDefense();
        }
        else
        {
            _heldTime = Mathf.Max(0f, _heldTime - Time.deltaTime * 2f);
            Progression = Mathf.Clamp01(_heldTime / holdDuration);
        }
    }

    private void LaunchDefense()
    {
        _heldTime = 0f;
        Progression = 0f;
        GameManager.Instance.EnterDefensePhase();
    }
}
