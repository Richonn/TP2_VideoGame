using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Gère le panneau des contrôles avec interface de rebinding
/// Utilise KeyBindingManager pour gérer les contrôles personnalisés
/// </summary>
public class ControlsPanelController : MonoBehaviour
{
    [Header("Références UI")]
    [SerializeField] private Transform rebindingContainer;
    
    private ControlsRebindingUI rebindingUI;

    void OnEnable()
    {
        if (rebindingUI == null)
        {
            InitializeControlsUI();
        }
    }

    private void InitializeControlsUI()
    {
        Debug.Log("[ControlsPanel] Initialisation du panneau des contrôles");

        // Trouver ou créer le container pour les rebindings
        if (rebindingContainer == null)
        {
            GameObject containerGO = new GameObject("RebindingContainer");
            containerGO.transform.SetParent(transform, false);
            rebindingContainer = containerGO.transform;
            
            // Ajouter un VerticalLayoutGroup
            VerticalLayoutGroup vlg = containerGO.AddComponent<VerticalLayoutGroup>();
            vlg.spacing = 10;
            vlg.padding = new RectOffset(20, 20, 20, 20);
            vlg.childForceExpandWidth = true;
            vlg.childForceExpandHeight = false;

            RectTransform rt = containerGO.GetComponent<RectTransform>();
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }

        // Créer ou obtenir le ControlsRebindingUI
        rebindingUI = GetComponent<ControlsRebindingUI>();
        if (rebindingUI == null)
        {
            rebindingUI = gameObject.AddComponent<ControlsRebindingUI>();
        }

        Debug.Log("[ControlsPanel] UI des contrôles initialisée");
    }

    public void ResetBindings()
    {
        Debug.Log("[ControlsPanel] Réinitialisation des contrôles par défaut");
        
        if (KeyBindingManager.Instance != null)
        {
            KeyBindingManager.Instance.ResetAllBindings();
            
            // Recharger le panneau
            foreach (Transform child in rebindingContainer)
            {
                Destroy(child.gameObject);
            }
            
            rebindingUI = null;
            InitializeControlsUI();
        }
    }

    public void GoBack()
    {
        Debug.Log("[ControlsPanel] Retour au menu principal");
        gameObject.SetActive(false);
    }
}


