using UnityEngine;

/// <summary>
/// Script utilitaire pour ajuster le zoom des caméras du jeu.
/// À mettre en debug ou à utiliser pour calibrer.
/// Orthographic Size = hauteur du monde visible en unités Unity
/// Plus grand = plus zoomé dehors (voir plus)
/// Plus petit = plus zoomé dedans (voir moins, mais plus grand)
/// </summary>
public class CameraZoomHelper : MonoBehaviour
{
    void Start()
    {
        LogCameraSettings();
    }

    /// <summary>
    /// Affiche les paramètres actuels des caméras
    /// </summary>
    void LogCameraSettings()
    {
        Camera[] cameras = FindObjectsByType<Camera>(FindObjectsSortMode.None);
        Debug.Log($"[CameraZoom] {cameras.Length} caméra(s) trouvée(s):");
        
        foreach (Camera cam in cameras)
        {
            if (cam.orthographic)
            {
                Debug.Log($"  - {cam.gameObject.name}: Orthographic Size = {cam.orthographicSize}");
            }
            else
            {
                Debug.Log($"  - {cam.gameObject.name}: FOV = {cam.fieldOfView}");
            }
        }
    }

    /// <summary>
    /// Suggestion de valeurs de zoom optimales :
    /// Pour un jeu Tower Defense avec split-screen 2 joueurs:
    /// - Orthographic Size: 5 (standard)
    /// - Si votre map est grande, augmentez à 6-7
    /// - Si votre map est petite, diminuez à 3-4
    /// </summary>
    public static void SetCameraZoom(float orthographicSize)
    {
        Camera[] cameras = FindObjectsByType<Camera>(FindObjectsSortMode.None);
        foreach (Camera cam in cameras)
        {
            if (cam.orthographic)
            {
                cam.orthographicSize = orthographicSize;
                Debug.Log($"[CameraZoom] {cam.gameObject.name}: Orthographic Size changée à {orthographicSize}");
            }
        }
    }
}
