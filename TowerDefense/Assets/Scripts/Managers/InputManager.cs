using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    [SerializeField] private InputActionAsset inputActionsAsset;

    [Tooltip("Rayon du seuil de dead zone (0–1)")]
    [SerializeField, Range(0f, 0.5f)] private float deadZone = 0.15f;

    public struct PlayerInputData
    {
        public Vector2 MoveDirection;
        public bool PlaceTowerPressed;
        public bool InteractPressed;
        public bool PlaceTowerHeld;
        public bool InteractHeld;
        public bool LancerVagueHeld;
    }

    private InputActionMap _p1Map;
    private InputActionMap _p2Map;

    private InputAction _p1Move, _p1Place, _p1Interact, _p1LancerVague;
    private InputAction _p2Move, _p2Place, _p2Interact, _p2LancerVague;

    private PlayerInputData[] _inputData = new PlayerInputData[2];

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitialiserActions();
    }

    void Update()
    {
        _inputData[0] = TraiterInputs(_p1Move, _p1Place, _p1Interact, _p1LancerVague);
        _inputData[1] = TraiterInputs(_p2Move, _p2Place, _p2Interact, _p2LancerVague);
    }

    void OnDestroy()
    {
        _p1Map?.Disable();
        _p2Map?.Disable();
    }

    private void InitialiserActions()
    {
        _p1Map = inputActionsAsset.FindActionMap("Player1", throwIfNotFound: true);
        _p2Map = inputActionsAsset.FindActionMap("Player2", throwIfNotFound: true);

        _p1Move        = _p1Map.FindAction("Move",        throwIfNotFound: true);
        _p1Place       = _p1Map.FindAction("PlaceTower",  throwIfNotFound: true);
        _p1Interact    = _p1Map.FindAction("Interact",    throwIfNotFound: true);
        _p1LancerVague = _p1Map.FindAction("LancerVague", throwIfNotFound: true);

        _p2Move        = _p2Map.FindAction("Move",        throwIfNotFound: true);
        _p2Place       = _p2Map.FindAction("PlaceTower",  throwIfNotFound: true);
        _p2Interact    = _p2Map.FindAction("Interact",    throwIfNotFound: true);
        _p2LancerVague = _p2Map.FindAction("LancerVague", throwIfNotFound: true);

        _p1Map.Enable();
        _p2Map.Enable();
    }


    private PlayerInputData TraiterInputs(
        InputAction move, InputAction place, InputAction interact, InputAction lancerVague)
    {
        Vector2 brut = move.ReadValue<Vector2>();
        float magnitude = brut.magnitude;

        Vector2 direction;
        if (magnitude <= deadZone)
        {
            direction = Vector2.zero;
        }
        else
        {
            float magnitudeRemappee = Mathf.InverseLerp(deadZone, 1f, magnitude);
            direction = brut.normalized * magnitudeRemappee;
        }

        return new PlayerInputData
        {
            MoveDirection     = direction,
            PlaceTowerPressed = place.WasPressedThisFrame(),
            InteractPressed   = interact.WasPressedThisFrame(),
            PlaceTowerHeld    = place.IsPressed(),
            InteractHeld      = interact.IsPressed(),
            LancerVagueHeld   = lancerVague.IsPressed(),
        };
    }

    public PlayerInputData GetInput(int playerIndex)
    {
        int idx = Mathf.Clamp(playerIndex - 1, 0, 1);
        return _inputData[idx];
    }

    public void SetPlayerInputEnabled(int playerIndex, bool enabled)
    {
        InputActionMap map = playerIndex == 1 ? _p1Map : _p2Map;
        if (enabled) map.Enable();
        else  map.Disable();
    }
}
