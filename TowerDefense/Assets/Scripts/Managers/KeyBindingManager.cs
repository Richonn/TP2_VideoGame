using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Gère les bindings de touches personnalisés
/// Stocke et charge les configurations de touches depuis PlayerPrefs
/// </summary>
public class KeyBindingManager : MonoBehaviour
{
    public static KeyBindingManager Instance { get; private set; }

    [System.Serializable]
    public struct KeyBinding
    {
        public string ActionName;
        public KeyCode KeyboardKey;
        public string GamepadButton;  // ex: "joystick1button0" pour A button
    }

    private Dictionary<string, KeyBinding> keyBindings = new Dictionary<string, KeyBinding>();

    // Les actions disponibles
    public enum ActionType
    {
        Move_Up,
        Move_Down,
        Move_Left,
        Move_Right,
        PlaceTower,
        Interact,
        LancerVague
    }

    // Bindings par défaut
    private static readonly Dictionary<ActionType, KeyBinding> DefaultBindings = new Dictionary<ActionType, KeyBinding>()
    {
        { ActionType.Move_Up, new KeyBinding { ActionName = "Move_Up", KeyboardKey = KeyCode.Z, GamepadButton = "Up" } },
        { ActionType.Move_Down, new KeyBinding { ActionName = "Move_Down", KeyboardKey = KeyCode.S, GamepadButton = "Down" } },
        { ActionType.Move_Left, new KeyBinding { ActionName = "Move_Left", KeyboardKey = KeyCode.Q, GamepadButton = "Left" } },
        { ActionType.Move_Right, new KeyBinding { ActionName = "Move_Right", KeyboardKey = KeyCode.D, GamepadButton = "Right" } },
        { ActionType.PlaceTower, new KeyBinding { ActionName = "PlaceTower", KeyboardKey = KeyCode.E, GamepadButton = "Button0" } },
        { ActionType.Interact, new KeyBinding { ActionName = "Interact", KeyboardKey = KeyCode.F, GamepadButton = "Button3" } },
        { ActionType.LancerVague, new KeyBinding { ActionName = "LancerVague", KeyboardKey = KeyCode.Tab, GamepadButton = "Button1" } }
    };

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        LoadAllBindings();
    }

    /// <summary>
    /// Charge tous les bindings depuis PlayerPrefs
    /// </summary>
    private void LoadAllBindings()
    {
        keyBindings.Clear();
        
        foreach (var defaultBinding in DefaultBindings)
        {
            string key = $"KeyBinding_{defaultBinding.Value.ActionName}";
            
            if (PlayerPrefs.HasKey(key))
            {
                // Parser le binding sauvegardé (format: "KeyCode_name")
                string savedValue = PlayerPrefs.GetString(key);
                if (System.Enum.TryParse(savedValue, out KeyCode parsedKey))
                {
                    KeyBinding binding = defaultBinding.Value;
                    binding.KeyboardKey = parsedKey;
                    keyBindings[defaultBinding.Value.ActionName] = binding;
                }
                else
                {
                    keyBindings[defaultBinding.Value.ActionName] = defaultBinding.Value;
                }
            }
            else
            {
                keyBindings[defaultBinding.Value.ActionName] = defaultBinding.Value;
            }
        }
        
        Debug.Log("[KeyBindingManager] Bindings chargés");
    }

    /// <summary>
    /// Obtient le binding pour une action
    /// </summary>
    public KeyBinding GetBinding(ActionType action)
    {
        string actionName = action.ToString();
        if (keyBindings.ContainsKey(actionName))
            return keyBindings[actionName];
        
        return DefaultBindings[action];
    }

    /// <summary>
    /// Change le binding pour une action
    /// </summary>
    public void SetBinding(ActionType action, KeyCode newKey)
    {
        string actionName = action.ToString();
        KeyBinding binding = keyBindings.ContainsKey(actionName) ? keyBindings[actionName] : DefaultBindings[action];
        binding.KeyboardKey = newKey;
        keyBindings[actionName] = binding;
        
        // Sauvegarder
        SaveBinding(actionName, newKey);
        Debug.Log($"[KeyBindingManager] {actionName} changé à {newKey}");
    }

    /// <summary>
    /// Sauvegarde un binding dans PlayerPrefs
    /// </summary>
    private void SaveBinding(string actionName, KeyCode key)
    {
        string prefKey = $"KeyBinding_{actionName}";
        PlayerPrefs.SetString(prefKey, key.ToString());
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Réinitialise tous les bindings par défaut
    /// </summary>
    public void ResetAllBindings()
    {
        foreach (var defaultBinding in DefaultBindings)
        {
            string key = $"KeyBinding_{defaultBinding.Value.ActionName}";
            PlayerPrefs.DeleteKey(key);
        }
        PlayerPrefs.Save();
        
        LoadAllBindings();
        Debug.Log("[KeyBindingManager] Bindings réinitialisés");
    }

    /// <summary>
    /// Affiche le nom lisible d'une touche
    /// </summary>
    public static string GetKeyDisplayName(KeyCode key)
    {
        switch (key)
        {
            case KeyCode.Space: return "SPACE";
            case KeyCode.Return: return "ENTER";
            case KeyCode.Tab: return "TAB";
            case KeyCode.Escape: return "ESCAPE";
            case KeyCode.LeftShift: return "LSHIFT";
            case KeyCode.RightShift: return "RSHIFT";
            case KeyCode.LeftControl: return "LCTRL";
            case KeyCode.RightControl: return "RCTRL";
            default:
                string name = key.ToString().ToUpper();
                // Enlever le préfixe "Keypad" si présent
                if (name.StartsWith("KEYPAD"))
                    name = "KP_" + name.Substring(6);
                return name;
        }
    }

    /// <summary>
    /// Obtient tous les bindings (pour l'affichage)
    /// </summary>
    public List<KeyBinding> GetAllBindings()
    {
        List<KeyBinding> bindings = new List<KeyBinding>();
        foreach (var action in DefaultBindings.Keys)
        {
            bindings.Add(GetBinding(action));
        }
        return bindings;
    }
}
