using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    [SerializeField, Range(0f, 0.5f)] private float deadZone = 0.15f;

    public struct PlayerInputData
    {
        public Vector2 MoveDirection;
        public bool PlaceTowerPressed;
        public bool InteractPressed;
        public bool PlaceTowerHeld;
        public bool InteractHeld;
        public bool LaunchWaveHeld;
    }

    private PlayerInputData[] _inputData = new PlayerInputData[2];
    private bool _playerInputEnabled1 = true;
    private bool _playerInputEnabled2 = true;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        EnsureKeyBindingManager();
    }

    private void EnsureKeyBindingManager()
    {
        if (KeyBindingManager.Instance == null)
        {
            GameObject go = new GameObject("KeyBindingManager");
            go.AddComponent<KeyBindingManager>();
            DontDestroyOnLoad(go);
        }
    }

    void Update()
    {
        _inputData[0] = _playerInputEnabled1 ? ProcessKeyboardInput() : new PlayerInputData();
        _inputData[1] = _playerInputEnabled2 ? ProcessGamepadInput() : new PlayerInputData();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseMenuController pauseController = PauseMenuController.Instance
                ?? FindFirstObjectByType<PauseMenuController>();

            if (pauseController == null)
            {
                GameObject go = new GameObject("PauseMenuManager");
                pauseController = go.AddComponent<PauseMenuController>();
            }

            pauseController?.TogglePause();
        }
    }

    private PlayerInputData ProcessKeyboardInput()
    {
        KeyBindingManager.KeyBinding up = KeyBindingManager.Instance.GetBinding(KeyBindingManager.ActionType.Move_Up);
        KeyBindingManager.KeyBinding down = KeyBindingManager.Instance.GetBinding(KeyBindingManager.ActionType.Move_Down);
        KeyBindingManager.KeyBinding left = KeyBindingManager.Instance.GetBinding(KeyBindingManager.ActionType.Move_Left);
        KeyBindingManager.KeyBinding right = KeyBindingManager.Instance.GetBinding(KeyBindingManager.ActionType.Move_Right);
        KeyBindingManager.KeyBinding placeTower = KeyBindingManager.Instance.GetBinding(KeyBindingManager.ActionType.PlaceTower);
        KeyBindingManager.KeyBinding interact = KeyBindingManager.Instance.GetBinding(KeyBindingManager.ActionType.Interact);
        KeyBindingManager.KeyBinding launchWave = KeyBindingManager.Instance.GetBinding(KeyBindingManager.ActionType.LaunchWave);

        Vector2 direction = Vector2.zero;
        if (Input.GetKey(up.KeyboardKey)) direction.y += 1;
        if (Input.GetKey(down.KeyboardKey)) direction.y -= 1;
        if (Input.GetKey(left.KeyboardKey)) direction.x -= 1;
        if (Input.GetKey(right.KeyboardKey)) direction.x += 1;
        if (direction.magnitude > 0) direction = direction.normalized;

        return new PlayerInputData
        {
            MoveDirection = direction,
            PlaceTowerPressed = Input.GetKeyDown(placeTower.KeyboardKey),
            InteractPressed = Input.GetKeyDown(interact.KeyboardKey),
            PlaceTowerHeld = Input.GetKey(placeTower.KeyboardKey),
            InteractHeld = Input.GetKey(interact.KeyboardKey),
            LaunchWaveHeld = Input.GetKey(launchWave.KeyboardKey),
        };
    }

    private PlayerInputData ProcessGamepadInput()
    {
        Gamepad gamepad = Gamepad.current;
        if (gamepad == null) return new PlayerInputData();

        Vector2 stick = gamepad.leftStick.ReadValue();
        if (stick.magnitude > deadZone)
            stick = stick.normalized * Mathf.InverseLerp(deadZone, 1f, stick.magnitude);
        else
            stick = Vector2.zero;

        return new PlayerInputData
        {
            MoveDirection = stick,
            PlaceTowerPressed = gamepad.buttonSouth.wasPressedThisFrame,
            InteractPressed = gamepad.buttonNorth.wasPressedThisFrame,
            PlaceTowerHeld = gamepad.buttonSouth.isPressed,
            InteractHeld = gamepad.buttonNorth.isPressed,
            LaunchWaveHeld = gamepad.buttonEast.isPressed,
        };
    }

    public PlayerInputData GetInput(int playerIndex)
    {
        int idx = Mathf.Clamp(playerIndex - 1, 0, 1);
        return _inputData[idx];
    }

    public void SetPlayerInputEnabled(int playerIndex, bool enabled)
    {
        if (playerIndex == 1) _playerInputEnabled1 = enabled;
        else if (playerIndex == 2) _playerInputEnabled2 = enabled;
    }
}
