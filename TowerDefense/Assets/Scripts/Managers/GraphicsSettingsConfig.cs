using UnityEngine;

/// <summary>
/// Script de configuration des paramètres de graphiques et résolution.
/// À utiliser en cas de problèmes de zoom ou d'affichage.
/// </summary>
public class GraphicsSettings : MonoBehaviour
{
    void Start()
    {
        Debug.Log("[GraphicsSettings] Configuration actuelle:");
        Debug.Log($"  - Résolution: {Screen.width} x {Screen.height}");
        Debug.Log($"  - Refresh Rate: {Screen.currentResolution.refreshRateRatio}");
        Debug.Log($"  - Quality Level: {QualitySettings.GetQualityLevel()}");
        Debug.Log($"  - VSyncCount: {QualitySettings.vSyncCount}");
        Debug.Log($"  - TargetFrameRate: {Application.targetFrameRate}");
    }

    /// <summary>
    /// Configure les paramètres de base pour une expérience optimale
    /// </summary>
    public static void ConfigureOptimal()
    {
        Debug.Log("[GraphicsSettings] Configuration optimale appliquée...");
        
        // Définir la résolution en 1920x1080 (standard)
        Screen.SetResolution(1920, 1080, FullScreenMode.Windowed);
        
        // Viser 60 FPS
        Application.targetFrameRate = 60;
        
        // VSync activé
        QualitySettings.vSyncCount = 1;
        
        Debug.Log("[GraphicsSettings] Résolution: 1920x1080, TargetFPS: 60, VSync: ON");
    }

    /// <summary>
    /// Pour debug: affiche l'orthographic size des caméras
    /// </summary>
    public static void DebugCameras()
    {
        Camera[] cameras = FindObjectsByType<Camera>(FindObjectsSortMode.None);
        Debug.Log($"[GraphicsSettings] Caméras trouvées: {cameras.Length}");
        
        foreach (Camera cam in cameras)
        {
            if (cam.orthographic)
            {
                Debug.Log($"  {cam.gameObject.name}: Size = {cam.orthographicSize}, Position = {cam.transform.position}, Viewport = {cam.rect}");
            }
        }
    }
}
