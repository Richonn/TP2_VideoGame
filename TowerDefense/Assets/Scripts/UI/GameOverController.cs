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
        int waves = GameManager.Instance.CurrentWave;

        if (titleText != null)
            titleText.text = victory ? "VICTORY!" : "GAME OVER";

        if (subtitleText != null)
            subtitleText.text = victory ? "The base held." : "The base was destroyed.";

        if (wavesText != null)
            wavesText.text = $"Waves survived: {waves}";
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
