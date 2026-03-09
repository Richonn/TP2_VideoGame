using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    public void OnPlayPressed()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("[MainMenu] GameManager introuvable dans la scène !");
            return;
        }
        GameManager.Instance.StartGame();
    }

    public void OnQuitPressed()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
