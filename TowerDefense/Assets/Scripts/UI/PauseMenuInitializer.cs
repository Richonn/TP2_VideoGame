using UnityEngine;

/// <summary>
/// Script d'initialisation qui s'assure que le PauseMenuController existe dans la scène.
/// À ajouter à n'importe quel GameObject existant dans la scène (ex: GameManager)
/// </summary>
public class PauseMenuInitializer : MonoBehaviour
{
    void Awake()
    {
        // Si aucun PauseMenuController n'existe, le créer
        if (PauseMenuController.Instance == null)
        {
            Debug.Log("[PauseMenuInitializer] Création automatique du PauseMenuController...");
            
            GameObject pauseManagerGO = new GameObject("PauseMenuManager");
            pauseManagerGO.AddComponent<PauseMenuController>();
            
            Debug.Log("[PauseMenuInitializer] PauseMenuController créé avec succès!");
        }
    }
}
