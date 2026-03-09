using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState { Menu, Preparation, Defense, GameOver }
    public GameState CurrentState { get; private set; } = GameState.Menu;

    public int CurrentWave { get; private set; }
    public float PrepTimeRemaining { get; private set; }
    public bool IsVictory { get; private set; }

    [Header("Phase de préparation")]
    [Tooltip("Durée du timer de préparation en secondes.")]
    [SerializeField] private float prepDuration = 30f;

    public static event Action<GameState> OnPhaseChanged;
    public static event Action<int> OnWaveChanged;
    public static event Action<float> OnPrepTimerUpdated;
    public static event Action<bool> OnGameEnded;

    private bool _prepRunning;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (_prepRunning)
            TickPrepTimer();
    }

    public void StartGame()
    {
        CurrentWave = 0;
        SceneManager.sceneLoaded += OnGameSceneLoaded;
        SceneManager.LoadScene("Game");
    }

    private void OnGameSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "Game") return;
        SceneManager.sceneLoaded -= OnGameSceneLoaded;
        EnterPreparationPhase();
    }

    public void EnterPreparationPhase()
    {
        CurrentState = GameState.Preparation;
        PrepTimeRemaining = prepDuration;
        _prepRunning = true;

        InputManager.Instance?.SetPlayerInputEnabled(1, true);
        InputManager.Instance?.SetPlayerInputEnabled(2, true);

        OnPhaseChanged?.Invoke(CurrentState);
    }

    private void TickPrepTimer()
    {
        PrepTimeRemaining -= Time.deltaTime;

        if (PrepTimeRemaining <= 0f)
        {
            PrepTimeRemaining = 0f;
            _prepRunning = false;
        }

        OnPrepTimerUpdated?.Invoke(PrepTimeRemaining);
    }

    public void EnterDefensePhase()
    {
        _prepRunning = false;
        CurrentState = GameState.Defense;
        CurrentWave++;

        OnPhaseChanged?.Invoke(CurrentState);
        OnWaveChanged?.Invoke(CurrentWave);
    }

    public void WaveCompleted()
    {
        EnterPreparationPhase();
    }

    public void TriggerGameOver(bool victory)
    {
        IsVictory = victory;
        CurrentState = GameState.GameOver;
        _prepRunning = false;

        OnPhaseChanged?.Invoke(CurrentState);
        OnGameEnded?.Invoke(victory);

        SceneManager.LoadScene("GameOver");
    }

    public void ReturnToMenu()
    {
        CurrentState = GameState.Menu;
        SceneManager.LoadScene("MainMenu");
    }
}
