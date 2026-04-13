using System.Collections;
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

        AudioManager.Instance?.PlayMusic(victory ? MusicTrack.Victory : MusicTrack.Defeat, 0.6f);
        StartCoroutine(CascadeIn());
    }

    private IEnumerator CascadeIn()
    {
        Transform[] targets = { titleText?.transform, subtitleText?.transform, wavesText?.transform };
        foreach (Transform t in targets)
        {
            if (t == null) continue;
            t.localScale = Vector3.zero;
        }
        yield return null;

        for (int i = 0; i < targets.Length; i++)
        {
            if (targets[i] == null) continue;
            UITween.ScaleTo(targets[i], Vector3.one, 0.45f, Easing.Ease.EaseOutBack);
            yield return new WaitForSecondsRealtime(0.15f);
        }
    }

    public void OnReplayPressed()
    {
        AudioManager.Instance?.PlaySFX(SFXType.UIClick);
        if (GameManager.Instance != null)
            GameManager.Instance.StartGame();
        else
            SceneManager.LoadScene("Game");
    }

    public void OnMenuPressed()
    {
        AudioManager.Instance?.PlaySFX(SFXType.UIBack);
        if (GameManager.Instance != null)
            GameManager.Instance.ReturnToMenu();
        else
            SceneManager.LoadScene("MainMenu");
    }
}
