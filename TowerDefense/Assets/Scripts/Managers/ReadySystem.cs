using UnityEngine;

/// <summary>
/// Pendant la phase de préparation, n'importe quel joueur peut maintenir
/// sa touche "LancerVague" (Espace / buttonEast) pour démarrer la défense.
///
/// Une barre de progression se charge pendant <dureeTenue> secondes.
/// Si le joueur relâche avant, la barre se vide.
///
/// Placer ce script dans la scène Game sur un GameObject dédié.
/// </summary>
public class ReadySystem : MonoBehaviour
{
    [Tooltip("Durée de maintien nécessaire pour lancer la vague (secondes).")]
    [SerializeField] private float dureeTenue = 1.5f;

    // Progression actuelle exposée au HUD (0..1)
    public float Progression { get; private set; }

    private float _tempsMaintienu;

    void Update()
    {
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.CurrentState != GameManager.GameState.Preparation) return;
        if (InputManager.Instance == null) return;

        bool appuye = InputManager.Instance.GetInput(1).LancerVagueHeld
                   || InputManager.Instance.GetInput(2).LancerVagueHeld;

        if (appuye)
        {
            _tempsMaintienu += Time.deltaTime;
            Progression = Mathf.Clamp01(_tempsMaintienu / dureeTenue);

            if (_tempsMaintienu >= dureeTenue)
                LancerDefense();
        }
        else
        {
            // Relâché : vider progressivement (2x plus vite)
            _tempsMaintienu = Mathf.Max(0f, _tempsMaintienu - Time.deltaTime * 2f);
            Progression = Mathf.Clamp01(_tempsMaintienu / dureeTenue);
        }
    }

    private void LancerDefense()
    {
        _tempsMaintienu = 0f;
        Progression = 0f;
        Debug.Log("[ReadySystem] Lancement de la vague !");
        GameManager.Instance.EnterDefensePhase();
    }
}
