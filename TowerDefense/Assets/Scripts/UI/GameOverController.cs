using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverController : MonoBehaviour
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text subtitleText;
    [SerializeField] private TMP_Text wavesText;

    void Start()
    {
        if (GameManager.Instance == null) return;

        bool victory = GameManager.Instance.IsVictory;
        int  waves   = GameManager.Instance.CurrentWave;

        if (titleText != null)
            titleText.text = victory ? "VICTOIRE !" : "GAME OVER";

        if (subtitleText != null)
            subtitleText.text = victory ? "La base a tenu bon." : "La base a été détruite.";

        if (wavesText != null)
            wavesText.text = $"Vagues survécues : {waves}";
    }

    public void OnReplayPressed()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.StartGame();
        else
            SceneManager.LoadScene("Game");
    }

    public void OnMenuPressed()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.ReturnToMenu();
        else
            SceneManager.LoadScene("MainMenu");
    }
}
